using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OcarinaData {
    public class CameraLookAt : Event {
        public bool Scroll { get; set; }
        public Vector2 Location { get; set; }
        public float Speed { get; set; }
        public bool ActivelyScrolling { get; set; }

        public CameraLookAt(Vector2 location, bool scroll, float speed) {
            UpdateGame = true;
            Scroll = scroll;
            Location = location;
            Speed = speed;
            ActivelyScrolling = false;
        }

        public override void Execute() {
            ActivelyScrolling = true;
            base.Execute();
        }

    }
}
