using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon 
{
    public struct Response
    {
        public string action;
        public string payload;
        public Response(string action, string payload)
        {
            this.action = action;
            this.payload = payload;
        }
        public static readonly Response Empty = new Response("Empty", "");
    }

    public struct MessageHeader
    {
       public Int32 messageLength;
       public Int32 messageMethod;
       public Int32 messageID;
       public Int32 messageVersion;
    }



    public class DataEventArgs : EventArgs
    {
        public readonly string action;
        public readonly string payload;

        public readonly Definitions.Tokens method;


        public Response response;

        public byte[] data;
        public DataEventArgs(byte[] data)
        {
            this.data = data;
        }

        public DataEventArgs(string action, string payload , Definitions.Tokens method)
        {
            this.method = method;
            response = Response.Empty;
            this.action = action;
            this.payload = payload;
        }
        public override string ToString()
        {
            return $"Args act:{action} payload:{payload} method:{method}";
        }
    }



    public class ResponseEventArgs : EventArgs
    {
        public readonly int messageID;
        public readonly Response response;


        public ResponseEventArgs(int messageID , string action, string payload)
        {
            this.messageID = messageID;
            this.response = new Response(action, payload);
        }
        public override string ToString()
        {
            return $"Response act:{response.action} payload:{response.payload} messageID:{messageID}";
        }
    }


}
