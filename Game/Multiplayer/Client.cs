using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using LD30.Logic;
using LD30.Multiplayer.DataObjects;
using Microsoft.Xna.Framework;


namespace Bounce.Multiplayer
{
    class Client : InsaneDev.Networking.Client.Base
    {
        //Data Stores
        private readonly List<LD30.Multiplayer.DataObjects.Player> _KnownPlayers = new List<LD30.Multiplayer.DataObjects.Player>();
        private readonly Dictionary<long, Base> _ExternalObjects = new Dictionary<long, Base>();
        //Intervals
        private DateTime _LastExistenceAnnounce;
        private readonly TimeSpan _ExistenceAnnAnnounceInterval = new TimeSpan(0, 0, 0, 0, 500);
        private DateTime _LastPlayerDetailsAnnounce;
        private readonly TimeSpan _MinPlayerDetailsAnnounceInterval = new TimeSpan(0, 0, 0, 0, 500);
        //Info about client
        private long _MyPlayerID = -1;
        private string _Name = Guid.NewGuid().ToString();


        public Client(string ipaddress, int port)
        {
            Connect(ipaddress, port);
            if (IsConnected())
            {
                NerfCorev2.Plugins.Core.HookModule("Console", "AddStringToConsole", new object[] { "Connected" });
            }
            SendPacket(new Packet(Manager.PID_IDREQUEST));
        }

        public virtual void Update()
        {
            if (DateTime.Now - _LastExistenceAnnounce > _ExistenceAnnAnnounceInterval && _MyPlayerID != -1)
            {
                _LastExistenceAnnounce = DateTime.Now;
                Packet p = new Packet(Manager.PID_ANNOUNCEEXISTENCE);
                p.AddLong(_MyPlayerID);
                SendPacket(p);
            }
            if (GetPacketsToProcessCount() > 0)
            {
                foreach (Packet packet in GetPacketsToProcess())
                {
                    if (packet == null) continue;
                    switch (packet.Type)
                    {
                        case Manager.PID_LOADLEVEl:
                            if (Manager._Server != null) return;
                            //   Core.SetLevel(Encoding.UTF8.GetString((byte[])packet.GetObjects()[0]));
                            break;

                        case Manager.PID_SENDCHARACTERPHYSICS:
                            HandlePacket_SendCharacterPhysics(packet);
                            break;

                        case Manager.PID_IDRESPONCE:
                            _MyPlayerID = (long)packet.GetObjects()[0];
                            SendPlayerDetails();
                            break;

                        case Manager.PID_REQUESTPLAYERDETAILS:
                            object[] bObjects = packet.GetObjects();
                            long bId = (long)bObjects[0];
                            if (bId == _MyPlayerID)
                            {
                                if (DateTime.Now - _LastPlayerDetailsAnnounce > _MinPlayerDetailsAnnounceInterval)
                                {
                                    SendPlayerDetails();
                                }
                            }
                            break;

                        case Manager.PID_ANNOUNCEEXISTENCE:
                            object[] aObjects = packet.GetObjects();
                            long aID = (long)aObjects[0];
                            lock (_KnownPlayers)
                            {
                                if (_KnownPlayers.All(a => a.ID != aID))
                                {
                                    Packet detailsRequest = new Packet(Manager.PID_REQUESTPLAYERDETAILS);
                                    detailsRequest.AddLong(aID);
                                    SendPacket(detailsRequest);
                                }
                            }
                            break;

                        case Manager.PID_ANNOUNCEPLAYERDETAILS:
                            object[] objects = packet.GetObjects();
                            long id = (long)objects[0];
                            string name = Encoding.UTF8.GetString((byte[])objects[1]);
                            lock (_KnownPlayers)
                            {
                                LD30.Multiplayer.DataObjects.Player player = _KnownPlayers.FirstOrDefault(a => a.ID == id);
                                if (player != null)
                                {
                                    player.Name = name;
                                }
                                else
                                {
                                    _KnownPlayers.Add(new LD30.Multiplayer.DataObjects.Player(id, name));
                                }
                            }
                            break;
                    }
                }
            }
            if (LD30.Game.PlayerLevel.GetActiveCharacter() != null && _MyPlayerID != -1)
            {
                Character player = LD30.Game.PlayerLevel.GetActiveCharacter();
                Vector2 position = player.Position;
                Packet p = new Packet(Manager.PID_SENDCHARACTERPHYSICS);
                p.AddLong(_MyPlayerID);
                p.AddFloat(position.X);
                p.AddFloat(position.Y);
                SendPacket(p);
            }
        }

        private void SendPlayerDetails()
        {
            Packet p = new Packet(Manager.PID_ANNOUNCEPLAYERDETAILS);
            byte[] nameData = Encoding.UTF8.GetBytes(_Name);
            p.AddLong(_MyPlayerID);
            p.AddBytePacket(nameData);
            SendPacket(p);
            _LastPlayerDetailsAnnounce = DateTime.Now;

        }

        private void HandlePacket_SendCharacterPhysics(Packet p)
        {
            object[] data = p.GetObjects();
            long targetID = (long)data[0];
            if (targetID == _MyPlayerID) return;
            LD30.Multiplayer.Ghosts.Character character;
            if (_ExternalObjects.ContainsKey(targetID))
            {
                character = (LD30.Multiplayer.Ghosts.Character)_ExternalObjects[targetID];

            }
            else
            {
                character = new LD30.Multiplayer.Ghosts.Character();
                character.ID = targetID;
                Manager.RegisterObject(2, character);
                _ExternalObjects.Add(targetID, character);
            }
            character.SetPosition((float)data[1], (float)data[2]);
        }

        public string[] GetKnownPlayerNames()
        {
            return _KnownPlayers.Select(a => a.Name).ToArray();
        }
    }
}
