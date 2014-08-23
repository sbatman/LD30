using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NerfCorev2.PhysicsSystem.Dynamics;
using NerfCorev2.Wrappers;

namespace LD30.Logic
{
    internal class Character
    {
        private const int FRAME_HEIGHT = 36;
        private const int FRAME_WIDTH = 32;
        private const int MAX_PHYSICS_STEPS = 4;

        private static Vector2 _Gravity = new Vector2(0, 0.1f);

        private Vector2 _Position;
        private Vector2 _Velocity;
        private Rect _CurrentDrawnRectangle;
        private Texture2D _Texture;
        private Vector2 _Size;
        private Color _Colour = Color.White;
        private bool _Visible;
        private Phys _PhysicsObject;


        /// <summary>
        /// The level this character is active on
        /// </summary>
        private Level _CurrentLevel;

        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public Color Colour
        {
            get { return _Colour; }
            set { _Colour = value; }
        }

        public Character(Texture2D texture)
        {
            _Texture = texture;
            _Size = new Vector2(FRAME_WIDTH, FRAME_HEIGHT);
            SetAnimationFrame(1, 2);
            _Visible = true;
            _PhysicsObject = new Phys(NerfCorev2.PhysicsSystem.Core.CreateRectangle((_Size - (Vector2.UnitX * 4)) * 0.01f, _Position * 0.01f));
            _PhysicsObject.PhysicsFixture.Body.BodyType = BodyType.Dynamic;
            _PhysicsObject.PhysicsFixture.Body.Mass = 0.5f;
            _PhysicsObject.PhysicsFixture.Body.Restitution = 0.2f;
            _PhysicsObject.PhysicsFixture.Body.Friction = 0.1f;
            _PhysicsObject.PhysicsFixture.Body.AngularDamping = 1000;
            _PhysicsObject.PhysicsFixture.Body.AngularVelocity = 0;
            _PhysicsObject.PhysicsFixture.Body.Rotation = 0;
            _PhysicsObject.PhysicsFixture.Body.FixedRotation = true;
        }

        public virtual void Draw()
        {
            if (!_Visible) return;
            Game.SpriteBatch.Draw(_Texture, _Position - (_Size * 0.5f), _CurrentDrawnRectangle, _Colour);
        }

        public virtual void Update()
        {
            _Position = _PhysicsObject.PhysicsFixture.Body.Position * 100.0f;


        }


        private void SetAnimationFrame(int x, int y)
        {
            _CurrentDrawnRectangle = new Rect(x * FRAME_WIDTH, y * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
        }

        public virtual void SetLevel(Level currentLevel)
        {
            _CurrentLevel = currentLevel;
        }

    }
}
