using System;
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
        public static DateTime _LastShift = DateTime.Now;
        public static TimeSpan _ShiftInterval = new TimeSpan(0, 0, 0, 1);
        private static long _NextObjectID = 0;
        private static readonly object _IDLock = new object();
        private static readonly List<ConnectedClient> _ConnectedClients = new List<ConnectedClient>();
        private static ConnectedClient _GameHost;
        private static bool _GameHostConnected = false;
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

        public virtual void Update()
        {
            if (_GameHostConnected && _GameHost == null)
            {
                //game host disconnected, time to shutdown
                ShutDown();
            }
            if (DateTime.Now - _LastShift > _ShiftInterval)
            {
                lock (_ConnectedClients)
                {
                    Packet shiftRightPacket = new Packet(Manager.PID_WORLDSHIFTRIGHT);
                    shiftRightPacket.AddInt(_ConnectedClients.Count);
                    SendToAll(shiftRightPacket);
                    _LastShift = DateTime.Now;
                }


            }
        }

        public virtual void ShutDown()
        {
            Manager._Server = null;
            StopListening();
            Dipose();
        }

        class ConnectedClient : ClientConnection
        {
            public int WorldOffset = -1;
            public ConnectedClient(TcpClient client)
                : base(client)
            {
                if (!_GameHostConnected)
                {
                    _GameHost = this;
                    _GameHostConnected = true;
                }
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
                if (this == _GameHost)
                {
                    _GameHost = null;
                }
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
                            case Manager.PID_WORLDDATAFULL:
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
