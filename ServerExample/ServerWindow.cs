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

namespace ServerExample
{
    public partial class MainWindow : Form
    {

        List<KeyValuePair<string, FServer>> pipeHolders;
        int pipeCounter = 0;
 
        public MainWindow()
        {
            InitializeComponent();
            pipeHolders = new List<KeyValuePair<string, FServer>>();
 
        }

        public void createPipe()
        {
            var masterPipe = new FServer("testAddr");

            var pipe = masterPipe;

            var currentPipeNum = pipeCounter;

            pipe.DataReceived += (s, e) =>
            {

                WriteConsole("Receive", " client " + currentPipeNum, e.action, e.payload);

                if (e.method == Felcon.Definitions.Tokens.Request)
                {
                    e.response.action = "SA";
                    e.response.payload = "Client " + currentPipeNum+ " kardes";

                    WriteConsole("Automatic Response", " client " + currentPipeNum, e.response.action, e.response.payload);

                }

            };

            pipe.Connected += (s, e) =>
            {

                WriteConsole("Connected", " client " + currentPipeNum);

                clientsListView.Invoke((clientsListView) =>
                {
                    var item = new ListViewItem()
                    {
                        Text = "client " + currentPipeNum,
                        Tag = s
                    };
                    clientsListView.Items.Add(item);
                    pipeCounter++;

                });


                createPipe();
            };

            pipe.Disconnected += (s, e) =>
            {
                WriteConsole("Disconnected", " client "+ currentPipeNum);
 
                clientsListView.Invoke(clientsListView =>
                {
                    foreach (var item in clientsListView.Items.Cast<ListViewItem>().Where(l => l.Tag == s))
                        item.Remove();
                }
                );


            };

            masterPipe.Initialize();
 
        }

        private void startButton_Click(object sender, EventArgs _)
        {
            createPipe();

            var btn = (Button)sender;
            btn.Text = "Listening";
            btn.Enabled = false;
           
        }

        private void sendButton_Click(object sender, EventArgs e)
        {

            
            if(clientsListView.SelectedItems.Count != 0)
            {
                foreach (ListViewItem selectedItem in clientsListView.SelectedItems)
                {


                    WriteConsole("Send", selectedItem.Text, actionTextBox.Text, payloadTextBox.Text);

                    var masterPipe = (FServer)selectedItem.Tag;
                    masterPipe.SendMessage(actionTextBox.Text, payloadTextBox.Text);
                }
            }
            
        }

        private void clientsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(clientsListView.SelectedItems.Count != 0)
            {

            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            startButton_Click(startButton, null);
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


        private void requestButton_Click(object sender, EventArgs e)
        {
            if (clientsListView.SelectedItems.Count != 0)
            {
                foreach (ListViewItem selectedItem in clientsListView.SelectedItems)
                {
       
                    WriteConsole("Request", selectedItem.Text, actionTextBox.Text, payloadTextBox.Text);
                    var masterPipe = (FServer)selectedItem.Tag;
                    masterPipe.SendRequestAsync(actionTextBox.Text, payloadTextBox.Text).ContinueWith(response =>
                    {
                        WriteConsole("Response", selectedItem.Text, response.Result.action, response.Result.payload);
                    });
                }
            }
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
