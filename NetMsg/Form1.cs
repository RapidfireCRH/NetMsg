using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetMsg
{
    public partial class Form1 : Form
    {
        public IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8123);
        public static string[] disp = new string[12];
        static bool changed = false;
        UdpClient client = new UdpClient(8123);
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            client.Close();
            try
            {
                remoteIPEndPoint = new IPEndPoint(IPAddress.Parse(textBox1.Text), 8123);
            }
            catch
            {
                remoteIPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8123);
                textBox1.Text = "192.168.1.1";
            }
            client = new UdpClient(8123);
            backgroundWorker1.RunWorkerAsync();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerSupportsCancellation = true;
            textBox1.Text = "192.168.1.1";
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Send(Encoding.ASCII.GetBytes(textBox3.Text), Encoding.ASCII.GetBytes(textBox3.Text).Length, remoteIPEndPoint);
            display("Local: " + textBox3.Text);
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
            while (!backgroundWorker1.CancellationPending)
            {
                Byte[] receivedBytes = client.Receive(ref remoteIPEndPoint);
                if (receivedBytes == null || receivedBytes.Length == 0)
                    return;
                display("Remote: " + Encoding.ASCII.GetString(receivedBytes));
            }
            backgroundWorker1.Dispose();
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
    }
}
