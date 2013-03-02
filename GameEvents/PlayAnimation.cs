using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OcarinaData {
    public class PlayAnimation : Event {
        public Actor Actor { get; set; }
        public string AnimationName { get; set; }

        public PlayAnimation(Actor actor, string name) {
            Actor = actor;
            AnimationName = name;
        }

        public override void Execute() {
            UpdateGame = true;
            Actor.PlayAnimation(AnimationName);
            Completed = true;
            base.Execute();
        }
    }
}
