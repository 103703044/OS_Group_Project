﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleepBarber5._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new MainForm(Decimal.ToInt32(numeric_barber.Value), Decimal.ToInt32(numeric_customer.Value),
            Decimal.ToInt32(numericUpDown1.Value), Decimal.ToInt32(numericUpDown2.Value)).Show();
            this.Hide();
        }
    }
}
