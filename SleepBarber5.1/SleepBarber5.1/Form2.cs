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
        int BARBERNUM, CHAIRNUM, CUSTOMERNUM;
        Random random = new Random();
        int[] customerList;
        System.Windows.Forms.Timer timer;

        int numEmptyChairsCount;
        Semaphore numEmptyChairs;
        Semaphore numEmptyBarbers;
        bool[] barberIsAwake;
        bool[] barberIsEmpty;
        bool[] chairIsEmpty;
        Semaphore[] isCutting;
        Semaphore[] doneCutting;
        Thread[] barberTid;
        Thread[] customerTid;

        public Form2(int BarberN, int WChairN, int CustomerN)  {
            BARBERNUM = BarberN;
            CHAIRNUM = WChairN;
            CUSTOMERNUM = CustomerN;

            customerList = new int[CUSTOMERNUM];
            InitializeComponent();
            SetCustomer();
            setTimer();
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
                    MessageBox.Show("理髮師" + number + "號在睡覺\n");
                    barberIsAwake[number] = false;
                }
                isCutting[number].WaitOne();
                Console.WriteLine("理髮師" + number + "號開始剪頭髮\n");
                Thread.Sleep(3000);
                Console.WriteLine("理髮師" + number + "號剪完頭髮\n");
                doneCutting[number].Release(1);

                numEmptyBarbers.Release(1);
                barberIsEmpty[number] = true;
            }
        }

        void customer(object number) {
         
            int num = Convert.ToInt32(number);
            Console.WriteLine("客人" + num + "號進入店內\n");
            Thread.Sleep(50);
            if (numEmptyChairsCount != 0){
                numEmptyChairs.WaitOne();
                numEmptyChairsCount--;
                Console.WriteLine("客人" + num + "號坐在椅子上等待\n");
                int chairNo = getEmptyChair();
                int _time = getRandomTime(80, 5);        
                Thread.Sleep(_time);         //time of goint to waiting chair
                numEmptyBarbers.WaitOne();

                int bnum = getEmptyBarber();
                if (bnum == -1){
                    Console.WriteLine("XXXXXXXX");
                    return;
                }
    
                numEmptyChairs.Release(1);
                numEmptyChairsCount++;
                chairIsEmpty[chairNo] = true;           

                _time = getRandomTime(15, 5);             
                Thread.Sleep(_time);    //time to move to barber

                if (!barberIsAwake[bnum]){
                    Console.WriteLine("客人" + num + "號叫醒理髮師" + bnum + "號\n");
                    barberIsAwake[bnum] = true;
                    Console.WriteLine("理髮師" + bnum + "號被叫醒\n");
                }
                isCutting[bnum].Release(1);
                doneCutting[bnum].WaitOne();
            }
            else Thread.Sleep(500);
            Console.WriteLine("客人" + num + "號離開了理髮店\n");
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
            return 5 * (int)(random.Next(updownTime) + baseTime);
        }

        private void SetCustomer()
        {
            for (int i = 0; i < CUSTOMERNUM; i++)
            {
                customerList[i] = i;
            }
        }

        private void setTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5;
            timer.Start();
        }
    }
}
