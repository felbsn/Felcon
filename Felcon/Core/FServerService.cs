using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Felcon.Utils;

namespace Felcon.Core
{
    public class FServerService
    {
        public event Action<int,FServer> ClientConnected;
        public event Action<int, FServer> ClientDisconnected;
        public Dictionary<int, FServer> servers;

        private int m_serverCounter;

        public string ServerAddress { get; protected set; }


        public FServerService(string serverAddress)
        {
            servers = new Dictionary<int, FServer>();
            ServerAddress = serverAddress;
        }

        public int GetID() => m_serverCounter++;
        public FServer GetInstance(int id) => servers.TryGetValue(id , out var fServer) ? fServer : null;
 
        public void Register(string regPath , string regKey , string executable_path )
        {
 
            Util.RegisterRegistry(regPath , regKey , executable_path);
        }
        public void Start()
        {
            CreateConnection();
        }
        public void Start(string serverAddress)
        {
            ServerAddress = serverAddress;
            Start();
        }


        private void CreateConnection()
        {
            var fserver = new FServer(ServerAddress);
            var currentId = GetID();

            servers[currentId] = fserver;

            
            fserver.Connected += (s, e) =>
            {
                // create new pipes ondemand
                CreateConnection();
                ClientConnected?.Invoke(currentId, fserver);
            };
            fserver.Disconnected += (s, e) =>
            {
                ClientDisconnected?.Invoke(currentId, fserver);
                servers.Remove(currentId);
            };

            fserver.Initialize();
        }
    }
}
