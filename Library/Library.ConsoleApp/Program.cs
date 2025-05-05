using Library.Common;
using System.Reflection.PortableExecutable;

namespace Library.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var bookService = new InMemoryCrudService<Book>();

            var book = new Book
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Pages = 464,
                Year = 2008
            };

            bookService.Create(book);

            Console.WriteLine("\uD83D\uDCDA Додано книги:");
            foreach (var b in bookService.ReadAll())
            {
                b.DisplayInfo();
            }

            var path = "books.json";
            bookService.Save(path);
            Console.WriteLine($"✅ Збережено у файл {path}");

            // Події й делегати
            var reader = new Reader
            {
                Name = "Іван Іванов",
                Email = "ivan@example.com"
            };
            reader.OnNotification += msg => Console.WriteLine($"[🔔] {msg}");
            reader.Notify("Нова книга доступна в бібліотеці!");

            // Метод-розширення
            reader.Display();
        }
    }
}