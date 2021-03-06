using System;
using System.Collections.Generic;

namespace MiswGame2007
{
    public class GameScene14 : GameScene
    {
        int skyColorCount;

        public GameScene14(StageData data)
            : base(data)
        {
            Init();
        }

        public GameScene14(StageData data, PlayerState playerState)
            : base(data, playerState)
        {
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < 256; i++)
            {
                AddParticle(new Rain(this, false));
            }
            for (int i = 0; i < 256; i++)
            {
                AddParticle(new Rain(this, true));
            }
            skyColorCount = 255;
        }

        public override void Tick(GameInput input)
        {
            if (Ticks == 0)
            {
                PlayMusic(GameMusic.Stage3);
            }

            base.Tick(input);

            int targetColorCount = (int)Math.Round(256.0 * (Map.Width - Player.Center.X) / Map.Width);
            if (targetColorCount < 0) targetColorCount = 0;
            else if (targetColorCount > 255) targetColorCount = 255;
            if (Ticks % 4 == 0)
            {
                if (skyColorCount < targetColorCount) skyColorCount++;
                else if (skyColorCount > targetColorCount) skyColorCount--;
            }
        }

        public override void Draw(GraphicsDevice graphics)
        {
            base.Draw(graphics);
        }

        public override void DrawBackground(GraphicsDevice graphics)
        {
            int color = 64 + skyColorCount * 3 / 4;
            graphics.DrawImage(GameImage.Background3, 1024, 512, IntBackgroundX, IntBackgroundY, color, color, color);
        }

        public override void DrawMap(GraphicsDevice graphics)
        {
            int color = 128 + skyColorCount / 2;
            Map.Draw(graphics, color, color, color);
        }
    }
}
