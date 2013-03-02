using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace OcarinaData {
    public class Event : ContentObject
    {
        string name;
        bool updateGame;
        bool completed;
        bool executed;
        bool gameScreen;
        bool gameScreenActive;

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public bool UpdateGame {
            get { return updateGame; }
            set { updateGame = value; }
        }

        [ContentSerializerIgnore]
        public bool Completed {
            get { return completed; }
            set { completed = value; }
        }

        [ContentSerializerIgnore]
        public bool Executed {
            get { return executed; }
            set { executed = value; }
        }

        [ContentSerializerIgnore]
        public bool GameScreen {
            get { return gameScreen; }
            set { gameScreen = value; }
        }

        [ContentSerializerIgnore]
        public bool GameScreenActive {
            get { return gameScreenActive; }
            set { gameScreenActive = value; }
        }

        public virtual void Execute() {
            executed = true;
        }

        public virtual void Update(GameTime gameTime) {
        }


    }

    public class EventReader : ContentTypeReader<Event> {
        protected override Event Read(ContentReader input, Event existingInstance) {
            Event ev = existingInstance;
            if (ev == null)
                ev = new Event();

            ev.AssetName = input.AssetName;
            ev.Name = input.ReadString();

            return ev;
        }
    }
}
