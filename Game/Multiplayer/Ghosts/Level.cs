using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using LD30.Logic;
using Microsoft.Xna.Framework;

namespace LD30.Multiplayer.Ghosts
{
    class Level : Base, IDisposable
    {
        private int _WorldOffset;
        private Block[,] _BlockData = new Block[Game.GAMELEVELSIZE, Game.GAMELEVELSIZE];

        public int WorldOffset
        {
            get { return _WorldOffset; }
        }

        public Level()
        {

        }

        public void BuildFromPacket(Packet p)
        {
            Dispose();
            object[] data = p.GetObjects();
            _WorldOffset = (int)data[0];
            int blocks = (int)data[1];

            Vector2 start = Vector2.Zero + (Vector2.UnitX * _WorldOffset * Game.GAMELEVELSIZE * Block.BLOCK_SIZE_MULTIPLIER);

            for (int i = 0; i < blocks; i++)
            {
                int x = (int)data[2 + (i * 3) + 0];
                int y = (int)data[2 + (i * 3) + 1];
                int type = (int)data[2 + (i * 3) + 2];
                _BlockData[x, y] = new Block((Block.BlockTypes)type, new Vector2(start.X + (x * Block.BLOCK_SIZE_MULTIPLIER), start.Y + (y * Block.BLOCK_SIZE_MULTIPLIER)), false);
            }
        }

        public override void Draw()
        {
            for (int x = 0; x < Game.GAMELEVELSIZE; x++)
            {
                for (int y = 0; y < Game.GAMELEVELSIZE; y++)
                {
                    if (_BlockData[x, y] != null) _BlockData[x, y].Draw();
                }
            }
            base.Draw();
        }

        public override void Dispose()
        {
            for (int x = 0; x < Game.GAMELEVELSIZE; x++)
            {
                for (int y = 0; y < Game.GAMELEVELSIZE; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Dispose();
                    _BlockData[x, y] = null;
                }
            }
        }
    }
}
