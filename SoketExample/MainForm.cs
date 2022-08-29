using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoketExample
{
    public partial class MainForm : Form
    {
        private readonly string connectType;
        TcpListener server;
        TcpClient client;

        StreamReader reader;
        StreamWriter writer;
        NetworkStream stream;

        Thread receiveThread;
        bool isConnect;

        private delegate void AddTextDelegate(string text);

        public MainForm(string connectType)
        {
            InitializeComponent();
            this.connectType = connectType;
        }

        private void Listen()
        {
            try
            {
                AddTextDelegate addText = new AddTextDelegate(txtMessageBox.AppendText);

                IPAddress addr = IPAddress.Parse("192.168.0.123");
                int port = 8080;
                server = new TcpListener(addr, port);
                server.Start();

                Invoke(addText, "Server Start!\r\n");

                client = server.AcceptTcpClient();
                isConnect = true;

                Invoke(addText, "Connected to Client\r\n");

                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.Start();
            }
            catch(Exception ex)
            {
                txtMessageBox.AppendText(ex.Message);
            }
        }
        private void Receive()
        {
            AddTextDelegate addText = new AddTextDelegate(txtMessageBox.AppendText);
            while(isConnect)
            {
                if(stream.CanRead)
                {
                    string receiveChat = reader.ReadLine();
                    if(string.IsNullOrEmpty(receiveChat) == false)
                    {
                        Invoke(addText, $"You : {this.GetHashCode() + receiveChat}\r\n");
                    }
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.connectType == "Server")
            {
                Thread ListenThread = new Thread(new ThreadStart(Listen));
                ListenThread.Start();
            }
            else
            {
                IPAddress addr = IPAddress.Parse("192.168.0.123");
                int port = 8080;

                client = new TcpClient();
                client.Connect(addr, port);

                stream = client.GetStream();
                isConnect = true;

                txtMessageBox.AppendText("Connected to Server" + "\r\n");
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.Start();
            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            txtMessageBox.AppendText($"Me : {this.GetHashCode() + txtInput.Text}\r\n");
            
            writer.WriteLine(txtInput.Text);
            writer.Flush();
            
            txtInput.Clear();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isConnect = false;
            if (reader != null) reader.Close();
            if (writer != null) writer.Close();
            if (server != null) server.Stop();
            if (client != null) client.Close();
            if (receiveThread != null) receiveThread.Abort();
        }

    }
}
