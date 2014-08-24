using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InsaneDev.Networking;
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
        private Character _ActiveCharacter;
        private Vector2 _Size;
        private Phys LevelWallLeft;
        private Phys LevelWallRight;
        private int _WorldOffset;

        private readonly object _LockingObject = new object();

        public Vector2 Size
        {
            get { return _Size; }
        }

        public Level(Vector2 size)
        {
            _Size = size;
            _BlockData = new Block[(int)_Size.X, (int)_Size.Y];

            LevelWallLeft = new Phys(Core.CreateRectangle(new Vector2(16, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER) * 0.02f, new Vector2(-32, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER * 0.5f) * 0.01f));
            LevelWallLeft.PhysicsFixture.Body.BodyType = BodyType.Static;

            LevelWallRight = new Phys(Core.CreateRectangle(new Vector2(16, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER) * 0.02f, new Vector2(_Size.X * Block.BLOCK_SIZE_MULTIPLIER, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER * 0.5f) * 0.01f));
            LevelWallRight.PhysicsFixture.Body.BodyType = BodyType.Static;
        }

        public virtual void Draw()
        {
            lock (_LockingObject)
            {
                for (int x = 0; x < _Size.X; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        if (_BlockData[x, y] == null) continue;
                        if (_BlockData[x, y].IsDisposed())
                        {
                            _BlockData[x, y] = null;
                            continue;
                        }
                        _BlockData[x, y].Draw();
                    }
                }

                _ActiveCharacter.Draw();
            }

        }
        public void OffsetWorld(int i)
        {
            lock (_LockingObject)
            {
                int difference = i - _WorldOffset;
                _WorldOffset = i;
                Vector2 start = Vector2.Zero + (Vector2.UnitX * i * _Size.X * Block.BLOCK_SIZE_MULTIPLIER);

                LevelWallLeft.PhysicsFixture.Body.Position = new Vector2(start.X - 32, _Size.Y * Block.BLOCK_SIZE_MULTIPLIER * 0.5f) * 0.01f;
                LevelWallRight.PhysicsFixture.Body.Position = new Vector2(start.X + (_Size.X * Block.BLOCK_SIZE_MULTIPLIER), _Size.Y * Block.BLOCK_SIZE_MULTIPLIER * 0.5f) * 0.01f;


                for (int x = 0; x < _Size.X; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        if (_BlockData[x, y] == null) continue;
                        _BlockData[x, y].SetPosition(start + new Vector2(x * Block.BLOCK_SIZE_MULTIPLIER, y * Block.BLOCK_SIZE_MULTIPLIER));
                    }
                }
                if (difference != 0)
                {
                    _ActiveCharacter.Position += difference * (Vector2.UnitX * _Size.X * Block.BLOCK_SIZE_MULTIPLIER);
                }
            }
        }

        public virtual void Update()
        {
            lock (_LockingObject)
            {
                for (int x = 0; x < _Size.X; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        if (_BlockData[x, y] == null) continue;
                        if (_BlockData[x, y].IsDisposed())
                        {
                            _BlockData[x, y] = null;
                            continue;
                        }
                        _BlockData[x, y].Update();
                    }
                }

                _ActiveCharacter.Update();
                GameCore.PrimaryCamera.Position = _ActiveCharacter.Position - (GameCore.ScreenSize * 0.5f);
            }
        }

        public virtual void PlaceBlock(Block.BlockTypes type, Vector2 position)
        {
            lock (_LockingObject)
            {
                int x = (int)position.X;
                int y = (int)position.Y;
                if (x >= _Size.X || x < 0) throw new ArgumentOutOfRangeException("position");
                if (y >= _Size.Y || y < 0) throw new ArgumentOutOfRangeException("position");

                _BlockData[x, y] = new Block(type, position * Block.BLOCK_SIZE_MULTIPLIER);
            }
        }

        public virtual void SetActiveCharacter(Character character)
        {
            lock (_LockingObject)
            {

                _ActiveCharacter = character;
                character.SetLevel(this);
            }
        }

        public virtual Character GetActiveCharacter()
        {
            lock (_LockingObject)
            {
                return _ActiveCharacter;
            }
        }

        public virtual Block BlockAtGC(Vector2 position)
        {
            lock (_LockingObject)
            {
                int x = (int)position.X / Block.BLOCK_SIZE_MULTIPLIER;
                int y = (int)position.Y / Block.BLOCK_SIZE_MULTIPLIER;
                if (x >= _Size.X || x < 0) return null;
                if (y >= _Size.Y || y < 0) return null;
                return _BlockData[x, y];
            }
        }

        public virtual float GetLevelWidthGC()
        {
            lock (_LockingObject)
            {
                return _Size.X * Block.BLOCK_SIZE_MULTIPLIER;
            }
        }

        public virtual void ShiftRight()
        {
            lock (_LockingObject)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[(int)_Size.X - 1, y] != null)
                    {
                        _BlockData[(int)_Size.X - 1, y].Dispose();
                        _BlockData[(int)_Size.X - 1, y] = null;
                    }
                }
                for (int x = ((int)_Size.X - 1); x > 0; x--)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        _BlockData[x, y] = _BlockData[x - 1, y];
                    }
                }
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[0, y] != null)
                    {
                        _BlockData[0, y].Dispose();
                        _BlockData[0, y] = null;
                    }
                }
                OffsetWorld(_WorldOffset);
            }
        }
        public virtual void ShiftLeft()
        {
            lock (_LockingObject)
            {
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[0, y] != null)
                    {
                        _BlockData[0, y].Dispose();
                        _BlockData[0, y] = null;
                    }
                }
                for (int x = 0; x < _Size.X - 1; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        _BlockData[x, y] = _BlockData[x + 1, y];
                    }
                }
                for (int y = 0; y < _Size.Y; y++)
                {
                    if (_BlockData[(int)_Size.X - 1, y] != null)
                    {
                        _BlockData[(int)_Size.X - 1, y].Dispose();
                        _BlockData[(int)_Size.X - 1, y] = null;
                    }
                }
                OffsetWorld(_WorldOffset);
            }
        }

        public virtual Packet ToPacket()
        {
            lock (_LockingObject)
            {
                Packet p = new Packet(Multiplayer.Manager.PID_WORLDDATAFULL);
                p.AddInt(_WorldOffset);
                int i = 0;
                for (int x = 0; x < _Size.X; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        if (_BlockData[x, y] != null) i++;
                    }
                }
                p.AddInt(i);
                for (int x = 0; x < _Size.X; x++)
                {
                    for (int y = 0; y < _Size.Y; y++)
                    {
                        if (_BlockData[x, y] == null) continue;
                        p.AddInt(x);
                        p.AddInt(y);
                        p.AddInt((int)_BlockData[x, y].BlockType);
                    }
                }
                return p;
            }
        }
    }
}
