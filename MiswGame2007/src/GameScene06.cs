using System;
using System.Collections.Generic;

namespace MiswGame2007
{
    public class GameScene06 : GameScene
    {
        public GameScene06(StageData data)
            : base(data)
        {
        }

        public GameScene06(StageData data, PlayerState playerState)
            : base(data, playerState)
        {
        }

        public override void Tick(GameInput input)
        {
            if (Ticks == 0)
            {
                PlayMusic(GameMusic.Stage2);
            }

            base.Tick(input);
        }

        public override void Draw(GraphicsDevice graphics)
        {
            base.Draw(graphics);
        }

        public override void DrawBackground(GraphicsDevice graphics)
        {
            graphics.DrawImage(GameImage.Background2, 1024, 512, IntBackgroundX, IntBackgroundY);
        }
    }
}
