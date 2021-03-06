﻿using System;
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
    public class DataEventArgs : EventArgs
    {
        public readonly string action;
        public readonly string payload;

        public readonly Definitions.Tokens method;
        public Response response;

        public DataEventArgs(string action, string payload , Definitions.Tokens method)
        {
            this.method = method;
            response = Response.Empty;
            this.action = action;
            this.payload = payload;
        }
        public override string ToString()
        {
            return $"args act:{action} payload:{payload} method:{method}";
        }
    }






}
