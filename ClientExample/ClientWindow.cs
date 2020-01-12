using Felcon.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientExample
{
    public partial class ClientWindow : Form
    {
        FClient slavePipe;
        public ClientWindow()
        {
            InitializeComponent();
            slavePipe = new FClient("testAddr");

            var pipe = slavePipe;
            pipe.DataReceived += (s, e) =>
            {
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
                $"[{DateTime.Now.ToLongTimeString()}](INCOMING)-> act:{e.action}, payload:{e.payload}\r\n");
            };

            pipe.Connected += (s, e) =>
            {
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
                $"[{DateTime.Now.ToLongTimeString()}](EVENT)-> Connected\r\n");

                
            };

            pipe.Disconnected += (s, e) =>
            {
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
              $"[{DateTime.Now.ToLongTimeString()}](EVENT)-> Disconnected\r\n");
            };
        }

        private void connectButtton_Click(object sender, EventArgs e)
        {
            var btn = ((Button)sender);
            btn.Invoke((b) =>
            {
                if (slavePipe.IsConnected)
                {
                    slavePipe.Close();

                    btn.Text = "Connect";
                    btn.ForeColor = Color.Black;
                }
                else
                {
                    var txt = connectionTextBox.Text;
                    slavePipe.PipeAddress = txt;
                    slavePipe.Initialize();
                    slavePipe.Connect(1000);
 
                    btn.Text = "Disconnect";
                    btn.ForeColor = Color.Red;
                }

            });
 
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
            $"[{DateTime.Now.ToLongTimeString()}](OUTGOING)-> act:{actionTextBox.Text}, payload:{payloadTextBox.Text}\r\n");
            slavePipe.send(actionTextBox.Text, payloadTextBox.Text);
        }
    }


    public static class Extensions
    {
        public static void Invoke<TControlType>(this TControlType control, Action<TControlType> del)
            where TControlType : Control
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => del(control)));
            else
                del(control);
        }
    }
}
