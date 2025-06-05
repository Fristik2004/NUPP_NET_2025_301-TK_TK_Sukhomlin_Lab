using System;

namespace Lab2_0110D.Common
{
    public abstract class Device
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Price { get; set; }

        protected Device(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public virtual void PrintInfo()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Назва: {Name}, Ціна: {Price}");
        }
    }
}
