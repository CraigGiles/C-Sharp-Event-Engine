using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace OcarinaData {
    public class MoveTo : Event
    {
        string actorId;
        Actor actor;
        Vector2 destination;
        float speed;

        public string ActorID {
            get { return actorId; }
            set { actorId = value; }
        }

        [ContentSerializerIgnore]
        public Actor Actor {
            get { return actor; }
            set { actor = value; }
        }

        public Vector2 Destination {
            get { return destination; }
            set { destination = value; }
        }

        public float Speed {
            get { return speed; }
            set { speed = value; }
        }

        public MoveTo(Actor actor, Vector2 location, float speed) {
            UpdateGame = true;

            Actor = actor;
            Destination = location;
            Speed = speed;
        }

        public override void Update(GameTime gameTime) {
            updateActorLocation();   
        }

        private void updateActorLocation() {
            Vector2 direction = getDirectionVector(actor.Location, destination);
            direction *= speed;
            Vector2 newLoc = direction + actor.Location;

            if (Math.Abs(destination.Y - newLoc.Y) <= speed)
                newLoc.Y = actor.Location.Y;
            if (Math.Abs(destination.X - newLoc.X) <= destination.X * .10f)
                newLoc.X = actor.Location.X;

            if (newLoc == actor.Location) Completed = true;
            else actor.MoveToLocation(newLoc);
        }

        private Vector2 getDirectionVector(Vector2 current, Vector2 destination) {
            Vector2 result = Vector2.Zero;
            if (current.X < destination.X) result.X = 1;
            else if (current.X > destination.X) result.X = -1;
            else result.X = 0;

            if (current.Y < destination.Y) { result.Y = 1; }
            else if (current.Y > destination.Y) { result.Y = -1;  }
            else result.Y = 0;

            result.Normalize();
            return result;
        }
    }

    //public class MoveToReader : ContentTypeReader<MoveTo> {
    //    protected override MoveTo Read(ContentReader input, MoveTo existingInstance) {
    //        MoveTo mov = existingInstance;
    //        if (mov == null)
    //            mov = new MoveTo();

    //        mov.AssetName = input.AssetName;
    //        mov.ActorID = input.ReadString();
    //        mov.Destination = input.ReadObject<Vector2>();
    //        mov.Speed = input.ReadSingle();

    //        return mov;
    //    }
    //}
}
