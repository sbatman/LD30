﻿using System;
using System.Collections.Generic;
using InsaneDev.Networking;
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
        internal enum GameState
        {
            Idle,
            CountDown,
            Playing
        }

        private static GameState _CurrentGameState;
        private static DateTime _CountDownStart;

        //Controls
        internal const int GAME_CONTROL_LOCK = 10;
        internal const int GAMELEVELSIZE = 16;

        //game
        internal static Level PlayerLevel;
        internal static Logic.AbilityBar PlayerAbilityBar;

        //content
        internal static Texture2D ContentCharacterTexture;
        internal static Texture2D LevelEdgeTexture;
        internal static Texture2D[] BackgroundTextures = new Texture2D[5];
        internal static readonly Dictionary<Block.BlockTypes, Block.BlockData> BlockData = new Dictionary<Block.BlockTypes, Block.BlockData>();

        //test
        private Character testcharacter;
        public Game()
            : base("LD30", true)
        {

        }

        protected override void CoreInit()
        {
            _CurrentGameState = GameState.Idle;
        }

        protected override void CoreLoadContent()
        {
            for (int i = 0; i < 5; i++)
                BackgroundTextures[i] = ContentManager.Load<Texture2D>("Graphics/Backgrounds/" + i);

            Core.LoadContent(Content, GraphicsDevice);
            Core.Gravity.Value = new Vector2(0, 9);

            PlayerAbilityBar = new AbilityBar();
            PlayerAbilityBar.LoadContent();
            //Sort out blocks
            BlockData.Add(Block.BlockTypes.Main, new Block.BlockData() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BaseRock") });
            BlockData.Add(Block.BlockTypes.Exploding, new Block.BlockData() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/TNTBlock") });
            BlockData.Add(Block.BlockTypes.BigBomb, new Block.BlockData() { Colour = Color.White, Size = Vector2.One * 32, Texture = Content.Load<Texture2D>("Graphics/Blocks/BBBlock") });

            PlayerLevel = new Level(Vector2.One * GAMELEVELSIZE);

            ResetMap();

            //Sort out characters
            ContentCharacterTexture = Content.Load<Texture2D>("Graphics/Characters/Main");
            LevelEdgeTexture = Content.Load<Texture2D>("Graphics/Level/Bar");

            testcharacter = new Character(ContentCharacterTexture);
            testcharacter.Position = (Vector2.UnitX * 128);
            PlayerLevel.SetActiveCharacter(testcharacter);
        }

        protected override void CorePostLoadContent()
        {

        }

        protected static void ResetMap()
        {
            PlayerLevel.ClearWorld();
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(2, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(3, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(4, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(5, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(6, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(7, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(8, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(9, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(10, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(11, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(12, 12));
            PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2(13, 12));
        }

        protected override void CoreUpdate()
        {
            if (PlayerLevel != null) PlayerLevel.Update();
            Manager.Update();

            if (Manager._Server != null)
            {
                switch (_CurrentGameState)
                {
                    case GameState.Idle:
                        PlayerAbilityBar.Update();
                        break;
                    case GameState.CountDown:
                        if (DateTime.Now - _CountDownStart > TimeSpan.FromSeconds(3))
                        {
                            Packet p = new Packet(Manager.PID_CHANGEGAMEMODE);
                            p.AddInt((int)GameState.Playing);
                            Manager._Client.SendPacket(p);
                        }
                        break;
                    case GameState.Playing:
                        PlayerAbilityBar.Update();
                        break;
                }
            }
            else
            {
                switch (_CurrentGameState)
                {
                    case GameState.Idle:
                        PlayerAbilityBar.Update();
                        break;
                    case GameState.CountDown:
                        break;
                    case GameState.Playing:
                        PlayerAbilityBar.Update();
                        break;
                }
            }
        }

        protected override void CoreDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, PrimaryCamera);
            if (PlayerLevel != null) PlayerLevel.Draw();

            Manager.LevelObjectDraw();
            SpriteBatch.End();
            SpriteBatch.Begin();
            switch (_CurrentGameState)
            {
                case GameState.Idle:
                    PlayerAbilityBar.Draw();
                    break;
                case GameState.CountDown:
                    break;
                case GameState.Playing:
                    PlayerAbilityBar.Draw();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SpriteBatch.End();
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
                    Manager._Server = new Host();
                    Manager.EnterMultiplayer("127.0.0.1", Manager.MuliplayerModes.NORMAL);
                    return true;
                }
            }
            if (action.Length >= 2)
            {
                if (action[0].Equals("CONNECT"))
                {
                    try
                    {
                        Manager.EnterMultiplayer(action[1], Manager.MuliplayerModes.NORMAL);
                        return true;
                    }
                    catch
                    {

                    }
                }
                if (action[0].Equals("STATE"))
                {
                    if (Manager._Server == null) return false;
                    Packet p = new Packet(Manager.PID_CHANGEGAMEMODE);
                    p.AddInt(int.Parse(action[1]));
                    Manager._Client.SendPacket(p);
                }
            }
            return false;
        }

        protected override void UnloadContent()
        {
            Manager.Dispose();

        }

        internal static void ChangeGameMode(GameState newGameMode)
        {
            if (_CurrentGameState == GameState.Idle && newGameMode == GameState.CountDown)
            {
                _CountDownStart = DateTime.Now;

            }
            if (_CurrentGameState == GameState.CountDown && newGameMode == GameState.Playing)
            {
                PlayerAbilityBar.ResetAbilityTimers();
                PlayerLevel.GetActiveCharacter().Position = PlayerLevel.WorldTopLeft + (Vector2.UnitX * 128);
                ResetMap();
                if (Manager._Client != null) Manager._Client.SendWorldData();
            }
            _CurrentGameState = newGameMode;

        }
    }
}
