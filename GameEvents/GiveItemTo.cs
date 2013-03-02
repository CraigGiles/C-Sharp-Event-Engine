using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OcarinaData {
    public class GiveItemTo : Event {
        public Item Item { get; set; }
        public Actor Actor { get; set; }

        public GiveItemTo(Item item, Actor actor) {
            UpdateGame = false;
            Item = item;
            Actor = actor;
        }

        public override void Execute() {
            Completed = true;
            base.Execute();
        }
    }
}
