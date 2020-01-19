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

using u = UniLog.UniLog;

namespace Felcon.Core
{
    public abstract class ConnectionBase
    {
        // events
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> DataReceived;
        public event EventHandler<EventArgs> Connected;

        // public fields
        public string PipeAddress;
        public bool IsConnected { get => pipeStream == null ? false : pipeStream.IsConnected; }
        public string FullPipeName { get => PipeAddress; }
        public bool IsListening { get; protected set; }

        private string tag;
        public string Tag { get => tag; set => tag = value; }

        public string Name;

        // private-protected fields
        private Response lastResponse;
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        
        protected PipeStream pipeStream;
  

        // low level send event
        protected void send(string action, string payload, Definitions.Tokens token)
        {


            u.log($"[SEND] Name:{Name} TOKEN:{token}");

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
            u.log($"[REQUEST START] Name:{Name} {token}");
            if (!waitHandle.WaitOne())
            {
                u.error($" ANOTHER WAIT { token}");
                waitHandle.Set();
                return;
            }
            u.log($"[REQUEST RESET HANDLE] Name:{Name}");
            waitHandle.Reset();


            var bActionPair = action.ToLengthValuePair();
            var bPayloadPair = payload.ToLengthValuePair();

            var bMsgLen = BitConverter.GetBytes((Int32)(bActionPair.Length + bPayloadPair.Length));
            var bToken = BitConverter.GetBytes((Int32)token);

            var bMessage = Util.Merge(bMsgLen, bToken, bActionPair, bPayloadPair);


            u.log($"[REQUEST WRITE] Name:{Name} TOKEN:{token}");
            try
            {
                pipeStream.Write(bMessage, 0, bMessage.Length);

            }
            catch (Exception ex)
            {
                _ = ex;
                u.error($"[REQUEST WRITE EXCEPTION] Name:{Name} TOKEN:{token}");
            }

            if (waitHandle.WaitOne())
            {
                u.log($"[REQUEST DONE] Name:{Name} \r\n\r\n");

            }
            else
            {
                u.error($"[REQUEST WAIT FAIL] Name:{Name}");
            }
        }

        // high level virtual functions , which expects return value
        public virtual void SendMessage(string action, string payload)
        {
            send(action, payload, Tokens.Message);
        }
        public virtual Response SendRequest(string action, string payload)
        {
            request(action, payload, Tokens.Request);
            var response = lastResponse;
            return response;
        }
        public virtual Task<Response> SendRequestAsync(string action, string payload) => Task.Run(() => SendRequest(action, payload));

        // tag things

        public virtual string RequestTag()
        {

            request($"{Name}", "reqTag", Tokens.RequestTag);
            return tag;
        }
        public void Close()
        {
            if (pipeStream != null)
            {
                waitHandle.Set();
                if (pipeStream.IsConnected)
                {
                    u.log($"[CLOSING] Name:{Name}");
                    pipeStream.WaitForPipeDrain();
                }

                pipeStream.Close();
                pipeStream.Dispose();
                pipeStream = null;
            }
        }
        protected ConnectionBase(string pipeAddress)
        {
            PipeAddress = pipeAddress;
        }
        protected void OnConnect()
        {
            Listen();
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private Task Listen()
        {
            if (IsListening == false)
            {
                u.success($"[LISTEN START] Name:{Name}");
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

                            u.log($"[LISTEN BEGIN] Name:{Name} msg:{messageToken}");
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

                                        u.log($"[HANDLE SET] Name:{Name} msg:{messageToken}");
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

                                        u.log($"[HANDLE SET] Name:{Name} msg:{messageToken}");
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
                                    u.log("Unsupported protocol! token:" + messageToken);
                                    break;
                            }



                            u.log($"[LISTEN END] Name:{Name} msg:{messageToken}");
                        }
                        catch (Exception ex)
                        {
                            IsListening = false;
                            u.log("Exception in listener!:" + ex.Message);
                            waitHandle.Set();
                            Close();
                        }
                    }

                    IsListening = false;
                    u.log($"[LISTEN END] Name:{Name}");
                    lastResponse = Response.Empty;
                    waitHandle.Set();
                    Disconnected?.Invoke(this, null);
                });
            }
            u.error($"[LISTEN FAIL] Name:{Name}");
            return null;
        }
    }
}
