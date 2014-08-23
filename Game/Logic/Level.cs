using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NerfCorev2;
using NerfCorev2.PhysicsSystem;
using NerfCorev2.PhysicsSystem.Dynamics;
using NerfCorev2.Wrappers;

namespace LD30.Logic
{
    internal class Level
    {
        private readonly Block[,] _BlockData;
        private readonly List<Logic.Character> _ActiveCharacters = new List<Character>();
        private Vector2 _Size;
        private Phys LevelWallLeft;

        public Level(Vector2 size)
        {
            _Size = size;
            _BlockData = new Block[(int)_Size.X, (int)_Size.Y];

            LevelWallLeft = new Phys(Core.CreateRectangle(new Vector2(32, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER) * 0.01f, new Vector2(-32, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER * 0.5f) * 0.01f));
            LevelWallLeft.PhysicsFixture.Body.BodyType = BodyType.Static;
        }

        public virtual void Draw()
        {
            GameCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, GameCore.PrimaryCamera);
            for (int x = 0; x < _Size.X; x++)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Draw();
                }
            }
            foreach (Character character in _ActiveCharacters)
            {
                character.Draw();
            }
            GameCore.SpriteBatch.End();
        }

        public virtual void Update()
        {
            for (int x = 0; x < _Size.X; x++)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[x, y] == null) continue;
                    _BlockData[x, y].Update();
                }
            }
            foreach (Character character in _ActiveCharacters)
            {
                character.Update();
                GameCore.PrimaryCamera.Position = character.Position - (GameCore.ScreenSize * 0.5f);
            }
        }

        public virtual void PlaceBlock(Block.BlockType type, Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (x >= _Size.X || x < 0) throw new ArgumentOutOfRangeException("position");
            if (y >= _Size.Y || y < 0) throw new ArgumentOutOfRangeException("position");

            _BlockData[x, y] = new Block(type, position * Block.BLOCK_SIZE_MULTIPLIER);
        }

        public virtual void AddCharacterToLevel(Character character)
        {
            _ActiveCharacters.Add(character);
            character.SetLevel(this);
        }

        public virtual Block BlockAtGC(Vector2 position)
        {
            int x = (int)position.X / Block.BLOCK_SIZE_MULTIPLIER;
            int y = (int)position.Y / Block.BLOCK_SIZE_MULTIPLIER;
            if (x >= _Size.X || x < 0) return null;
            if (y >= _Size.Y || y < 0) return null;
            return _BlockData[x, y];
        }

        public virtual float GetLevelWidthGC()
        {
            return _Size.X * Block.BLOCK_SIZE_MULTIPLIER;
        }
    }
}
