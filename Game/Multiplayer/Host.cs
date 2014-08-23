using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using InsaneDev.Networking;
using InsaneDev.Networking.Server;
using NerfCorev2.Plugins;

namespace LD30.Multiplayer
{
    class Host : Base
    {
        private static long _NextObjectID = 0;
        private static readonly object _IDLock = new object();
        private static readonly List<ConnectedClient> _ConnectedClients = new List<ConnectedClient>();
        internal Host()
        {
            Init(new IPEndPoint(IPAddress.Any, 3456), typeof(ConnectedClient));
            StartListening();
        }

        public static void SendUpdatedWorldPositions()
        {
            lock (_ConnectedClients)
            {
                int position = 0;
                foreach (ConnectedClient client in _ConnectedClients)
                {
                    client.WorldOffset = position;
                    Packet p = new Packet(Manager.PID_SENDWORLDPOSITION);
                    p.AddInt(position);
                    position++;
                    client.SendPacket(p);
                }
            }
        }

        class ConnectedClient : ClientConnection
        {
            public int WorldOffset = -1;
            public ConnectedClient(TcpClient client)
                : base(client)
            {

            }

            protected override void OnConnect()
            {
                Core.HookModule("Console", "AddStringToConsole", new object[] { "Client Connected" });
                lock (_ConnectedClients) _ConnectedClients.Add(this);
                SendUpdatedWorldPositions();


            }

            protected override void OnDisconnect()
            {
                Core.HookModule("Console", "AddStringToConsole", new object[] { "Client Disconnected" });
                lock (_ConnectedClients) _ConnectedClients.Remove(this);
                SendUpdatedWorldPositions();
            }

            protected override void ClientUpdateLogic()
            {
                if (GetOutStandingProcessingPacketsCount() > 0)
                {
                    foreach (Packet p in GetOutStandingProcessingPackets())
                    {
                        object[] objects = p.GetObjects();
                        switch (p.Type)
                        {
                            case Manager.PID_REQUESTPLAYERDETAILS:
                            case Manager.PID_ANNOUNCEEXISTENCE:
                            case Manager.PID_ANNOUNCEPLAYERDETAILS:
                            case Manager.PID_SENDCHARACTERPHYSICS:
                                Manager._Server.SendToAll(p);
                                break;
                            case Manager.PID_IDREQUEST:
                                long id = -1;
                                lock (_IDLock)
                                {
                                    id = _NextObjectID;
                                    _NextObjectID++;
                                }
                                Packet rp = new Packet(Manager.PID_IDRESPONCE);
                                rp.AddLong(id);
                                SendPacket(rp);
                                break;
                        }
                        p.Dispose();
                    }
                }
            }
        }
    }
}
