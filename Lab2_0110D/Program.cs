using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lab2_0110D.Common;
using Lab2_0110D.Services;

namespace Lab2_0110D
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string filePath = "smartphones.json";

            var smartphoneService = new CrudServiceAsync<Smartphone>(
                s => s.Id,
                filePath);

            object lockObj = new object();
            var semaphore = new SemaphoreSlim(10); // максимум 10 одночасно
            var resetEvent = new AutoResetEvent(true);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var phone = Smartphone.CreateNew();

                    // 🔐 lock приклад
                    lock (lockObj)
                    {
                        // можна логувати або змінювати лічильник
                    }

                    // 🔐 SemaphoreSlim приклад
                    await semaphore.WaitAsync();
                    try
                    {
                        await smartphoneService.CreateAsync(phone);
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                    // 🔐 AutoResetEvent приклад
                    resetEvent.WaitOne(); // блокування
                    resetEvent.Set();     // пропустити наступного
                }));
            }

            await Task.WhenAll(tasks);

            await smartphoneService.SaveAsync();

            var allPhones = await smartphoneService.ReadAllAsync();
            var prices = allPhones.Select(p => p.Price);

            Console.WriteLine("Статистика по ціні:");
            Console.WriteLine($"Мінімальна: {prices.Min()}");
            Console.WriteLine($"Максимальна: {prices.Max()}");
            Console.WriteLine($"Середня:    {prices.Average():F2}");

            Console.WriteLine($"\nЗбережено {allPhones.Count()} смартфонів у файл {filePath}.");
            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
