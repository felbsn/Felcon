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
                
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
                $"[{DateTime.Now.ToLongTimeString()}](INCOMING::{currentPipeNum})-> act:{e.action}, payload:{e.payload}\r\n");
            };

            pipe.Connected += (s, e) =>
            {
 
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
                $"[{DateTime.Now.ToLongTimeString()}](EVENT)-> Connected {{{currentPipeNum}}}\r\n");

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
                consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
              $"[{DateTime.Now.ToLongTimeString()}](EVENT)-> Disconnected {{{currentPipeNum}}}\r\n");

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
                    consoleTextBox.Invoke(consoleTextBox => consoleTextBox.Text +=
                    $"[{DateTime.Now.ToLongTimeString()}](OUTGOING::{selectedItem.Text})-> act:{actionTextBox.Text}, payload:{payloadTextBox.Text}\r\n");



                    var masterPipe = (FServer)selectedItem.Tag;
                    masterPipe.send(actionTextBox.Text, payloadTextBox.Text);
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
