using System.Collections.Generic;
using System.Text;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using Microsoft.Xna.Framework;

namespace Bounce.Multiplayer
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

        public enum MuliplayerModes
        {
            GHOST
        }

        internal static MuliplayerModes Mode = MuliplayerModes.GHOST;
        internal static Host _Server;
        internal static Client _Client;

        internal static List<Base>[] _Objects = new List<Base>[1];

        public static bool Hosting
        {
            get { return _Server != null; }
        }

        internal static void Update()
        {
            if (_Client != null) _Client.Update();
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

        internal static void RegisterObject(int z, Base gameObject)
        {
            lock (_Objects)
            {
                if (_Objects[z] == null) _Objects[z] = new List<Base>();
                _Objects[z].Add(gameObject);
            }
        }

        internal static void LevelBlockUpdate()
        {

        }

        internal static void LevelObjectUpdate(int z)
        {

        }

        internal static void LevelBlockDraw(int z)
        {

        }

        internal static void LevelObjectDraw(GameTime gameTime, int z)
        {
            lock (_Objects)
            {
                if (_Objects[z] == null) return;
                foreach (Base o in _Objects[z])
                {
                    o.Draw();
                }
            }
        }

        private static void DisposeClient()
        {
            _Client.Disconnect();
            _Client = null;
            for (int i = 0; i < _Objects.Length; i++)
            {
                List<Base> o = _Objects[i];
                if (o == null) continue;

                foreach (Base b in o) b.Dispose();

                o.Clear();
                _Objects[i] = null;
            }
        }

        internal static void Dispose()
        {
            if (_Server != null)
            {
                _Server.StopListening();
                _Server.Dipose();
            }
            if (_Client != null)
            {
                DisposeClient();
            }
        }
    }
}
