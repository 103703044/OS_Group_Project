using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SleepingBarber
{
    class Program
    {
        public static Queue<int> customers = new Queue<int>();

        static void Main(string[] args)
        {
            Thread PaulMitchell = new Thread(Barber);
            Thread Person = new Thread(Customer);
            Person.Start();
            PaulMitchell.Start();   
        }

        private static void Barber()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (customers.Count > 0)
                {
                    Console.WriteLine("I must wake up and cut {0} heads", customers.Count);
                    lock (customers)
                    {
                        customers.Dequeue();
                    }
                }
                else
                {
                    Console.WriteLine("I'm Sleeping ZZZZ");
                }
            }
        }

        private static void Customer()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (customers.Count < 5)
                {
                    Console.WriteLine("My turn, {0} people left", customers.Count);
                    lock (customers)
                    {
                        customers.Enqueue(1);
                    }
                }

                else
                {
                    Console.WriteLine("Too many people, I'll come back later");
                }
               
            }
        }
    }
}
