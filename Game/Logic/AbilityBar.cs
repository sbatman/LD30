using System;
using LD30.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NerfCorev2;

namespace LD30.Logic
{
    class AbilityBar : IDisposable
    {
        private const int BUTTON_WIDTH = 32;
        private const int BUTTON_HEIGHT = 32;
        private Texture2D[] _AbilityTextures;
        private DateTime[] _LastAbilityActivationTimes;
        private TimeSpan[] _AbilityCoolDownTimes;
        private Keys[] _AbilityActivationKeys;
        private Keys[] _AlternateAbilityActivationKeys;
        private const int BUTTON_COUNT = 5;

        public virtual void LoadContent()
        {
            _LastAbilityActivationTimes = new DateTime[BUTTON_COUNT];
            _AbilityCoolDownTimes = new TimeSpan[BUTTON_COUNT];

            _AbilityCoolDownTimes[0] = TimeSpan.FromSeconds(5);
            _AbilityCoolDownTimes[1] = TimeSpan.FromSeconds(5);
            _AbilityCoolDownTimes[2] = TimeSpan.FromSeconds(3);
            _AbilityCoolDownTimes[3] = TimeSpan.FromSeconds(5);
            _AbilityCoolDownTimes[4] = TimeSpan.FromSeconds(30);

            _AbilityActivationKeys = new[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5 };
            _AlternateAbilityActivationKeys = new[] { Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5 };

            ResetAbilityTimers();

            _AbilityTextures = new Texture2D[BUTTON_COUNT];
            _AbilityTextures[0] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/CreateThree");
            _AbilityTextures[1] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/DestroyThree");
            _AbilityTextures[2] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/TNT");
            _AbilityTextures[3] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/VerticalThree");
            _AbilityTextures[4] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/BigBomb");
        }

        public virtual void Update()
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                if (NerfCorev2.Input.Keyboard.WasKeyReleased(_AbilityActivationKeys[i], Game.GAME_CONTROL_LOCK))
                {
                    if (now - _LastAbilityActivationTimes[i] > _AbilityCoolDownTimes[i])
                    {
                        _LastAbilityActivationTimes[i] = DateTime.Now;
                        TriggerAbility(i);
                    }
                }
                else if (NerfCorev2.Input.Keyboard.WasKeyReleased(_AlternateAbilityActivationKeys[i], Game.GAME_CONTROL_LOCK))
                {
                    if (now - _LastAbilityActivationTimes[i] > _AbilityCoolDownTimes[i])
                    {
                        _LastAbilityActivationTimes[i] = DateTime.Now;
                        TriggerAbility(i);
                    }
                }
            }
        }

        public virtual void Draw()
        {
            const float barWidth = BUTTON_COUNT * BUTTON_WIDTH;
            DateTime now = DateTime.Now;
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                Game.SpriteBatch.Draw(_AbilityTextures[i], new Vector2((Game.ScreenSize.X * 0.5f) - barWidth + (i * BUTTON_WIDTH), Game.ScreenSize.Y - BUTTON_HEIGHT), (now - _LastAbilityActivationTimes[i] > _AbilityCoolDownTimes[i]) ? Color.White : new Color(0.2f, 0.2f, 0.2f));
            }
        }

        public void Dispose()
        {

        }

        public void TriggerAbility(int id)
        {
            int x = (int)Math.Round((Game.PlayerLevel.GetActiveCharacter().Position.X + (Block.BLOCK_SIZE_MULTIPLIER * 0.5f)) / Block.BLOCK_SIZE_MULTIPLIER) - (Game.PlayerLevel.WorldOffset * Game.GAMELEVELSIZE);
            int y = (int)Math.Round((Game.PlayerLevel.GetActiveCharacter().Position.Y + (Block.BLOCK_SIZE_MULTIPLIER)) / Block.BLOCK_SIZE_MULTIPLIER) + 1;
            Level level = Game.PlayerLevel;
            switch (id)
            {
                case 0: //create 3

                    if (level.GetBlock(new Vector2(x, y)) == null || level.GetBlock(new Vector2(x, y)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x, y));
                    if (level.GetBlock(new Vector2(x - 1, y)) == null || level.GetBlock(new Vector2(x - 1, y)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x - 1, y));
                    if (level.GetBlock(new Vector2(x + 1, y)) == null || level.GetBlock(new Vector2(x + 1, y)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x + 1, y));
                    if (Manager._Client != null) Manager._Client.SendWorldData();
                    break;
                case 1: //destroy 3

                    if (level.GetBlock(new Vector2(x, y)) == null || level.GetBlock(new Vector2(x, y)).BlockType == Block.BlockTypes.Main) level.PlaceBlock(Block.BlockTypes.Air, new Vector2(x, y));
                    if (level.GetBlock(new Vector2(x - 1, y)) == null || level.GetBlock(new Vector2(x - 1, y)).BlockType == Block.BlockTypes.Main) level.PlaceBlock(Block.BlockTypes.Air, new Vector2(x - 1, y));
                    if (level.GetBlock(new Vector2(x + 1, y)) == null || level.GetBlock(new Vector2(x + 1, y)).BlockType == Block.BlockTypes.Main) level.PlaceBlock(Block.BlockTypes.Air, new Vector2(x + 1, y));
                    if (Manager._Client != null) Manager._Client.SendWorldData();
                    break;
                case 2: //TNT
                    level.PlaceBlock(Block.BlockTypes.Exploding, new Vector2(x, y), 5);
                    if (Manager._Client != null) Manager._Client.SendWorldData();
                    break;
                case 3: //Vertical Three
                    if (level.GetBlock(new Vector2(x, y)) == null || level.GetBlock(new Vector2(x, y)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x, y));
                    if (level.GetBlock(new Vector2(x, y + 1)) == null || level.GetBlock(new Vector2(x, y + 1)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x, y + 1));
                    if (level.GetBlock(new Vector2(x, y + 2)) == null || level.GetBlock(new Vector2(x, y + 2)).BlockType == Block.BlockTypes.Air) level.PlaceBlock(Block.BlockTypes.Main, new Vector2(x, y + 2));
                    if (Manager._Client != null) Manager._Client.SendWorldData();
                    break;
                case 4: //Vertical Three
                    level.PlaceBlock(Block.BlockTypes.BigBomb, new Vector2(x, y), 5);
                    if (Manager._Client != null) Manager._Client.SendWorldData();
                    break;
            }
        }

        public void ResetAbilityTimers()
        {
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                _LastAbilityActivationTimes[i] = DateTime.MinValue;
            }
            _LastAbilityActivationTimes[4] = DateTime.Now;
        }
    }
}
