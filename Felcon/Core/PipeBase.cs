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
 

        // private-protected fields
        private Response lastResponse;
        private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);
        protected PipeStream pipeStream;


        // low level send event
        protected void send(string action, string payload, Definitions.Tokens token)
        {
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

        // high level virtual functions , which expects return value
        public virtual void SendMessage(string action, string payload)
        {
            send(action, payload, Tokens.Message);
        }
        public virtual Response SendRequest(string action, string payload)
        {
            waitHandle.Reset();
            send(action, payload, Tokens.Request);

            waitHandle.WaitOne();
            var response = lastResponse;
            waitHandle.Reset();

            return response;
        }
        public virtual Task<Response> SendRequestAsync(string action, string payload) => Task.Run(() => SendRequest(action, payload));

        // tag things

        public virtual string RequestTag()
        {
            EventWaitHandle handle = new AutoResetEvent(false);

             TagReceived += (s, e) =>
             {
                 handle.Set();
             };

            send("reqTag", "somethng", Tokens.RequestTag);

             var res = handle.WaitOne(20_100);
             if (!res) Console.WriteLine("Request tag failed!");
             
             foreach (Delegate d in TagReceived.GetInvocationList())
             {
                TagReceived -= (EventHandler<EventArgs>)d;
             }
             
            return res ? tag : null;
        }
                
           

   


        public void Close()
        {
            if (pipeStream != null)
            {
                if(pipeStream.IsConnected)
                {
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
            Listen(); 
            Connected?.Invoke(this, EventArgs.Empty);
            //ListenOnce(args =>
            //{
            //    Console.WriteLine("some message " + args);
            //}).ContinueWith(t =>
            //{
            //    
            //});
        }
            

        private Task Listen()
        {
            if (IsListening == false)
            {
                IsListening = true;
                return Task.Run(() =>
                {
                    while (pipeStream != null && pipeStream.IsConnected)
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

                                        TagReceived?.Invoke(this , EventArgs.Empty);
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
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception in listener!:" + ex.Message);

                            Close();
                        }
                    }

                    lastResponse = Response.Empty;
                    waitHandle.Set();
                    Disconnected?.Invoke(this, null);
                });
            }
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
