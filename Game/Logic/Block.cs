using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NerfCorev2.PhysicsSystem.Dynamics;
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
        private Phys _PhysicsObject;


        public Block(BlockType type, Vector2 position)
        {
            _BlockTexture = type.Texture;
            _Position = position;
            _Colour = type.Colour;
            _Size = type.Size;
            _PhysicsObject = new Phys(NerfCorev2.PhysicsSystem.Core.CreateRectangle(_Size * 0.01f, _Position * 0.01f));
            _PhysicsObject.PhysicsFixture.Body.BodyType = BodyType.Static;


        }

        public virtual void Draw()
        {
            Game.SpriteBatch.Draw(_BlockTexture, _Position - (_Size * 0.5f), _Colour);
        }

        public virtual void Update()
        {

        }
    }
}
