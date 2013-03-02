using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using OcarinaData;

namespace OcarinaData {
    public class Script 
    {
        const string directory = @"Content\Scripts\";
        //since a script is a series of events...
        List<Event> events = new List<Event>();
        int index;
        bool dialogCompleted = false;

        public string Name {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public Actor Instigator {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Event> Events {
            get { return events; }
            set { events = value; }
        }

        [ContentSerializerIgnore]
        public Event CurrentEvent {
            get { return events[index]; }
        }

        [ContentSerializerIgnore]
        public bool Completed {
            get { return index >= events.Count; }
        }

        [ContentSerializerIgnore]
        public bool GameScreen {
            get {
                if (index >= events.Count) return false;
                return events[index].GameScreen; 
            }
        }

        [ContentSerializerIgnore]
        public bool GameScreenActive {
            get { return events[index].GameScreenActive; }
            set { events[index].GameScreenActive = value; }
        }

        public Event GetActiveEvent() {
            return events[index];
        }

        public void DialogForceCompleted() {
            dialogCompleted = true;
        }

        public bool Run(GameTime gameTime) {
            bool shouldBreak = false;
            while (true) {
                //update all events
                if (index >= events.Count) return false;
                if (shouldBreak) return true;

                events[index].Update(gameTime);
                shouldBreak = events[index].UpdateGame;
                if (events[index].Completed) ++index;
            }
        }

        //#region Load
        //public void Load(string asset) {
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(directory + asset + ".xml");
        //}

        //protected void Load(XmlDocument doc) {
        //    List<Event> loading = new List<Event>();

        //    foreach (XmlNode root in doc.ChildNodes) {
        //        foreach (XmlNode node in root.ChildNodes) {
        //            if (node.Name == "MoveTo") loading.Add(MoveTo_Load(node));
        //        }//end child nodes 
        //    }

        //}

        //private MoveTo MoveTo_Load(XmlNode node) {
        //    Actor actor;
        //    Vector2 destination = Vector2.Zero;
        //    float speed;

        //    string actorId = node.Attributes["ActorID"].Value;

        //    actor = director.GetActorByID(actorId);
        //    destination.X = float.Parse(node.Attributes["X"].Value);
        //    destination.Y = float.Parse(node.Attributes["Y"].Value);
        //    speed = float.Parse(node.Attributes["Speed"].Value);

        //    return new MoveTo(actor, destination, speed);
        //}
        //#endregion
    }
}
