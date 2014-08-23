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

        private Texture2D _BlockTexture;
        private Vector2 _Position;
        private Color _Colour;
        private Vector2 _Size;

        public virtual void Draw()
        {
            GameCore.SpriteBatch.Draw(_BlockTexture, _Position, _Colour);
        }

        public virtual void Update()
        {

        }
    }
}
