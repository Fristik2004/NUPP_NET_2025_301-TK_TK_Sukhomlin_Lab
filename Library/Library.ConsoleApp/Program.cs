using Library.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Library.ConsoleApp
{
    internal class Program
    {
        private static readonly object _lockObj = new();
        private static readonly SemaphoreSlim _semaphore = new(3); // Дозволяє 3 паралельні операції
        private static readonly AutoResetEvent _autoResetEvent = new(true);

        static async Task Main(string[] args)
        {
            var service = new InMemoryCrudServiceAsync<Book>();
            var books = new List<Book>();

            // Паралельне створення книг з використанням lock, Semaphore, AutoResetEvent
            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await _semaphore.WaitAsync();
                    _autoResetEvent.WaitOne();
                    try
                    {
                        var book = Book.CreateNew();
                        lock (_lockObj)
                        {
                            books.Add(book);
                        }
                        await service.CreateAsync(book);
                    }
                    finally
                    {
                        _autoResetEvent.Set();
                        _semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Обчислення статистики
            var minPages = books.Min(b => b.Pages);
            var maxPages = books.Max(b => b.Pages);
            var avgPages = books.Average(b => b.Pages);

            Console.WriteLine($"Мінімальна кількість сторінок: {minPages}");
            Console.WriteLine($"Максимальна кількість сторінок: {maxPages}");
            Console.WriteLine($"Середня кількість сторінок: {avgPages:F2}");

            // Збереження у файл
            string path = "books_async.json";
            await service.SaveAsync(path);
            Console.WriteLine($"Книги збережено у файл: {path}");
        }
    }
}