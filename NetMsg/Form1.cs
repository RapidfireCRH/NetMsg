using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NetMsg
{
    public partial class Form1 : Form
    {
        IPAddress ip;
        _net net = new _net();
        public static string[] disp = new string[10];
        static bool changed = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ip = IPAddress.Parse(textBox1.Text);
            }
            catch
            {
                ip = IPAddress.Parse("192.168.1.1");
                textBox1.Text = "192.168.1.1";
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            backgroundWorker1.WorkerSupportsCancellation = true;
            textBox1.Text = "192.168.1.1";
            timer1.Enabled = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!net.send(ip, textBox3.Text))
                ;
            display("->" + ip.ToString() + ": " + textBox3.Text);
            textBox3.Text = "";
        }
        static void display(string nextline)
        {
            int place = 0;
            for (; place != disp.Length; place++)
            {
                if (disp[place] == null)
                {
                    disp[place] = nextline;
                    changed = true;
                    return;
                }
            }
            if (disp[--place] != nextline)
            {
                for (int i = 1; i != disp.Length; i++)
                    disp[i - 1] = disp[i];
                disp[place] = nextline;
            }
            changed = true;
        }
        public void draw()
        {
            foreach (string x in disp)
                textBox2.Text += x + Environment.NewLine;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                _net._rec temp = net.receive();
                if (temp.ip == null && temp.message.Length > 0)
                {
                    log("Error: " + net.error, log_level.Error);
                    continue;
                }
                else if (temp.ip == null)
                    continue;
                else
                    display("<-" + temp.ip + ": " + temp.message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!changed)
                return;
            textBox2.Text = "";
            foreach (string x in disp)
                textBox2.Text += x + Environment.NewLine;
            changed = false;
        }

        enum log_level { Error, Log}
        void log(string message, log_level msglvl)
        {
            switch(msglvl)
            {
                case log_level.Error:
                    File.AppendAllText("Error.log", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + message);
                    break;
                case log_level.Log:
                    File.AppendAllText("Info.log", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + message);
                    break;
            }
        }
    }
}
