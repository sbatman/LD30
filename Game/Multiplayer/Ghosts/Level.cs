using System;
using Bounce.Multiplayer.Ghosts;
using InsaneDev.Networking;
using LD30.Logic;
using Microsoft.Xna.Framework;
using NerfCorev2;
using NerfCorev2.Wrappers;

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
                int x = (int)data[2 + (i * 4) + 0];
                int y = (int)data[2 + (i * 4) + 1];
                int type = (int)data[2 + (i * 4) + 2];
                int state = (int)data[2 + (i * 4) + 3];
                _BlockData[x, y] = new Block((Block.BlockTypes)type, new Vector2(start.X + (x * Block.BLOCK_SIZE_MULTIPLIER), start.Y + (y * Block.BLOCK_SIZE_MULTIPLIER)), false);
                _BlockData[x, y].SetState(state);
            }
        }

        public override void Draw()
        {
            Vector2 worldTopLeft = (Vector2.UnitX * _WorldOffset * Game.GAMELEVELSIZE * Block.BLOCK_SIZE_MULTIPLIER);
            GameCore.SpriteBatch.Draw(Game.BackgroundTextures[_WorldOffset % 5],
               new Rect(worldTopLeft.X - (Block.BLOCK_SIZE_MULTIPLIER * 0.5f), worldTopLeft.Y - (Block.BLOCK_SIZE_MULTIPLIER * 0.5f), Block.BLOCK_SIZE_MULTIPLIER * Game.GAMELEVELSIZE,
                   Block.BLOCK_SIZE_MULTIPLIER * Game.GAMELEVELSIZE), Color.White);
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

        public virtual Tuple<Block.BlockTypes, int>[] GetCollumn(int index)
        {
            Tuple<Block.BlockTypes, int>[] returnData = new Tuple<Block.BlockTypes, int>[Game.GAMELEVELSIZE];
            for (int y = 0; y < Game.GAMELEVELSIZE; y++)
            {
                if (_BlockData[index, y] == null)
                {
                    returnData[y] = new Tuple<Block.BlockTypes, int>(Block.BlockTypes.Air, 0);
                }
                else
                {
                    returnData[y] = new Tuple<Block.BlockTypes, int>(_BlockData[index, y].BlockType, _BlockData[index, y].GetState());
                }

            }
            return returnData;
        }
    }
}
