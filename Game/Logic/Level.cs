using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD30.Logic
{
    internal class Level
    {
        private readonly Block[,] _BlockData;
        private Vector2 _Size;

        public Level(Vector2 size)
        {
            _Size = size;
            _BlockData = new Block[(int)_Size.X, (int)_Size.Y];
        }

        public virtual void Draw()
        {
            GameCore.SpriteBatch.Begin();
            for (int x = 0; x < _Size.X; x++)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Draw();
                }
            }
            GameCore.SpriteBatch.End();
        }

        public virtual void Update()
        {
            for (int x = 0; x < _Size.X; x++)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Update();
                }
            }
        }

        public virtual void PlaceBlock(Block.BlockType type, Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (x >= _Size.X || x < 0) throw new ArgumentOutOfRangeException("position");
            if (y >= _Size.Y || y < 0) throw new ArgumentOutOfRangeException("position");

            _BlockData[x, y] = new Block(type, position * Block.BLOCK_SIZE_MULTIPLIER);
        }
    }
}
