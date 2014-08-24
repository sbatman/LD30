﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using LD30.Multiplayer.DataObjects;
using LD30.Multiplayer.Ghosts;
using Microsoft.Xna.Framework;
using NerfCorev2.Plugins;
using Base = InsaneDev.Networking.Client.Base;
using Character = LD30.Logic.Character;

namespace LD30.Multiplayer
{
    class Client : Base
    {
        //Data Stores
        private readonly List<Player> _KnownPlayers = new List<Player>();
        private readonly Dictionary<long, Bounce.Multiplayer.Ghosts.Base> _ExternalObjects = new Dictionary<long, Bounce.Multiplayer.Ghosts.Base>();
        //Intervals
        private DateTime _LastExistenceAnnounce;
        private readonly TimeSpan _ExistenceAnnAnnounceInterval = new TimeSpan(0, 0, 0, 0, 500);
        private DateTime _LastPlayerDetailsAnnounce;
        private readonly TimeSpan _MinPlayerDetailsAnnounceInterval = new TimeSpan(0, 0, 0, 0, 500);
        //Info about client
        private long _MyPlayerID = -1;
        private string _Name = Guid.NewGuid().ToString();
        private int _MyWorldPosition = -1;

        public Client(string ipaddress, int port)
        {
            Connect(ipaddress, port);
            if (IsConnected())
            {
                Core.HookModule("Console", "AddStringToConsole", new object[] { "Connected" });
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

                    object[] objects = packet.GetObjects();
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
                            long bId = (long)objects[0];
                            if (bId == _MyPlayerID)
                            {
                                if (DateTime.Now - _LastPlayerDetailsAnnounce > _MinPlayerDetailsAnnounceInterval)
                                {
                                    SendPlayerDetails();
                                }
                            }
                            break;

                        case Manager.PID_ANNOUNCEEXISTENCE:
                            long aID = (long)objects[0];
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
                            long id = (long)objects[0];
                            string name = Encoding.UTF8.GetString((byte[])objects[1]);
                            int worldOffset = (int)objects[2];
                            lock (_KnownPlayers)
                            {
                                Player player = _KnownPlayers.FirstOrDefault(a => a.ID == id);
                                if (player != null)
                                {
                                    player.Name = name;
                                    player.WorldOffset = worldOffset;
                                }
                                else
                                {
                                    _KnownPlayers.Add(new Player(id, name) { WorldOffset = worldOffset });
                                }
                            }
                            break;

                        case Manager.PID_WORLDDATAFULL:
                            long playerid = (long)objects[objects.Length - 1];
                            if (playerid == _MyPlayerID) continue;
                            lock (_KnownPlayers)
                            {
                                Player player = _KnownPlayers.FirstOrDefault(a => a.ID == playerid);
                                if (player != null)
                                {
                                    if (player.Level == null)
                                    {
                                        player.Level = new Level();
                                        Manager.RegisterObject(player.Level);
                                    }
                                    player.Level.BuildFromPacket(packet);

                                }
                            }
                            break;

                        case Manager.PID_WORLDSHIFTRIGHT:
                            break;

                        case Manager.PID_SENDWORLDPOSITION:
                            _MyWorldPosition = (int)objects[0];
                            Game.PlayerLevel.OffsetWorld(_MyWorldPosition);
                            SendPlayerDetails();
                            break;
                    }
                }
            }
            if (Game.PlayerLevel.GetActiveCharacter() != null && _MyPlayerID != -1)
            {
                Character player = Game.PlayerLevel.GetActiveCharacter();
                Vector2 position = player.Position;
                Packet p = new Packet(Manager.PID_SENDCHARACTERPHYSICS);
                p.AddLong(_MyPlayerID);
                p.AddFloat(position.X + (Game.PlayerLevel.Size.X));
                p.AddFloat(position.Y);
                SendPacket(p);
            }
        }

        private void SendWorldData()
        {
            Packet p = Game.PlayerLevel.ToPacket();
            p.AddLong(_MyPlayerID);
            SendPacket(p);
        }

        private void SendPlayerDetails()
        {
            Packet p = new Packet(Manager.PID_ANNOUNCEPLAYERDETAILS);
            byte[] nameData = Encoding.UTF8.GetBytes(_Name);
            p.AddLong(_MyPlayerID);
            p.AddBytePacket(nameData);
            p.AddInt(_MyWorldPosition);
            SendPacket(p);
            _LastPlayerDetailsAnnounce = DateTime.Now;

            SendWorldData();

        }

        private void HandlePacket_SendCharacterPhysics(Packet p)
        {
            object[] data = p.GetObjects();
            long targetID = (long)data[0];
            if (targetID == _MyPlayerID) return;
            Ghosts.Character character;
            if (_ExternalObjects.ContainsKey(targetID))
            {
                character = (Ghosts.Character)_ExternalObjects[targetID];
            }
            else
            {
                character = new Ghosts.Character(Game.ContentCharacterTexture) {ID = targetID};
                Manager.RegisterObject(character);
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
