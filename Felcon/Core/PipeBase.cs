using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Felcon.Extensions;
using Felcon.Utils;
using Felcon.Definitions;

namespace Felcon.Core
{

    public abstract class PipeBase
    {


        // events
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> DataReceived;
        public event EventHandler<EventArgs> Connected;

        private event EventHandler<EventArgs> TagReceived;

        // public fields
        public string PipeAddress;
        public bool IsConnected { get => pipeStream == null ? false : pipeStream.IsConnected; }
        public string FullPipeName { get => PipeAddress; }
        public bool IsListening { get; protected set; }

        public bool IsMaster = true;

        private string tag;
        public string Tag
        {
            get => tag;
            set => tag = value;

            //set
            //{
            //    if (IsConnected) 
            //        send("setTag", value, Tokens.SetTag);
            //    tag = value;
            //}
        }

        public string Name;
        public string IsServer = " SERVER::";
        // private-protected fields
        private Response lastResponse;
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        protected PipeStream pipeStream;


        // low level send event
        protected void send(string action, string payload, Definitions.Tokens token)
        {


            Console.WriteLine($"{IsServer}[SEND] Name:{Name} TOKEN:{token}");

            var bActionPair = action.ToLengthValuePair();
            var bPayloadPair = payload.ToLengthValuePair();

            var bMsgLen = BitConverter.GetBytes((Int32)(bActionPair.Length + bPayloadPair.Length));
            var bToken = BitConverter.GetBytes((Int32)token);
            var bMessage = Util.Merge(bMsgLen, bToken, bActionPair, bPayloadPair);

            try
            {
                pipeStream.Write(bMessage, 0, bMessage.Length);

            }
            catch (Exception ex)
            {
                _ = ex;
            }    
        }

        protected void request(string action, string payload, Definitions.Tokens token)
        {
            Console.WriteLine($"{IsServer}[REQUEST START] Name:{Name}");
            if (!waitHandle.WaitOne())
            {
                throw new Exception($"{IsServer} ANOTHER WAIT");
            }
            Console.WriteLine($"{IsServer}[REQUEST RESET HANDLE] Name:{Name}");
            waitHandle.Reset();
          

            var bActionPair = action.ToLengthValuePair();
            var bPayloadPair = payload.ToLengthValuePair();

            var bMsgLen = BitConverter.GetBytes((Int32)(bActionPair.Length + bPayloadPair.Length));
            var bToken = BitConverter.GetBytes((Int32)token);

            var bMessage = Util.Merge(bMsgLen, bToken, bActionPair, bPayloadPair);


            Console.WriteLine($"{IsServer}[REQUEST WRITE] Name:{Name} TOKEN:{token}");
            try
            {
                pipeStream.Write(bMessage, 0, bMessage.Length);

            }
            catch (Exception ex)
            {
                _ = ex;
            }

            if (waitHandle.WaitOne())
            {
                Console.WriteLine($"{IsServer}[REQUEST DONE] Name:{Name} \r\n\r\n");
               
            }
            else
            {
                Console.WriteLine($"[REQUEST WAIT FAIL] Name:{Name}");
            }
        }

        // high level virtual functions , which expects return value
        public virtual void SendMessage(string action, string payload)
        {
            send(action, payload, Tokens.Message);
        }
        public virtual Response SendRequest(string action, string payload)
        {
            //if (waitHandle.WaitOne(200))
            //{ 
            //    Console.WriteLine($"[FAILED ANOTHER CALL] Name:{Name}");
            //    Close();
            //    return default;
            //}

            request(action, payload, Tokens.Request);
            var response = lastResponse;
            return response;

            //waitHandle.Reset();
            //send(action, payload, Tokens.Request);
            //
            //waitHandle.WaitOne();
            //var response = lastResponse;
            //return response;
        }
        public virtual Task<Response> SendRequestAsync(string action, string payload) => Task.Run(() => SendRequest(action, payload));

        // tag things

