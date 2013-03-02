using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OcarinaData {
    public class SpawnActor : Event {
        public Actor Actor { get; set; }
        public SpawnActor(Actor actor) {
            UpdateGame = true;
            Actor = actor;
        }

        public override void Execute() {
            Completed = true;
            base.Execute();
        }
    }
}
