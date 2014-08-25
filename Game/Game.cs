using System.Collections.Generic;
using LD30.Logic;
using LD30.Multiplayer;
using Microsoft.Xna.Framework;
using NerfCorev2;
using Microsoft.Xna.Framework.Graphics;
using NerfCorev2.PhysicsSystem;


namespace LD30
{
    public class Game : GameCore
    {


        //Controls
        internal const int GAME_CONTROL_LOCK = 10;
        internal const int GAMELEVELSIZE = 16;

        //game
        internal static Level PlayerLevel;

        //content
        internal static Texture2D ContentCharacterTexture;
        internal static Texture2D LevelEdgeTexture;
        internal static readonly Dictionary<Block.BlockTypes, Block.BlockData> BlockData = new Dictionary<Block.BlockTypes, Block.BlockData>();

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
            BlockData.Add(Block.BlockTypes.Main, new Block.BlockData() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BaseRock") });

            PlayerLevel = new Level(Vector2.One * GAMELEVELSIZE);
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(0,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(1,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(2,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(3,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(4,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(5,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(6,4));
            
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(8,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(9,4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(10, 4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(11, 4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(12, 4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(13, 4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(14, 4));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(15, 4));


            //Sort out characters
            ContentCharacterTexture = Content.Load<Texture2D>("Graphics/Characters/Main");
            LevelEdgeTexture = Content.Load<Texture2D>("Graphics/Level/Bar");

            testcharacter = new Character(ContentCharacterTexture);
            PlayerLevel.SetActiveCharacter(testcharacter);
        }

        protected override void CorePostLoadContent()
        {

        }

        protected override void CoreUpdate()
        {
            if (PlayerLevel != null) PlayerLevel.Update();
            Multiplayer.Manager.Update();
        }

        protected override void CoreDraw()
        {
            GameCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, GameCore.PrimaryCamera);
            if (PlayerLevel != null) PlayerLevel.Draw();

            Multiplayer.Manager.LevelObjectDraw();
            GameCore.SpriteBatch.End();
        }

        protected override void CoreResChange()
        {

        }

        protected override void CoreStartExiting()
        {

        }

        protected override bool CoreDebugCommandEntered(string[] action)
        {
            if (action.Length >= 1)
            {
                if (action[0].Equals("HOST"))
                {
                    Multiplayer.Manager._Server = new Host();
                    Multiplayer.Manager.EnterMultiplayer("127.0.0.1", Manager.MuliplayerModes.GHOST);
                }
            }
            if (action.Length >= 2)
            {
                if (action[0].Equals("CONNECT"))
                {
                    try
                    {
                        Multiplayer.Manager.EnterMultiplayer(action[1], Manager.MuliplayerModes.GHOST);
                    }
                    catch
                    {

                    }
                }
            }
            return false;
        }

        protected override void UnloadContent()
        {
            Manager.Dispose();
        }
    }
}