        public virtual string RequestTag()
        {

            request($"{IsServer}{Name}", "reqTag", Tokens.RequestTag);
            
            //if (!waitHandle.WaitOne(200))
            //{
            //    Console.WriteLine($"{IsServer}[FAILED ANOTHER CALL] Name:{Name}");
            //    Close();
            //    return default;
            //}
            //
            //waitHandle.Reset();
            //send($"{IsServer}{Name}", "reqTag", Tokens.RequestTag);
            //
            //waitHandle.WaitOne();
            return tag;


            //EventWaitHandle handle = new AutoResetEvent(false);
            //
            //var x = false;
            //EventHandler<EventArgs> handler = (s, e) =>
            //{
            //    Console.WriteLine($"[TAG RECEIVED LAUNCH] Name:{Name}");
            //    x = true;
            //
            //    handle.Set();
            //};
            //
            //TagReceived += handler;


            //Task.run

            //send("reqTag", "somethng", Tokens.RequestTag);



            //var res = handle.WaitOne();
            //if (!res) Console.WriteLine("Request tag failed!");


            //TagReceived -= handler;

            //return res ? tag : null;
            return tag;
        }
                
           

   


        public void Close()
        {
            if (pipeStream != null)
            {
                waitHandle.Set();
                if (pipeStream.IsConnected)
                {
                    Console.WriteLine($"{IsServer}[CLOSING] Name:{Name}");
                    pipeStream.WaitForPipeDrain();
                }
                
                pipeStream.Close();
                pipeStream.Dispose();
                pipeStream = null;
            }
        }
        protected PipeBase(string pipeAddress)
        {
            PipeAddress = pipeAddress;
        }

        protected void OnConnect()
        {
            Console.WriteLine($"{IsServer}[CONNECTED] Name:{Name}");
            Listen();
            Console.WriteLine($"{IsServer}[CONNECTED INVOKE EVENT] Name:{Name}");
            if(IsMaster)
                Connected?.Invoke(this, EventArgs.Empty);
            else
            Task.Run(() =>
            {
                Task.Delay(10).Wait();
                Connected?.Invoke(this, EventArgs.Empty);
            });
 
        }
            

