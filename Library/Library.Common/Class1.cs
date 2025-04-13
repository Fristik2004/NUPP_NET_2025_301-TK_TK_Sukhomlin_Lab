using Library.Common;
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Library.Common
{
    public abstract class LibraryItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public abstract void DisplayInfo();
    }

    public class Book : LibraryItem
    {
        public string Author { get; set; }
        public int Pages { get; set; }

        public Book()
        {
            Id = Guid.NewGuid();
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Книга: {Title}, Автор: {Author}, Сторінки: {Pages}");
        }

        public static Book CreateNew()
        {
            var rnd = new Random();
            return new Book
            {
                Title = $"Book {Guid.NewGuid().ToString().Substring(0, 5)}",
                Author = "Author " + rnd.Next(1, 100),
                Pages = rnd.Next(100, 1000),
                Year = rnd.Next(1950, 2025)
            };
        }
    }

    public class Magazine : LibraryItem
    {
        public int Issue { get; set; }
        public string Publisher { get; set; }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Журнал: {Title}, Випуск: {Issue}, Видавець: {Publisher}");
        }
    }

    public class Reader
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public event Action<string>? OnNotification;

        public void Notify(string message)
        {
            OnNotification?.Invoke(message);
        }

        public static Reader CreateNew()
        {
            var rnd = new Random();
            return new Reader
            {
                Id = Guid.NewGuid(),
                Name = $"Reader {rnd.Next(1, 1000)}",
                Email = $"reader{rnd.Next(1, 1000)}@example.com"
            };
        }
    }

    public static class ReaderExtensions
    {
        public static void Display(this Reader reader)
        {
            Console.WriteLine($"Читач: {reader.Name}, Email: {reader.Email}");
        }
    }

    public interface ICrudService<T>
    {
        void Create(T element);
        T Read(Guid id);
        IEnumerable<T> ReadAll();
        void Update(T element);
        void Remove(T element);
        void Save(string filePath);
        void Load(string filePath);
    }

    public interface ICrudServiceAsync<T> : IEnumerable<T>
    {
        Task<bool> CreateAsync(T element);
        Task<T> ReadAsync(Guid id);
        Task<IEnumerable<T>> ReadAllAsync();
        Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
        Task<bool> UpdateAsync(T element);
        Task<bool> RemoveAsync(T element);
        Task<bool> SaveAsync(string filePath);
    }

    public class InMemoryCrudService<T> : ICrudService<T> where T : class
    {
        private List<T> _items = new();

        public void Create(T element) => _items.Add(element);

        public T Read(Guid id)
        {
            var prop = typeof(T).GetProperty("Id");
            return _items.FirstOrDefault(i => (Guid?)prop?.GetValue(i) == id)!;
        }

        public IEnumerable<T> ReadAll() => _items;

        public void Update(T element)
        {
            Remove(element);
            Create(element);
        }

        public void Remove(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            var id = (Guid?)prop?.GetValue(element);
            var existing = Read(id!.Value);
            _items.Remove(existing);
        }

        public void Save(string filePath)
        {
            var json = JsonSerializer.Serialize(_items);
            File.WriteAllText(filePath, json);
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _items = JsonSerializer.Deserialize<List<T>>(json)!;
        }
    }

    public class InMemoryCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _items = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<bool> CreateAsync(T element)
        {
            await _semaphore.WaitAsync();
            try
            {
                var id = (Guid?)typeof(T).GetProperty("Id")?.GetValue(element);
                if (id == null) return false;
                return _items.TryAdd(id.Value, element);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            await Task.Yield();
            return _items.TryGetValue(id, out var item) ? item : null!;
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            await Task.Yield();
            return _items.Values;
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            await Task.Yield();
            return _items.Values.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(T element)
        {
            await _semaphore.WaitAsync();
            try
            {
                var id = (Guid?)typeof(T).GetProperty("Id")?.GetValue(element);
                if (id == null || !_items.ContainsKey(id.Value)) return false;
                _items[id.Value] = element;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> RemoveAsync(T element)
        {
            await _semaphore.WaitAsync();
            try
            {
                var id = (Guid?)typeof(T).GetProperty("Id")?.GetValue(element);
                if (id == null) return false;
                return _items.TryRemove(id.Value, out _);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> SaveAsync(string filePath)
        {
            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(_items.Values);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch { return false; }
            finally
            {
                _semaphore.Release();
            }
        }

        public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
