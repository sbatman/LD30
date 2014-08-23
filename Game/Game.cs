using System.Collections.Generic;
using LD30.Logic;
using Microsoft.Xna.Framework;
using NerfCorev2;
using Microsoft.Xna.Framework.Graphics;
using NerfCorev2.PhysicsSystem;


namespace LD30
{
    public class Game : GameCore
    {

        //game
        private Level _CurrentLevel;

        //content
        public static Texture2D ContentCharacterTexture;
        private readonly Dictionary<string, Logic.Block.BlockType> _BlockTypes = new Dictionary<string, Block.BlockType>();

        //test
        private Character testcharacter;
        public Game()
            : base("LD30", true)
        {

        }

        protected override void CoreInit()
        {

        }

        protected override void CoreLoadContent()
        {
            Core.LoadContent(Content, GraphicsDevice);
            Core.Gravity.Value = new Vector2(0, 9);
            //Sort out blocks
            _BlockTypes.Add("Main", new Block.BlockType() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BaseRock") });

            _CurrentLevel = new Level(Vector2.One * 10);
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(0, 4));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(1, 3));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(2, 3));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(3, 3));
            _CurrentLevel.PlaceBlock(_BlockTypes["Main"], new Vector2(4, 4));

            //Sort out characters
            ContentCharacterTexture = Content.Load<Texture2D>("Graphics/Characters/Main");

            testcharacter = new Character(ContentCharacterTexture);
            _CurrentLevel.AddCharacterToLevel(testcharacter);
        }

        protected override void CorePostLoadContent()
        {

        }

        protected override void CoreUpdate()
        {
            if (_CurrentLevel != null) _CurrentLevel.Update();
        }

        protected override void CoreDraw()
        {
            if (_CurrentLevel != null) _CurrentLevel.Draw();
        }

        protected override void CoreResChange()
        {

        }

        protected override void CoreStartExiting()
        {

        }

        protected override bool CoreDebugCommandEntered(string[] action)
        {
            return false;
        }

        protected override void UnloadContent()
        {

        }
    }
}
