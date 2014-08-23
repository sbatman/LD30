﻿using System.Collections.Generic;
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
        enum BlockTypes
        {
            Main
        }

        //Controls
        public const int GAME_CONTROL_LOCK = 10;

        //game
        internal static Level PlayerLevel;

        //content
        public static Texture2D ContentCharacterTexture;
        private readonly Dictionary<BlockTypes, Block.BlockType> _BlockTypes = new Dictionary<BlockTypes, Block.BlockType>();

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
            _BlockTypes.Add(BlockTypes.Main, new Block.BlockType() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BaseRock") });

            PlayerLevel = new Level(Vector2.One * 16);
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(0, 4));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(1, 3));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(2, 3));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(3, 3));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(4, 4));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(5, 4));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(6, 5));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(7, 5));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(8, 6));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(9, 6));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(10, 6));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(11, 6));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(12, 5));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(13, 5));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(14, 4));
            PlayerLevel.PlaceBlock(_BlockTypes[BlockTypes.Main], new Vector2(15, 3));


            //Sort out characters
            ContentCharacterTexture = Content.Load<Texture2D>("Graphics/Characters/Main");

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
