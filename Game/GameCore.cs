using System.Collections.Generic;
using LD30.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace LD30
{
    public class GameCore : Game
    {
        private GraphicsDeviceManager _Graphics;
        public static SpriteBatch SpriteBatch;
        private Level _CurrentLevel;
        private readonly Dictionary<string, Logic.Block.BlockType> _BlockTypes = new Dictionary<string, Block.BlockType>();

        public GameCore()
            : base()
        {
            _Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _BlockTypes.Add("Main", new Block.BlockType() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BaseRock") });

            _CurrentLevel = new Level(Vector2.One * 10);
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(0, 0));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(2, 0));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(1, 3));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(4, 4));
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.IsPressed_Back())
            {
                Exit();
            }

            if (_CurrentLevel != null) _CurrentLevel.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_CurrentLevel != null) _CurrentLevel.Draw();

            base.Draw(gameTime);
        }
    }
}
