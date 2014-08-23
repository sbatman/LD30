using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD30.Logic
{
    internal class Level
    {
        private Block[,] _BlockData;
        private Vector2 _Size;

        public Level(Vector2 size)
        {
            _Size = size;
            _BlockData = new Block[(int)_Size.X, (int)_Size.Y];
        }

        public virtual void Draw()
        {
            for (int x = 0; x < _Size.X; x++)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Draw();
                }
            }
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
    }
}
