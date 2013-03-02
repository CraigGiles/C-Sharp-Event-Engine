using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OcarinaData {
    public class Wait : Event
    {
        float delay;
        public float Delay {
            get { return delay; }
            set { delay = value; }
        }

        public Wait(float delay) {
            UpdateGame = true;
            Delay = delay;
        }

        public override void Execute() {
            UpdateGame = true;
            base.Execute();
        }

        public override void Update(GameTime gameTime) {
            delay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (delay < 0) Completed = true;
            else Completed = false;
        }
    }
}
