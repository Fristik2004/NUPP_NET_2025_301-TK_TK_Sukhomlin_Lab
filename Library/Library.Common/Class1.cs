using Library.Common;

namespace Library.Common
{
    public abstract class LibraryItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        // Абстрактний метод
        public abstract void DisplayInfo();
    }

    public class Book : LibraryItem
    {
        public string Author { get; set; }
        public int Pages { get; set; }

        // Конструктор
        public Book()
        {
            Id = Guid.NewGuid();
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Книга: {Title}, Автор: {Author}, Сторінки: {Pages}");
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

        // Подія
        public event Action<string>? OnNotification;

        public void Notify(string message)
        {
            OnNotification?.Invoke(message);
        }
    }

    public static class ReaderExtensions
    {
        // Метод-розширення
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
            var json = System.Text.Json.JsonSerializer.Serialize(_items);
            File.WriteAllText(filePath, json);
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _items = System.Text.Json.JsonSerializer.Deserialize<List<T>>(json)!;
        }
    }
}