using System;

namespace MiswGame2007
{
    public class BossRobot : Enemy
    {
        public enum Direction
        {
            Left = 1,
            Right
        }

        private enum State
        {
            Run = 1,
            Attack,
            BeginJump,
            Jump,
            EndJump
        }

        private const int INIT_HEALTH = 1000;

        private const double ACCELERATION_FALLING = 0.5;
        private const double MAX_FALLING_SPEED = 16;

        private static Vector SIZE1 = new Vector(40, 224);
        private static Vector SIZE2 = new Vector(40, 160);
        private static Rectangle RECTANGLE1 = new Rectangle(new Vector(44, 32), SIZE1);
        private static Rectangle RECTANGLE2 = new Rectangle(new Vector(44, 96), SIZE2);

        private Direction direction;
        private int moveCount;
        private int attackCount;
        private int attackCount2;
        private int jumpCount;
        private State currentState;
        private int animation;
        private int numDeathTicks;

        public BossRobot(GameScene game, int row, int col, Direction direction)
            : base(game, RECTANGLE1, new Vector(col * Settings.BLOCK_WDITH, row * Settings.BLOCK_WDITH), Vector.Zero, INIT_HEALTH)
        {
            velocity.X = game.Random.Next(0, 2) == 0 ? 1 : -1;
            this.direction = direction;
            moveCount = 60;
            attackCount = 0;
            attackCount2 = 120;
            jumpCount = 0;
            currentState = State.Run;
            animation = 0;
            numDeathTicks = 0;
        }

        public override void Tick(GameInput input)
        {
            if (health <= 0)
            {
                rectangle = RECTANGLE1;
                currentState = State.Attack;
                attackCount = 8;
                DeathTick(input);
                base.Tick(input);
                return;
            }

            double dx = game.Player.Center.X - Center.X;

            if (attackCount2 > 0)
            {
                attackCount2--;
                if (moveCount > 0)
                {
                    moveCount--;
                }
                else
                {
                    if (currentState == State.Run)
                    {
                        currentState = State.BeginJump;
                    }
                    else
                    {
                        moveCount = game.Random.Next(60, 120);
                        velocity.X = game.Random.Next(0, 2) == 0 ? 1 : -1;
                    }
                }
            }

            if (dx < 0)
            {
                direction = Direction.Left;
            }
            else if (dx > 0)
            {
                direction = Direction.Right;
            }

            if (currentState == State.BeginJump)
            {
                velocity.X = 0;
                rectangle = RECTANGLE2;
                if (jumpCount < 16)
                {
                    jumpCount++;
                }
                if (jumpCount == 16)
                {
                    currentState = State.Jump;
                    velocity.X = game.Random.Next(0, 2) == 0 ? -8 * game.Random.NextDouble() - 8 : 8 * game.Random.NextDouble() + 8;
                    velocity.Y = -16;
                    game.Quake(2);
                    game.PlaySound(GameSound.RobotJump);
                    rectangle = RECTANGLE1;
                }
            }

            if (currentState == State.EndJump)
            {
                rectangle = RECTANGLE1;
                if (jumpCount > 0)
                {
                    jumpCount--;
                }
                else
                {
                    moveCount = game.Random.Next(60, 120);
                    velocity.X = game.Random.Next(0, 2) == 0 ? 1 : -1;
                    currentState = State.Run;
                }
            }

            if (currentState == State.Run || currentState == State.Attack)
            {
                if (attackCount2 == 0)
                {
                    velocity.X = 0;
                    currentState = State.Attack;
                    if (attackCount < 160)
                    {
                        if (attackCount >= 32)
                        {
                            if (attackCount < 128)
                            {
                                rectangle = RECTANGLE2;
                                if (health > INIT_HEALTH / 2)
                                {
                                    if (attackCount % 32 == 0)
                                    {
                                        double rocketRotate = 6 - 0.015625 * Math.Abs(dx);
                                        if (rocketRotate < 1)
                                        {
                                            rocketRotate = 1;
                                        }
                                        if (direction == Direction.Left)
                                        {
                                            game.AddEnemyBullet(new RobotRocket(game, position + new Vector(16, 208), 180, rocketRotate));
                                            game.AddParticle(new BigExplosion(game, position + new Vector(32, 208), Vector.Zero));
                                        }
                                        else
                                        {
                                            game.AddEnemyBullet(new RobotRocket(game, position + new Vector(96, 208), 0, rocketRotate));
                                            game.AddParticle(new BigExplosion(game, position + new Vector(96, 208), Vector.Zero));
                                        }
                                        game.PlaySound(GameSound.Rocket);
                                    }
                                }
                                else
                                {
                                    if (attackCount % 24 == 0)
                                    {
                                        double rocketRotate = 6 - 0.015625 * Math.Abs(dx);
                                        if (rocketRotate < 1)
                                        {
                                            rocketRotate = 1;
                                        }
                                        if (direction == Direction.Left)
                                        {
                                            game.AddEnemyBullet(new RobotRocket(game, position + new Vector(16, 208), 180, rocketRotate));
                                            game.AddParticle(new BigExplosion(game, position + new Vector(32, 208), Vector.Zero));
                                        }
                                        else
                                        {
                                            game.AddEnemyBullet(new RobotRocket(game, position + new Vector(96, 208), 0, rocketRotate));
                                            game.AddParticle(new BigExplosion(game, position + new Vector(96, 208), Vector.Zero));
                                        }
                                        game.PlaySound(GameSound.Rocket);
                                    }
                                }
                            }
                            else
                            {
                                rectangle = RECTANGLE1;
                            }
                        }
                        attackCount++;
                    }
                    if (attackCount >= 160)
                    {
                        velocity.X = game.Random.Next(0, 2) == 0 ? 1 : -1;
                        attackCount = 0;
                        attackCount2 = game.Random.Next(30, 60);
                        currentState = State.Run;
                    }
                }
            }

            velocity.Y += ACCELERATION_FALLING;
            if (velocity.Y > MAX_FALLING_SPEED)
            {
                velocity.Y = MAX_FALLING_SPEED;
            }
            MoveBy(input, velocity);
            animation = (animation + 1) % 32;

            base.Tick(input);
        }

