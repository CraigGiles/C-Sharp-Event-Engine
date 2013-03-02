using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OcarinaData {
    public class RemoveActor : Event {
        public Actor Actor { get; set; }
        public RemoveActor(Actor actor) {
            UpdateGame = false;
            Actor = actor;
        }

        public override void Execute() {
            Completed = true;
            base.Execute();
        }
    }
}
