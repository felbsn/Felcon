using Felcon.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

            slavePipe.Tag = Path.GetRandomFileName();

            var pipe = slavePipe;
            pipe.DataReceived += (s, e) =>
            {
                WriteConsole("Receive", "Server", e.action, e.payload);


                if (e.method == Felcon.Definitions.Tokens.Request)
                {
                    if(e.action == "identification")
                    {
                        e.response.action = "x";
                        e.response.payload = "CADACADACADACAD";
                        WriteConsole("SEND ID", "Server", e.action, e.payload);
                    }
                    else
                    {

                        e.response.action = "M";
                        e.response.payload = "ben de bir clientim";

                        WriteConsole("Automatic Response", "Server", e.response.action, e.response.payload);
                    }

                 
                }
            };

            pipe.Connected += (s, e) =>
            {
                WriteConsole("Connected", "evet");



            };

            pipe.Disconnected += (s, e) =>
            {
                WriteConsole("Disconnect", "oyle");
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

                    slavePipe.ServerProcessName = "ServerServiceExample";
                    slavePipe.ServerRegeditPath = @"Software\testv0";
                    slavePipe.ServerRegeditPathKey = "path";

                    slavePipe.Connect(100);
 
                    btn.Text = "Disconnect";
                    btn.ForeColor = Color.Red;
                }

            });
 
        }

        public void WriteConsole(string eventName, string eventInfo)
        {
            consoleTextBox.Invoke(console =>
            {
                console.Text += $"[{DateTime.Now.ToLongTimeString()}]({eventName})-> {eventInfo}\r\n";
                console.SelectionStart = console.Text.Length;
                console.ScrollToCaret();
            });
        }
        public void WriteConsole(string eventName, string targetName, string action, string payload)
        {
            consoleTextBox.Invoke(console =>
            {

                console.Text += $"[{DateTime.Now.ToLongTimeString()}]({eventName}::{targetName})-> act:{action}, payload:{payload}\r\n";
                console.SelectionStart = console.Text.Length;
                console.ScrollToCaret();
            });
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            WriteConsole("Send", "Server", actionTextBox.Text, payloadTextBox.Text);
            slavePipe.SendMessage(actionTextBox.Text, payloadTextBox.Text);
        }

        private void requestButton_Click(object sender, EventArgs e)
        {
            WriteConsole("Request", "Server", actionTextBox.Text, payloadTextBox.Text);
            slavePipe.SendRequestAsync(actionTextBox.Text, payloadTextBox.Text).ContinueWith(
                t =>
                {
                    WriteConsole("Response", "Server", t.Result.action, t.Result.payload);
                });
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
