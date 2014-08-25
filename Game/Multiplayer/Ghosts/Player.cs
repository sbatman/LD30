using Bounce.Multiplayer.Ghosts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NerfCorev2;
using NerfCorev2.Wrappers;

namespace LD30.Multiplayer.Ghosts
{
    class Character : Base
    {
        private Texture2D _Texture;
        private Vector2 _Size;
        private Color _Colour = Color.White;
        private Rect _CurrentDrawnRectangle;
        private int _Gfx;

        public Character(Texture2D texture)
        {
            _Texture = texture;
            _Size = new Vector2(Logic.Character.FRAME_WIDTH, Logic.Character.FRAME_HEIGHT);
            _CurrentDrawnRectangle = Logic.Character.SetAnimationFrame(1, 2);
        }

        public override void Draw()
        {
            GameCore.SpriteBatch.Draw(_Texture, _Position - (Vector2.UnitY * (_Size.Y * 0.5f)) - (Vector2.UnitX * _Size.X), _CurrentDrawnRectangle, _Colour);
        }

        public void SetPosition(float x, float y)
        {
            _Position.X = x;
            _Position.Y = y;
        }
    }
}
