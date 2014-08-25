using System;
using Microsoft.Xna.Framework;
using NerfCorev2;
using NerfCorev2.PhysicsSystem;
using NerfCorev2.PhysicsSystem.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace LD30.Logic
{
    internal class Block : IDisposable
    {
        internal enum BlockTypes
        {
            Air,
            Main
        }

        public const int BLOCK_SIZE_MULTIPLIER = 32;

        public struct BlockData
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
        private BlockTypes _BlockType;

        private bool _Disposed;

        public BlockTypes BlockType
        {
            get { return _BlockType; }
        }

        public Block(BlockTypes type, Vector2 position, bool physics = true)
        {
            BlockData data = Game.BlockData[type];
            _BlockType = type;
            _BlockTexture = data.Texture;
            _Position = position;
            _Colour = data.Colour;
            _Size = data.Size;
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
            lock (this)
            {
                _Position = newPosition;
                if (_HasPhysics) _PhysicsObject.PhysicsFixture.Body.Position = newPosition*0.01f;
            }
        }

        public virtual void Draw()
        {
            lock (this)
            {
                GameCore.SpriteBatch.Draw(_BlockTexture, _Position - (_Size*0.5f), _Colour);
            }
        }

        public virtual void Update()
        {

        }

        public void Dispose()
        {
            lock (this)
            {
                _BlockTexture = null;
                if (_HasPhysics)
                {
                    Core.RemoveFixture(_PhysicsObject.PhysicsFixture);
                    _PhysicsObject = null;
                    _HasPhysics = false;
                }
                _Disposed = true;
            }
        }

        public bool IsDisposed()
        {
            return _Disposed;
        }
    }
}
