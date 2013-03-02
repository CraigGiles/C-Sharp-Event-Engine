using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OcarinaData {
    public class RemoveItem : Event {
        public Actor Actor { get; set; }
        public string ItemAsset { get; set; }
        public RemoveItem(string asset, Actor actor) {
            UpdateGame = false;
            ItemAsset = asset;
            Actor = actor;
        }

        public override void Execute() {
            Completed = true;
            base.Execute();
        }
    }
}
