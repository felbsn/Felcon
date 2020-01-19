using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Felcon.Core
{

    public class FClient : PipeBase
    {
        public string ServerProcessName    = "ArasPlmConnector";
        public string ServerRegeditPath    = @"Software\ArasPlmConnector";
        public string ServerRegeditPathKey = "executable_path";

        protected NamedPipeClientStream clientPipeStream;

        public FClient(string address, string ServerProcessName, string ServerRegeditPath, string ServerRegeditPathKey) : base(address)
        {
            this.ServerProcessName = ServerProcessName;
            this.ServerRegeditPath = ServerRegeditPath;
            this.ServerRegeditPathKey = ServerRegeditPathKey;
        }
        public void Initialize()
        {
            PipeSecurity ps = new PipeSecurity();
            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
            ps.AddAccessRule(par);
            clientPipeStream = new NamedPipeClientStream(".", PipeAddress, PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Anonymous);
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
                Console.WriteLine(e.Message + " Unable to connect server");
                Console.WriteLine("CAN NOT CONNECT ! " + e.Message + " " + e.StackTrace);
                return false;
            }
        }

        public void SendMessage(string action, string payload)
        {
            if (EnsureConnection())
            {
                base.message(action, payload);
            }
        }
        public Response SendRequest(string action, string payload)
        {
            if (EnsureConnection())
            {
                return base.request(action, payload);
            }
            else
            {
                return Response.Empty;
            }
        }
        public Task<Response> SendRequestAsync(string action, string payload)
        {
            if (EnsureConnection())
            {
                return base.requestAsync(action, payload);
            }
            else
            {
                return Task.Run(() => Response.Empty);
            }
        }

        // start contiounus service
        public void Start()
        {
            if(!IsConnected)
            Task.Run(async () =>
            {

                while(! IsConnected)
                {
                    await Task.Delay(100);
                    var b = Connect(-1);
                    Console.WriteLine($"Connect respond fr { b}");

                }
            });

            Disconnected +=  (s , e) =>
            {
                while (!IsConnected)
                {
                   
                    var b = Connect(-1);
                    Console.WriteLine($"Connect respond dc { b}");
                }
            };
        }

        // Connection checks
        public bool EnsureConnection()
        {
            if (!IsConnected)
            {
                if (CheckServerApplication())
                {
                    return Connect(2000);
                }
                else
                if(StartServerApplication())
                {
                    return Connect(2000); 
                }else
                {
                    // cant start application
                    return false;
                }
            }
            else
                return true;
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
