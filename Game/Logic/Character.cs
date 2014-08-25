using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NerfCorev2;
using NerfCorev2.PhysicsSystem;
using NerfCorev2.PhysicsSystem.Common;
using NerfCorev2.PhysicsSystem.Dynamics;
using NerfCorev2.PhysicsSystem.Dynamics.Contacts;
using NerfCorev2.Wrappers;
using Keyboard = NerfCorev2.Input.Keyboard;

namespace LD30.Logic
{
    internal class Character
    {
        public const int FRAME_HEIGHT = 36;
        public const int FRAME_WIDTH = 32;
        
        private Vector2 _Position;
        private Vector2 _Velocity;
        private Rect _CurrentDrawnRectangle;
        private Texture2D _Texture;
        private Vector2 _Size;
        private Color _Colour = Color.White;
        private bool _Visible;
        private Phys _PhysicsObject;

        //Character Stats
        private float StatJump = 2;
        private float StateRun = 4;

        //StateVariables
        private bool _Jumped;

        /// <summary>
        /// The level this character is active on
        /// </summary>
        private Level _CurrentLevel;

        public Vector2 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                _PhysicsObject.PhysicsFixture.Body.Position = value*0.01f;
            }
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
            _CurrentDrawnRectangle = SetAnimationFrame(1, 2);
            _Visible = true;
            _PhysicsObject = new Phys(Core.CreateRectangle((_Size - (Vector2.UnitX * 4) - (Vector2.UnitY * 2)) * 0.01f, _Position * 0.01f));
            _PhysicsObject.PhysicsFixture.Body.BodyType = BodyType.Dynamic;
            _PhysicsObject.PhysicsFixture.Body.Mass = 0.5f;
            _PhysicsObject.PhysicsFixture.Body.Restitution = 0.2f;
            _PhysicsObject.PhysicsFixture.Body.Friction = 0.1f;
            _PhysicsObject.PhysicsFixture.Body.AngularDamping = 1000;
            _PhysicsObject.PhysicsFixture.Body.AngularVelocity = 0;
            _PhysicsObject.PhysicsFixture.Body.Rotation = 0;
            _PhysicsObject.PhysicsFixture.Body.FixedRotation = true;
            _PhysicsObject.PhysicsFixture.UserData = this;
        }

        public virtual void Draw()
        {
            if (!_Visible) return;
            GameCore.SpriteBatch.Draw(_Texture, _Position - (_Size * 0.5f), _CurrentDrawnRectangle, _Colour);
        }

        public virtual void Update()
        {
            _Position = _PhysicsObject.PhysicsFixture.Body.Position * 100.0f;
            HandelMovement();
        }

        private void HandelMovement()
        {
            Vector2 movement = Vector2.Zero;
            if (IsLeftPressed())
            {
                movement = new Vector2(-StateRun, 0);
            }
            else if (IsRightPressed())
            {
                movement = new Vector2(StateRun, 0);
            }
            _PhysicsObject.PhysicsFixture.Body.ApplyForce(movement);
            bool jumpedLastUpdate = _Jumped;
            if (IsJumpPressed())
            {
                ContactEdge c = _PhysicsObject.PhysicsFixture.Body.ContactList;
                while (c != null)
                {
                    if (_Jumped) break;
                    if (c.Contact.IsTouching())
                    {
                        if (Phys.CollisionBetween(_PhysicsObject.PhysicsFixture, c.Contact.FixtureA, typeof(Character), typeof(Block)))
                        {
                            Vector2 dif;
                            FixedArray2<Vector2> p;
                            c.Contact.GetWorldManifold(out dif, out p);
                            dif.Normalize();
                            dif.X = 0;
                            if (dif.Y < 0)
                            {
                                _PhysicsObject.PhysicsFixture.Body.ApplyLinearImpulse(dif - (Vector2.UnitY * StatJump));
                                _Jumped = true;
                                break;
                            }
                        }
                    }
                    c = c.Next;
                }
            }
            if (jumpedLastUpdate) _Jumped = false;
        }

        private bool IsJumpPressed()
        {
            return Keyboard.IsKeyDown(Keys.Space, Game.GAME_CONTROL_LOCK) ||
                   Keyboard.IsKeyDown(Keys.W, Game.GAME_CONTROL_LOCK) ||
                   Keyboard.IsKeyDown(Keys.Up, Game.GAME_CONTROL_LOCK);
        }

        private bool IsLeftPressed()
        {
            return Keyboard.IsKeyDown(Keys.Left, Game.GAME_CONTROL_LOCK) ||
                   Keyboard.IsKeyDown(Keys.A, Game.GAME_CONTROL_LOCK);
        }

        private bool IsRightPressed()
        {
            return Keyboard.IsKeyDown(Keys.Right, Game.GAME_CONTROL_LOCK) ||
                   Keyboard.IsKeyDown(Keys.D, Game.GAME_CONTROL_LOCK);
        }


        public static Rect SetAnimationFrame(int x, int y)
        {
            return new Rect(x * FRAME_WIDTH, y * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
        }

        public virtual void SetLevel(Level currentLevel)
        {
            _CurrentLevel = currentLevel;
        }

    }
}