        public override void Draw(GraphicsDevice graphics)
        {
            int drawX = (int)Math.Round(position.X) - game.IntCameraX;
            int drawY = (int)Math.Round(position.Y) - game.IntCameraY;
            if (direction == Direction.Left)
            {
                if (currentState == State.Run)
                {
                    graphics.DrawImageFix(GameImage.Robot, 128, 256, 0, animation / 4, drawX, drawY, this);
                }
                else if (currentState == State.Attack)
                {
                    if (attackCount < 32)
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, attackCount / 4, drawX, drawY, this);
                    }
                    else if (attackCount < 128)
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, 7, drawX, drawY, this);
                    }
                    else
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, 7 - (attackCount - 128) / 4, drawX, drawY, this);
                    }
                }
                else
                {
                    if (jumpCount < 10)
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, jumpCount / 2, drawX, drawY, this);
                    }
                    else if (jumpCount < 16)
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, 4 - (jumpCount - 10) / 2, drawX, drawY, this);
                    }
                    else
                    {
                        graphics.DrawImageFix(GameImage.Robot, 128, 256, 1, 2, drawX, drawY, this);
                    }
                }
            }
            else
            {
                if (currentState == State.Run)
                {
                    graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 0, animation / 4, drawX, drawY, this);
                }
                else if (currentState == State.Attack)
                {
                    if (attackCount < 32)
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, attackCount / 4, drawX, drawY, this);
                    }
                    else if (attackCount < 128)
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, 7, drawX, drawY, this);
                    }
                    else
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, 7 - (attackCount - 128) / 4, drawX, drawY, this);
                    }
                }
                else
                {
                    if (jumpCount < 10)
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, jumpCount / 2, drawX, drawY, this);
                    }
                    else if (jumpCount < 16)
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, 4 - (jumpCount - 10) / 2, drawX, drawY, this);
                    }
                    else
                    {
                        graphics.DrawImageFixFlip(GameImage.Robot, 128, 256, 1, 2, drawX, drawY, this);
                    }
                }
            }
        }

        private void DeathTick(GameInput input)
        {
            if (Math.Abs(velocity.X) < 1)
            {
                velocity.X = 0;
            }
            else
            {
                velocity.X -= Math.Sign(velocity.X);
            }
            if (velocity.Y < 0)
            {
                velocity.Y += ACCELERATION_FALLING;
            }
            velocity.Y += ACCELERATION_FALLING;
            if (velocity.Y > MAX_FALLING_SPEED)
            {
                velocity.Y = MAX_FALLING_SPEED;
            }
            MoveBy(input, velocity);
            if (numDeathTicks < 128)
            {
                if (numDeathTicks == 0)
                {
                    game.EnemyBullets.BreakAll();
                    game.StopMusic();
                }
                if (numDeathTicks % 8 == 0)
                {
                    DoSomeExplode();
                    game.Quake(4);
                    game.PlaySound(GameSound.Shotgun);
                    damageFlash = 256;
                }
                if (numDeathTicks % 16 == 0)
                {
                    game.Flash(16);
                }
                numDeathTicks++;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    game.AddParticle(new BigExplosion(game, new Vector(Center.X - 32, Top + 0.125 * (Bottom - Top) + 0.25 * i * (Bottom - Top)), Vector.Zero));
                }
                for (int i = 0; i < 5; i++)
                {
                    game.AddParticle(new BigExplosion(game, new Vector(Center.X, Top + 0.25 * i * (Bottom - Top)), Vector.Zero));
                }
                for (int i = 0; i < 4; i++)
                {
                    game.AddParticle(new BigExplosion(game, new Vector(Center.X + 32, Top + 0.125 * (Bottom - Top) + 0.25 * i * (Bottom - Top)), Vector.Zero));
                }
                game.Quake(16);
                game.Flash(128);
                game.PlaySound(GameSound.Explode);
                SpreadDebris(64);
                game.Items.AddThing(new HealthItem(game, new Vector(32 + game.Random.NextDouble() * (game.Map.Width - 96), 32), Vector.Zero));
                Remove();
            }
        }

        public override void Blodked_Left(GameInput input)
        {
            if (currentState == State.Jump)
            {
                velocity.X = -0.25 * velocity.X;
            }
            else
            {
                velocity.X = -velocity.X;
            }
        }

        public override void  Blocked_Right(GameInput input)
        {
            if (currentState == State.Jump)
            {
                velocity.X = -0.25 * velocity.X;
            }
            else
            {
                velocity.X = -velocity.X;
            }
        }

        public override void Blocked_Bottom(GameInput input)
        {
            if (currentState == State.Jump)
            {
                velocity = Vector.Zero;
                attackCount = 8;
                attackCount2 = 0;
                jumpCount = 0;
                currentState = State.Attack;
                game.Quake(6);
                game.PlaySound(GameSound.Hammer);
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
        }
    }
}
