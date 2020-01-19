﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Felcon.Utils;

namespace Felcon.Core
{
    public class FServerService
    {
        public event Action<int, FServer> ClientConnected;
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
        public FServer GetInstance(int id) => servers.TryGetValue(id, out var fServer) ? fServer : null;

        public IEnumerable<FServer> GetInstances(string Tag) => servers.Values.Where(s => s.Tag == Tag);

        public IEnumerable<FServer> GetAllInstances() => servers.Values;
        //public FServer GetInstanceWhere(int id) => servers.TryGetValue(id, out var fServer) ? fServer : null;

        public void Register(string regPath, string regKey, string executable_path)
        {

            Util.RegisterRegistry(regPath, regKey, executable_path);
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
            fserver.Tag = "server" + m_serverCounter;
            var currentId = GetID();


            fserver.Connected += (s, e) =>
            {
                // create new pipes ondemand

                CreateConnection();

                ((FServer)s).requestTagAsync().ContinueWith(t=>
                {
                    servers[currentId] = (FServer)s;


                    ClientConnected?.Invoke(currentId, (FServer)s);

                });


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
