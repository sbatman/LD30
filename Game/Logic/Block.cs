using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1;
using Microsoft.Xna.Framework.Graphics;

namespace LD30.Logic
{
    internal class Block
    {
        public const int BLOCK_SIZE_MULTIPLIER = 32;

        public struct BlockType
        {
            public Texture2D Texture;
            public Color Colour;
            public Vector2 Size;
        }

        private Texture2D _BlockTexture;
        private Vector2 _Position;
        private Color _Colour;
        private Vector2 _Size;

        public Block(BlockType type, Vector2 position)
        {
            _BlockTexture = type.Texture;
            _Position = position;
            _Colour = type.Colour;
            _Size = type.Size;
        }

        public virtual void Draw()
        {
            GameCore.SpriteBatch.Draw(_BlockTexture, _Position, _Colour);
        }

        public virtual void Update()
        {

        }
    }
}