        private Task Listen()
        {
            if (IsListening == false)
            {
                Console.WriteLine($"{IsServer}[LISTEN START] Name:{Name}");
                IsListening = true;
                return Task.Run(() =>
                {
                    while (pipeStream != null && pipeStream.IsConnected)
                    {
                        try
                        {
                            pipeStream.Read(new byte[0], 0, 0);
                            lock(pipeStream)
                            { 

                            var messageLength = pipeStream.ReadI32();
                            var messageMethod = pipeStream.ReadI32();

                            var messageToken = (Tokens)messageMethod;

                            Console.WriteLine($"{IsServer}[LISTEN BEGIN] Name:{Name} msg:{messageToken}");
                            switch (messageToken)
                            {
                                case Tokens.Message:
                                    {
                                        var actionLength = pipeStream.ReadI32();
                                        var action = pipeStream.ReadString(actionLength);
                                        var payloadLength = pipeStream.ReadI32();
                                        var payload = pipeStream.ReadString(payloadLength);
                                        DataReceived?.SafeInvoke(this, new DataEventArgs(action, payload, messageToken));
                                    }
                                    break;
                                case Tokens.Request:
                                    {
                                        var actionLength = pipeStream.ReadI32();
                                        var action = pipeStream.ReadString(actionLength);
                                        var payloadLength = pipeStream.ReadI32();
                                        var payload = pipeStream.ReadString(payloadLength);
                                        var args = new DataEventArgs(action, payload, messageToken);

                                        DataReceived?.SafeInvoke(this, args);
                                        send(args.response.action, args.response.payload, Tokens.Response);
                                    }
                                    break;
                                case Tokens.Response:
                                    {
                                        var actionLength = pipeStream.ReadI32();
                                        var action = pipeStream.ReadString(actionLength);
                                        var payloadLength = pipeStream.ReadI32();
                                        var payload = pipeStream.ReadString(payloadLength);
                                        var args = new DataEventArgs(action, payload, messageToken);
                                        lastResponse = new Response(action, payload);

                                        Console.WriteLine($"{IsServer}[HANDLE SET] Name:{Name} msg:{messageToken}");
                                        waitHandle.Set();
                                    }
                                    break;
                                case Tokens.SetTag:
                                    {
                                        
                                        var actionLength = pipeStream.ReadI32();
                                        var action = pipeStream.ReadString(actionLength);
                                        var payloadLength = pipeStream.ReadI32();
                                        var payload = pipeStream.ReadString(payloadLength);

                                        tag = payload;

                                        Console.WriteLine($"{IsServer}[HANDLE SET] Name:{Name} msg:{messageToken}");
                                        waitHandle.Set();

                                        //TagReceived?.SafeInvoke(this , EventArgs.Empty);
                                    }
                                    break;
                                case Tokens.RequestTag:
                                    {
                                        var actionLength = pipeStream.ReadI32();
                                        var action = pipeStream.ReadString(actionLength);
                                        var payloadLength = pipeStream.ReadI32();
                                        var payload = pipeStream.ReadString(payloadLength);
                                        send("setTag", tag, Tokens.SetTag);
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Unsupported protocol! token:" + messageToken);
                                    break;
                            }

                            
                       
                            Console.WriteLine($"{IsServer}[LISTEN END] Name:{Name} msg:{messageToken}");
                            }
                        }
                        catch (Exception ex)
                        {
                            IsListening = false;
                            Console.WriteLine("Exception in listener!:" + ex.Message);
                            waitHandle.Set();
                            Close();
                        }
                    }

                    IsListening = false;
                    Console.WriteLine($"{IsServer}[LISTEN END] Name:{Name}");
                    lastResponse = Response.Empty;
                    waitHandle.Set();
                    Disconnected?.Invoke(this, null);
                });
            }
            Console.WriteLine($"{IsServer}[LISTEN FAIL] Name:{Name}");
            return null;
        }
        private Task ListenOnce(Action<DataEventArgs> func)
        {
            IsListening = true;

            return Task.Run(() =>
            {
                if (pipeStream != null && pipeStream.IsConnected)
                {
                    try
                    {
                        var messageLength = pipeStream.ReadI32();
                        var messageMethod = pipeStream.ReadI32();

                        var messageToken = (Tokens)messageMethod;
                        switch (messageToken)
                        {
                            case Tokens.Message:
                                {
                                    var actionLength = pipeStream.ReadI32();
                                    var action = pipeStream.ReadString(actionLength);
                                    var payloadLength = pipeStream.ReadI32();
                                    var payload = pipeStream.ReadString(payloadLength);

                                    IsListening = false;
                                    func.Invoke(new DataEventArgs(action, payload, messageToken));
                                }
                                break;
                            case Tokens.Request:
                                {
                                    var actionLength = pipeStream.ReadI32();
                                    var action = pipeStream.ReadString(actionLength);
                                    var payloadLength = pipeStream.ReadI32();
                                    var payload = pipeStream.ReadString(payloadLength);
                                    var args = new DataEventArgs(action, payload, messageToken);

                                    IsListening = false;
                                    func.Invoke(new DataEventArgs(action, payload, messageToken));
                                    send(args.response.action, args.response.payload, Tokens.Response);
                                }
                                break;
                            case Tokens.Response:
                                {
                                    var actionLength = pipeStream.ReadI32();
                                    var action = pipeStream.ReadString(actionLength);
                                    var payloadLength = pipeStream.ReadI32();
                                    var payload = pipeStream.ReadString(payloadLength);

                                    var args = new DataEventArgs(action, payload, messageToken);
                                    lastResponse = new Response(action, payload);
                                    func.Invoke(args);
                                    waitHandle.Set();
                                }
                                break;
                            default:
                                Console.WriteLine("Unsupported protocol! token:" + messageToken);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception in listener!:" + ex.Message);

                        Close();
                    }
                }
                // clear state...
                lastResponse = Response.Empty;
                waitHandle.Set();

                if (!IsConnected)
                    Disconnected?.Invoke(this, null);
            });
        }
    }
}
