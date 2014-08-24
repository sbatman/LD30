using System;
using Microsoft.Xna.Framework;

namespace Bounce.Multiplayer.Ghosts
{
    class Base : IDisposable
    {
        private long _ID;
        public Vector2 _Position;

        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual void Draw()
        {

        }
        public virtual void Update()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
