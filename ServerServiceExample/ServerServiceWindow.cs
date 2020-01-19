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

namespace ServerServiceExample
{
    public partial class ServerServiceWindow : Form
    {

        FServerService fServerService;
        public ServerServiceWindow()
        {
            InitializeComponent();
            fServerService = new FServerService(serverAddressTextBox.Text);


            fServerService.ClientConnected += (id, client) =>
            {
                WriteConsole("Client Connect", "id:" + id + " tag:" + client.Tag);



                clientsListView.Invoke(list =>
                {
                    var item = new ListViewItem();
                    item.Text = "Client:" + id +"T:"+ client.Tag;
                    item.Tag = id;
                    list.Items.Add(item);
                });
 
              



                client.DataReceived += (s, d) =>
                {
                    WriteConsole("Data Received" , "ClientID:"+ id , d.action , d.payload);


                    if (d.method ==Felcon.Definitions.Tokens.Request)
                    {
                        d.response.action = "server action!";
                        d.response.payload = "server payload!";

                        WriteConsole("Automatic Response", "ClientID:" + id, d.response.action, d.response.payload);
                    }
                };

                Task.Run(async () =>
                {
                    var thisClient = client;
                    int count = 1;
                    while (thisClient.IsConnected)
                    {
                        thisClient.message("test", "AHAH " + count++);
                        await Task.Delay(300);

                        if (count == 15) break;
                    }
      
                });


 
            };
            fServerService.ClientDisconnected += (id, client) =>
            {
                WriteConsole("Client Disconnect", "id:" + id);

                clientsListView.Invoke(list =>
                {
                    var item = new ListViewItem();
                    item.Text = "Client:" + id;
                    item.Tag = id;
                    var disconItems = list.Items.Cast<ListViewItem>().Where(elm => (int)elm.Tag == id);

                    foreach (var ditem in disconItems)
                        ditem.Remove();
                });
            };
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // fServerService.RegisterRegistry();
           var path =  System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", string.Empty);
            fServerService.Register(@"Software\testv0", "path" , path);
            fServerService.Start(serverAddressTextBox.Text);


            var btn = (Button)sender;
            btn.Text = "Listening";
            btn.Enabled = false;
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
 
                    int id = (int)selectedItem.Tag;
                    var instance = fServerService.GetInstance(id);



                    WriteConsole("Request", selectedItem.Text, actionTextBox.Text, payloadTextBox.Text);
                    instance?.requestAsync(actionTextBox.Text, payloadTextBox.Text).ContinueWith(response =>
                    {
                        WriteConsole("Response", selectedItem.Text, response.Result.action, response.Result.payload);
                    });
                }
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (clientsListView.SelectedItems.Count != 0)
            {
                foreach (ListViewItem selectedItem in clientsListView.SelectedItems)
                {
                    WriteConsole("Send", selectedItem.Text, actionTextBox.Text, payloadTextBox.Text);
                

                    int id = (int)selectedItem.Tag;
                    var instance = fServerService.GetInstance(id);
                    instance?.message(actionTextBox.Text, payloadTextBox.Text);
                }
            }
        }
        //enum Cads
        //{ }
        //void openCAD( string path, Cads cadEnum)
        //{
        //    fServerService.GetInstance(id);
        //}

        private void ServerServiceWindow_Load(object sender, EventArgs e)
        {
            startButton_Click(startButton, null);

           
        }

        private void clientsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clientsListView.SelectedItems.Count != 0)
            {
                foreach (ListViewItem selectedItem in clientsListView.SelectedItems)
                {
                    //WriteConsole("Send", selectedItem.Text, actionTextBox.Text, payloadTextBox.Text);

                    int id = (int)selectedItem.Tag;
                    var instance = fServerService.GetInstance(id);
            
                   // MessageBox.Show("Selected Tag" + instance.Tag);
      
                    //WriteConsole("INSTANCE ID", selectedItem.Text, instance.Tag, payloadTextBox.Text);
                    //instance?.SendMessage(actionTextBox.Text, payloadTextBox.Text);
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
