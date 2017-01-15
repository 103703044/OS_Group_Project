using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SleepBarber5._1
{
    public partial class Form2 : Form {
        int BARBERNUM, CHAIRNUM, CUSTOMERNUM, SPEED;
        Random random = new Random();
        int[] customerList;
        System.Windows.Forms.Timer timer;

        Semaphore numEmptyChairs;
        int numEmptyChairsCount;
        Semaphore numEmptyBarbers;
        bool[] barberIsAwake;
        bool[] barberIsEmpty;
        bool[] chairIsEmpty;
        Semaphore[] isCutting;
        Semaphore[] doneCutting;
        Thread[] barberTid;
        Thread[] customerTid;

        public Form2(int BarberN, int WChairN, int CustomerN, int speed)  {
            BARBERNUM = 2;
            CHAIRNUM = 3;
            CUSTOMERNUM = 10;
            SPEED = 5;
            customerList = new int[CUSTOMERNUM];
            InitializeComponent();
            SetCustomer();
            setTimer();
        }

        private void SetCustomer() {
            for (int i = 0; i < CUSTOMERNUM; i++)  {
                customerList[i] = i;
            }
        }

        private void setTimer()  {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5;
            timer.Start();
        }

        public void Main() {
            barberTid = new Thread[BARBERNUM];
            customerTid = new Thread[CUSTOMERNUM];

            numEmptyChairs = new Semaphore(CHAIRNUM, CHAIRNUM);
            numEmptyChairsCount = CHAIRNUM;
            numEmptyBarbers = new Semaphore(BARBERNUM, BARBERNUM);
            barberIsEmpty = new bool[BARBERNUM];
            barberIsAwake = new bool[BARBERNUM];
            chairIsEmpty = new bool[CHAIRNUM];
            isCutting = new Semaphore[BARBERNUM];
            doneCutting = new Semaphore[BARBERNUM];

            for (int i = 0; i < CHAIRNUM; i++) {
                chairIsEmpty[i] = true;
            }
            for (int i = 0; i < BARBERNUM; i++)   {
                barberTid[i] = new Thread(new ParameterizedThreadStart(barber));
                barberIsEmpty[i] = true;
                barberIsAwake[i] = false;
                isCutting[i] = new Semaphore(0, 1);
                doneCutting[i] = new Semaphore(0, 1);
                barberTid[i].Start(i);
            }
            timer.Tick += customerCome;
        }

        int timerCount = 0;
        private void customerCome(object sender, EventArgs e) {
            timerCount++;
            if (timerCount % 50 != 1) return;
            int count = timerCount / 40;
            if (count >= CUSTOMERNUM)
            {
                ((System.Windows.Forms.Timer)sender).Tick -= customerCome;
                return;
            }
            customerTid[count] = new Thread(new ParameterizedThreadStart(customer));
            customerTid[count].Start(count);
        }

        void barber(object _number) {
            int number = Convert.ToInt32(_number);

            while (true) {
                if (numEmptyChairsCount == CHAIRNUM && barberIsEmpty[number]){
                    MessageBox.Show("The barber " + number + " is sleeping\n");
                    barberIsAwake[number] = false;
                }
                isCutting[number].WaitOne();
                Console.WriteLine("The barber " + number + " is cutting hair\n");
                Thread.Sleep(3000);
                Console.WriteLine("The barber " + number + " has finished cutting hair.\n");
                doneCutting[number].Release(1);

                numEmptyBarbers.Release(1);
                barberIsEmpty[number] = true;
            }
        }

        void customer(object number) {
         
            int num = Convert.ToInt32(number);
            Console.WriteLine("Customer " + num + " arrived at barber shop.\n");
            Thread.Sleep(50);
            if (numEmptyChairsCount != 0){
                numEmptyChairs.WaitOne();
                numEmptyChairsCount--;
                Console.WriteLine("Customer " + num + " entering waiting room.\n");
                int chairNo = getEmptyChair();
                int _time = getRandomTime(80, 5);        
                Thread.Sleep(_time);         //time of goint to waiting chair
                numEmptyBarbers.WaitOne();

                int bnum = getEmptyBarber();
                if (bnum == -1){
                    Console.WriteLine("ERROR!");
                    return;
                }
    
                numEmptyChairs.Release(1);
                numEmptyChairsCount++;
                chairIsEmpty[chairNo] = true;
              //Console.WriteLine("Customer " + num + " walking to the barber " + bnum + ".\n");

                _time = getRandomTime(15, 5);             
                Thread.Sleep(_time);    //time to move to barber

                if (!barberIsAwake[bnum]){
                    Console.WriteLine("Customer " + num + " waking the barber " + bnum + ".\n");
                    barberIsAwake[bnum] = true;
                    Console.WriteLine("barber " + bnum + " is awaked.\n");
                }
                isCutting[bnum].Release(1);
                doneCutting[bnum].WaitOne();
            }
            else Thread.Sleep(500);
           // customer.moveFromAtoB(point_customBorn, 50, timer);
            Console.WriteLine("Customer " + num + " leaving barber shop.\n");
        }

        int getEmptyChair(){
            for (int i = 0; i < CHAIRNUM; i++) if (chairIsEmpty[i]){
                    chairIsEmpty[i] = false;
                    return i;
                }
           // Console.WriteLine("getEmptyChair ERROR!");
           return -1;
        }

       

        private void button1_Click_1(object sender, EventArgs e){
            Main();
        }

        int getEmptyBarber(){
            for (int i = 0; i < BARBERNUM; i++) if (barberIsEmpty[i]){
                    barberIsEmpty[i] = false;
                    return i;
                }
            return -1;
        }
        
        int getRandomTime(int baseTime, int updownTime){
            return SPEED * (int)(random.Next(updownTime) + baseTime);
        }
    }
}
