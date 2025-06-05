using System;

namespace Lab2_0110D.Common
{
    public class Charger : Device
    {
        public int Power { get; set; }

        public Charger(string name, double price, int power) : base(name, price)
        {
            Power = power;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Потужність: {Power} Вт");
        }

        public static Charger CreateNew()
        {
            var rnd = new Random();
            int[] powerOptions = { 18, 30, 45, 65, 100 };
            return new Charger("Зарядка " + rnd.Next(1000), rnd.Next(400, 3000), powerOptions[rnd.Next(powerOptions.Length)]);
        }
    }
}
