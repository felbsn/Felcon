using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon.Core
{
    public class FServer : PipeBase
    {
        protected NamedPipeServerStream serverPipeStream;

        public FServer(string name) : base(name)
        {
        }

        public void Initialize()
        {
            serverPipeStream = new NamedPipeServerStream(
            PipeAddress,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Message,
            PipeOptions.Asynchronous);

            pipeStream = serverPipeStream;
            serverPipeStream.WaitForConnectionAsync().ContinueWith((res) =>
            {
                OnConnect();
            });
        }


        public void SendMessage(string action, string payload)
        {
                base.message(action, payload);
        }
        public Response SendRequest(string action, string payload)
        {
                return base.request(action, payload);
        }
        public Task<Response> SendRequestAsync(string action, string payload)
        {
                return base.requestAsync(action, payload);
        }

    }
}
