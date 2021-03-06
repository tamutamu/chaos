using System;

namespace MiswGame2007
{
    public class Debris : Particle
    {
        private int animation;
        private int type;

        public Debris(GameScene game, Vector position, Vector velocity, int type)
            : base(game, position, velocity)
        {
            animation = 0;
            this.type = type;
        }

        public override void Tick()
        {
            velocity.Y += 0.5;
            base.Tick();
            animation++;
            if (animation == 64)
            {
                Remove();
            }
        }

        public override void Draw(GraphicsDevice graphics)
        {
            int drawX = (int)Math.Round(position.X) - game.IntCameraX - 8;
            int drawY = (int)Math.Round(position.Y) - game.IntCameraY - 8;
            if (animation < 48)
            {
                graphics.DrawImage(GameImage.Debris, 16, 16, type, animation / 2 % 8, drawX, drawY);
            }
            else
            {
                graphics.DrawImageAlpha(GameImage.Debris, 16, 16, type, animation / 2 % 8, drawX, drawY, 255 - 16 * (animation - 48));
            }
        }
    }
}
