using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using u = UniLog.UniLog;

namespace Felcon.Core
{

    public class FClient : PipeBase
    {

        public string ServerProcessName = "ArasPlmConnector";
        public string ServerRegeditPath = @"Software\ArasPlmConnector";
        public string ServerRegeditPathKey = "executable_path";


        protected NamedPipeClientStream clientPipeStream;


        public FClient(string address) : base(address)
        {

            IsMaster = false;
        }
        public void Initialize()
        {
            PipeSecurity ps = new PipeSecurity();
            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
            ps.AddAccessRule(par);

            clientPipeStream = new NamedPipeClientStream(".", FullPipeName, PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Anonymous);
            pipeStream = clientPipeStream;

         
        }


        public bool Connect(int ms = 100)
        {
            try
            {
                Initialize();
                clientPipeStream.Connect(ms);
                OnConnect();
                return true;
            }
            catch (Exception e)
            {
                u.log(e.Message + " Unable to connect server");
                u.log("CAN NOT CONNECT ! " + e.Message + " " + e.StackTrace);
                return false;
            }
        }

        public Task<bool> Connect(int ms, int delay)
        {
            return Task.Run(() =>
            {
                try
                {
                    Initialize();
                    clientPipeStream.Connect(ms);
                    OnConnect();

                    Task.Delay(delay).Wait();
                    return true;
                }
                catch (Exception e)
                {
                    u.log(e.Message + " Unable to connect server");
                    return false;
                }
            });

        }

        // methods...
        public override void SendMessage(string action, string payload)
        {
            if (EnsureConnection())
            {
                base.SendMessage(action, payload);
            }
        }
        public override Response SendRequest(string action, string payload)
        {
            if (EnsureConnection())
            {
                return base.SendRequest(action, payload);
            }
            else
            {
                return Response.Empty;
            }
        }



        // Connection checks
        public bool EnsureConnection()
        {
            //return Task.Run(() =>
            //{
            if (!IsConnected)
            {
                if (CheckServerApplication())
                {
                    var t = Connect(2000, 100);
                    t.Wait();
                    return t.Result;
                }
                else
                {
                    StartServerApplication();

                    var t = Connect(2000, 100);
                    t.Wait();
                    return t.Result;
                    //return Connect(2000);
                }
            }
            else
                return true;
            //});
        }
        public bool CheckServerApplication()
        {
            Process[] processes = Process.GetProcessesByName(ServerProcessName);

            return processes.Length > 0;
        }
        public bool StartServerApplication()
        {
            Process[] processes = Process.GetProcessesByName(ServerProcessName);

            if (processes.Length == 0)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(ServerRegeditPath, true);
                if (key == null)
                {
                    throw new Exception($"{ServerProcessName} installation could not be found!");

                }
                else
                {
                    var exePath = key.GetValue(ServerRegeditPathKey, "null").ToString();
                    if (exePath != null && File.Exists(exePath))
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo()
                        {
                            FileName = exePath
                        };
                        p.Start();
                    }
                    else
                    {
                        throw new Exception($"{ServerProcessName} not found on specified location \r\n Given path:{exePath}");

                    }
                }
            }

            processes = Process.GetProcessesByName(ServerProcessName);
            return processes.Length > 0;
        }
    }

}
