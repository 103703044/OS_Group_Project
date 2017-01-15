using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
class SleepingBarberProblem
{
    static Semaphore customerChair = new Semaphore(1, 1);
    static Semaphore barberChair = new Semaphore(1, 1);
    static Semaphore sleepingBarber = new Semaphore(0, 1);
    static Semaphore busyBarber = new Semaphore(1, 1);

    static bool someOneThereInWaitingRoom = false;
    static object tempObject = new object();
    static Random Rand = new Random();

    static bool isCommpleted = false;
    /// <summary>
    /// Define functionality for customer thread
    /// </summary>
    /// <param name="index"></param>
    static void DoWorkCustomer(object index)
    {
        Thread.Sleep(Rand.Next(1, 6) * 1000);
        Console.WriteLine("Customer:{0} has arrived.", (int)index);

        lock (tempObject)
        {
            if (someOneThereInWaitingRoom)
            {
                Console.WriteLine("Customer:{0} leaves the shop as no room to wait.", (int)index);
                return;
            }
        }

        customerChair.WaitOne();
        someOneThereInWaitingRoom = true;
        Console.WriteLine("Customer:{0} has entered waiting room", (int)index);
        barberChair.WaitOne();
        customerChair.Release();
        Console.WriteLine("Hello Barber, customer:{0} wants to wake you up!", (int)index);
        sleepingBarber.Release();
        Console.WriteLine("Customer:{0} is getting the hair cut", (int)index);
        busyBarber.WaitOne();
        someOneThereInWaitingRoom = false;
        // Console.WriteLine("Customer:{0} leaves the barber shop.", (int)index);
    }

    /// <summary>
    /// Define functionality for Barber thread
    /// </summary>
    static void DoWorkBarber()
    {
        while (!isCommpleted)
        {
            Console.WriteLine("Barber is sleeping.");
            sleepingBarber.WaitOne();
            if (!isCommpleted)
            {
                Console.WriteLine("Barber is busy with cutting the hair.");
                Thread.Sleep(Rand.Next(1, 5) * 1000);
                Console.WriteLine("Barber has finished cutting hair.");
                busyBarber.Release();
                barberChair.Release();

            }
            else
            {
                Console.WriteLine("Finished!");
            }
        }
    }

    /// <summary>
    /// Main
    /// </summary>
    static void Main()
    {
        Thread BarberThread = new Thread(DoWorkBarber);
        BarberThread.Start();
        int customersCount = Rand.Next(20, 30);
        Thread[] CustomerThreads = new Thread[customersCount];
        for (int i = 0; i < customersCount; i++)
        {
            CustomerThreads[i] = new Thread(new ParameterizedThreadStart(DoWorkCustomer));
            CustomerThreads[i].Start(i);
            Thread.Sleep(Rand.Next(1, 3) * 1000);
        }

        for (int i = 0; i < customersCount; i++)
        {
            CustomerThreads[i].Join();
        }
        isCommpleted = true;
        sleepingBarber.Release();

        BarberThread.Join();
        Console.WriteLine("No more customers for today, Finished!");
        Console.ReadLine();
        Console.ReadKey();
    }
}