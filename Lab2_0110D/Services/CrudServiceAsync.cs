using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab2_0110D.Services
{
   

    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _storage;
        private readonly Func<T, Guid> _idSelector;
        private readonly string _filePath;
        private readonly object _fileLock = new object();

        public CrudServiceAsync(Func<T, Guid> idSelector, string filePath)
        {
            _storage = new ConcurrentDictionary<Guid, T>();
            _idSelector = idSelector;
            _filePath = filePath;

            LoadFromFile();
        }

        public async Task<bool> CreateAsync(T element)
        {
            return await Task.Run(() =>
            {
                var id = _idSelector(element);
                return _storage.TryAdd(id, element);
            });
        }

        public async Task<T> ReadAsync(Guid id)
        {
            return await Task.Run(() =>
            {
                _storage.TryGetValue(id, out var element);
                return element;
            });
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await Task.Run(() => _storage.Values.AsEnumerable());
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            return await Task.Run(() =>
            {
                return _storage.Values
                    .Skip((page - 1) * amount)
                    .Take(amount)
                    .ToList();
            });
        }

        public async Task<bool> UpdateAsync(T element)
        {
            return await Task.Run(() =>
            {
                var id = _idSelector(element);
                if (!_storage.ContainsKey(id))
                    return false;

                _storage[id] = element;
                return true;
            });
        }

        public async Task<bool> RemoveAsync(T element)
        {
            return await Task.Run(() =>
            {
                var id = _idSelector(element);
                return _storage.TryRemove(id, out _);
            });
        }

        public async Task<bool> SaveAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(_storage.Values.ToList(), Formatting.Indented);

                    lock (_fileLock)
                    {
                        File.WriteAllText(_filePath, json);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath))
                return;

            try
            {
                string json;
                lock (_fileLock)
                {
                    json = File.ReadAllText(_filePath);
                }

                var list = JsonConvert.DeserializeObject<List<T>>(json);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        var id = _idSelector(item);
                        _storage.TryAdd(id, item);
                    }
                }
            }
            catch
            {
                
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
