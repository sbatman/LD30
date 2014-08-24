using System.Collections.Generic;
using System.Text;
using Bounce.Multiplayer;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using Microsoft.Xna.Framework;

namespace LD30.Multiplayer
{
    internal static class Manager
    {
        public const int PID_LOADLEVEl = 100;
        public const int PID_SENDCHARACTERPHYSICS = 200;
        public const int PID_IDREQUEST = 300;
        public const int PID_IDRESPONCE = 301;
        public const int PID_ANNOUNCEPLAYERDETAILS = 400;
        public const int PID_ANNOUNCEEXISTENCE = 401;
        public const int PID_REQUESTPLAYERDETAILS = 402;
        public const int PID_SENDWORLDPOSITION = 500;
        public const int PID_WORLDDATAFULL = 600;
        public const int PID_WORLDSHIFTRIGHT = 700;

        public enum MuliplayerModes
        {
            GHOST
        }

        internal static MuliplayerModes Mode = MuliplayerModes.GHOST;
        internal static Host _Server;
        internal static Client _Client;
        internal static List<Base> _Objects = new List<Base>();


        public static bool Hosting
        {
            get { return _Server != null; }
        }

        internal static void Update()
        {
            if (_Client != null) _Client.Update();
            if (_Server != null) _Server.Update();
        }

        internal static void EnterMultiplayer(string ip, MuliplayerModes mode)
        {
            if (_Client != null)
            {
                DisposeClient();
            }
            Mode = mode;
            _Client = new Client(ip, 3456);
        }

        internal static void LoadLevel(string levelName)
        {
            if (_Server != null)
            {
                Packet p = new Packet(PID_LOADLEVEl);
                p.AddBytePacket(Encoding.UTF8.GetBytes(levelName));
                _Server.SendToAll(p);
            }
        }

        internal static void PostLoadLevel()
        {
            // UIManager.PMPGhostGame.SetVisibility(true);
        }

        internal static void RegisterObject(Base gameObject)
        {
            lock (_Objects)
            {
                _Objects.Add(gameObject);
            }
        }

        internal static void LevelBlockUpdate()
        {

        }

        internal static void LevelObjectUpdate()
        {

        }

        internal static void LevelBlockDraw()
        {

        }

        internal static void LevelObjectDraw()
        {
            lock (_Objects)
            {

                foreach (Base o in _Objects)
                {
                    o.Draw();
                }
            }
        }

        private static void DisposeClient()
        {
            _Client.Disconnect();
            _Client = null;

            List<Base> o = _Objects;

            foreach (Base b in o) b.Dispose();

            o.Clear();
            _Objects = null;

        }

        internal static void Dispose()
        {
            if (_Server != null)
            {
                _Server.StopListening();
                _Server.Dipose();
                _Server = null;
            }
            if (_Client != null)
            {
                DisposeClient();
                _Client = null;
            }
        }
    }
}
