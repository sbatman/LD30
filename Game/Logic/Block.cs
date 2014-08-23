using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NerfCorev2.PhysicsSystem;
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
        private bool _HasPhysics;


        public Block(BlockType type, Vector2 position, bool physics = true)
        {
            _BlockTexture = type.Texture;
            _Position = position;
            _Colour = type.Colour;
            _Size = type.Size;
            _HasPhysics = physics;
            if (_HasPhysics)
            {
                _PhysicsObject = new Phys(Core.CreateRectangle(_Size * 0.01f, _Position * 0.01f));
                _PhysicsObject.PhysicsFixture.Body.BodyType = BodyType.Static;
                _PhysicsObject.PhysicsFixture.UserData = this;
            }

        }

        public virtual void SetPosition(Vector2 newPosition)
        {
            _Position = newPosition;
            if (_HasPhysics) _PhysicsObject.PhysicsFixture.Body.Position = newPosition * 0.01f;
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
