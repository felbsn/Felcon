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
            FullPipeName,
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

         //public override Response SendRequest(string action, string xml)
         //{
         //    EnsureConnection();
         //    return base.SendRequest(action, xml);
         //}
         //
         //public bool EnsureConnection()
         //{
         //    var res = OpenClientCallBack?.Invoke(PipeAddress);
         //    return res.HasValue ? res.Value : false;
         //}


    }
}
