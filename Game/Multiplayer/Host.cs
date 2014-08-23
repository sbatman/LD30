using System.Net;
using System.Net.Sockets;
using InsaneDev.Networking;
using InsaneDev.Networking.Server;
using Base = InsaneDev.Networking.Server.Base;

namespace Bounce.Multiplayer
{
    class Host : Base
    {
        private static long _NextObjectID = 0;
        private static readonly object _IDLock = new object();
        internal Host()
        {
            Init(new IPEndPoint(IPAddress.Any, 3456), typeof(ConnectedClient));
            StartListening();
        }

        class ConnectedClient : ClientConnection
        {
            public ConnectedClient(TcpClient client)
                : base(client)
            {

            }

            protected override void OnConnect()
            {
                NerfCorev2.Plugins.Core.HookModule("Console", "AddStringToConsole", new object[] { "Client Connected" });
            }

            protected override void OnDisconnect()
            {
                NerfCorev2.Plugins.Core.HookModule("Console", "AddStringToConsole", new object[] { "Client Disconnected" });
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
