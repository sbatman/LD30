using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LD30.Multiplayer.DataObjects;
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
        private const int BUTTON_COUNT = 2;

        public virtual void LoadContent()
        {
            _LastAbilityActivationTimes = new DateTime[BUTTON_COUNT];
            _AbilityCoolDownTimes = new TimeSpan[BUTTON_COUNT];

            _AbilityCoolDownTimes[0] = TimeSpan.FromSeconds(5);
            _AbilityCoolDownTimes[1] = TimeSpan.FromSeconds(2);

            _AbilityActivationKeys = new[] { Keys.D1, Keys.D2 };

            ResetAbilityTimers();

            _AbilityTextures = new Texture2D[BUTTON_COUNT];
            _AbilityTextures[0] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/CreateThree");
            _AbilityTextures[1] = GameCore.ContentManager.Load<Texture2D>("Graphics/AbilityBar/DestroyThree");
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
            float x = (Game.PlayerLevel.GetActiveCharacter().Position.X + (Block.BLOCK_SIZE_MULTIPLIER * 0.5f)) / Block.BLOCK_SIZE_MULTIPLIER;
            float y = ((Game.PlayerLevel.GetActiveCharacter().Position.Y + (Block.BLOCK_SIZE_MULTIPLIER * 0.5f)) / Block.BLOCK_SIZE_MULTIPLIER) + 1;
            switch (id)
            {
                case 0: //create 3

                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2((int)x, (int)y));
                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2((int)x - 1, (int)y));
                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Main, new Vector2((int)x + 1, (int)y));
                    break;
                case 1: //destroy 3

                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Air, new Vector2((int)x, (int)y));
                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Air, new Vector2((int)x - 1, (int)y));
                    Game.PlayerLevel.PlaceBlock(Block.BlockTypes.Air, new Vector2((int)x + 1, (int)y));
                    break;
            }
        }

        public void ResetAbilityTimers()
        {
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                _LastAbilityActivationTimes[i] = DateTime.MinValue;
            }
        }
    }
}
