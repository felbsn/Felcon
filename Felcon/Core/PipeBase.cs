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

        // private events
        private event EventHandler<ResponseEventArgs> ResponseReceived;

        // public fields
        public string PipeAddress;
        public bool IsConnected { get => pipeStream == null ? false : pipeStream.IsConnected; }
     

        public bool IsListening { get; protected set; }

        public string Tag { get;  set; }

        protected PipeStream pipeStream;
 
        int requestCounter = 1;


        // low level send event (actually lowest...)
        protected void send(byte[] value, Tokens token, int messageID)
        {
            // set header
            int msgLen = value.Length;
            int msgMethod = (int)token;
            int msgID = messageID;
            int msgVer = 0;

            // create message byte stream
            var bMsg = Util.MergeBytes(
                BitConverter.GetBytes(msgLen),
                BitConverter.GetBytes(msgMethod),
                BitConverter.GetBytes(msgID),
                BitConverter.GetBytes(msgVer),
                value);
 
            try
            {
                pipeStream.Write(bMsg, 0, bMsg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in send message token: {token} {ex.Message}  , {ex.StackTrace}");
                Close();
                _ = ex; 
            }
        }
        protected void response(byte[] value, int messageID)
        {
            send(value, Tokens.Response, messageID);
        }

        public void requestTag()
        {
                int reqCount = Interlocked.Increment(ref requestCounter);
                var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                EventHandler<ResponseEventArgs> eventHandler = null;
                Response response = Response.Empty;
                eventHandler = (s, e) =>
                {
                    if (e.messageID == reqCount)
                    {
                        response = e.response;
                        ResponseReceived -= eventHandler;
                        waitHandle.Set();
                    }
                };
                ResponseReceived += eventHandler;

                var bMsg = "tag".ToLengthValuePair().Concat("get".ToLengthValuePair()).ToArray();
                send(bMsg, Tokens.RequestTag, reqCount);
                waitHandle.WaitOne();
                Tag = response.payload;
        }
        public Task requestTagAsync()
        {
            return Task.Run(() => requestTag());
        }


        // high level functions
        public Response request(string action, string payload)
        {
            int reqCount = Interlocked.Increment(ref requestCounter);
            var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            EventHandler<ResponseEventArgs> eventHandler = null;
            Response response = Response.Empty;


            eventHandler = (s, e) =>
            {
                if(e.messageID == reqCount)
                {

                    response = e.response;
                    ResponseReceived -= eventHandler;
                    waitHandle.Set();
                }
            };
            ResponseReceived += eventHandler;
 
            var bMsg = action.ToLengthValuePair().Concat(payload.ToLengthValuePair()).ToArray();
            send(bMsg, Tokens.Request, reqCount);

            waitHandle.WaitOne();

            return response;
        }
        public Task<Response> requestAsync(string action, string payload) => Task.Run(() => request(action, payload));
        public void message(string action, string payload)
        {
            var bMsg = Util.MergeBytes(
                action.ToLengthValuePair(), 
                payload.ToLengthValuePair());

            send(bMsg, Tokens.Message , 0);
        }




        public void Close()
        {
            if (pipeStream != null)
            {
                if (pipeStream.IsConnected)
                {
                    Console.WriteLine($"[CLOSING]");
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
        }


        private Task Listen()
        {
            if (IsListening == false)
            {
                IsListening = true;
                return Task.Run(() =>
                {
                    try
                    {
                        while (IsConnected)
                        {
                            MessageHeader msgHeader =  pipeStream.ReadMessageHeader();

                            var buffer = new byte[msgHeader.messageLength];
                            var cbufferLen = pipeStream.Read(buffer, 0, msgHeader.messageLength);
                            if(cbufferLen == msgHeader.messageLength)
                            {
                                var token = (Tokens)msgHeader.messageMethod;

                                if(msgHeader.messageVersion == 0)
                                {
                                    int actionLen = BitConverter.ToInt32(buffer, 0);
                                    string action = Encoding.ASCII.GetString(buffer, 4, actionLen);
                                    int payloadLen = BitConverter.ToInt32(buffer, actionLen + 4);
                                    string payload = Encoding.ASCII.GetString(buffer, actionLen + 8, payloadLen);

                                    switch (token)
                                    {
                                        case Tokens.Message:
                                            {
                                                var args = new DataEventArgs( action, payload, token);
                                                DataReceived?.Invoke(this, args);
                                            }
                                            break;
                                        case Tokens.Request:
                                            {
                                                var args = new DataEventArgs(action, payload, token);
                                                DataReceived?.Invoke(this, args);

                                                // set response bytes stream
                                                var bytes = Util.MergeBytes(
                                                    args.response.action.ToLengthValuePair(),
                                                    args.response.payload.ToLengthValuePair()
                                                    );

                                                Task.Run(() => response(bytes, msgHeader.messageID));
                                               
                                            }
                                            break;
                                        case Tokens.RequestTag:
                                            {
                                                var bytes = Util.MergeBytes(
                                                    "tag".ToLengthValuePair(),
                                                     Tag.ToLengthValuePair()
                                                    );
                                                response(bytes, msgHeader.messageID);
                                            }
                                            break;
                                        case Tokens.Response:
                                            {
                                                var response = new ResponseEventArgs(msgHeader.messageID, action, payload);
                                                ResponseReceived.Invoke(this, response);
                                            }
                                            break;
                                        default:
                                            Console.WriteLine($"Undefined behaviour token:{token}!");
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Undefined message version! version:{msgHeader.messageVersion}");
                                    break;
                                }

                            }else
                            {
                                Console.WriteLine("Message receive error!");
                                break;
                            }
                        }   

                    }
                    catch (Exception ex)
                    {

                        IsListening = false;
                        Console.WriteLine("Exception in listener!:" + ex.Message);
                        Close();
                    }
                    IsListening = false;
                    Disconnected?.Invoke(this, null);
                });
            }
            IsListening = false;
            return null;
        }
    }


}
