using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD30.Logic
{
    internal class Character
    {
        private const int FRAME_HEIGHT = 36;
        private const int FRAME_WIDTH = 32;

        private Vector2 _Position;
        private Rectangle _CurrentDrawnRectangle;
        private Texture2D _Texture;
        private Vector2 _Size;
        private Color _Colour = Color.White;
        private bool _Visible;

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
        }

        public virtual void Draw()
        {
            if (!_Visible) return;
            GameCore.SpriteBatch.Draw(_Texture, _Position, _CurrentDrawnRectangle, _Colour);
        }

        public virtual void Update()
        {

        }

        private void SetAnimationFrame(int x, int y)
        {
            _CurrentDrawnRectangle = new Rectangle(x * FRAME_WIDTH, y * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
        }

        public virtual void SetLevel(Level currentLevel)
        {
            _CurrentLevel = currentLevel;
        }

    }
}
