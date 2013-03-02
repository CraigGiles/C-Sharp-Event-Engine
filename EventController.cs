/**
    Craig Giles
    For use with 'Project Ocarina RPG Engine' developed in 2010
    
    EventController.cs
    ------------------
    The Event Controller class is the driving force of the event system. This
    class will handle all of the scripts and delegate each script towards
    active, completed, and needing to start. 

    This class looks to be pretty long, and having known what I know now
    I would have figured out better ways to chunk this down. There could be
    some great reflection techniques that could be used to ensure better
    OO designs, however they would possibly be slower to develop / run vs
    this approach. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OcarinaData;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ocarina {
    public class EventController 
    {
        const string directory = @"Content\Scripts\";
        Director director;

        //list of events that have already been executed and need to be updated
        List<Script> activeScripts = new List<Script>();
        List<Script> scriptsToAdd = new List<Script>();
        List<Script> scriptsToDelete = new List<Script>();

        public EventController(Director gameDirector) {
            director = gameDirector;
        }

        public void Invoke(Actor actor, List<string> commands) {
            Script tmp = new Script();
            //breakdown teh command and initialize the correct script for it
            foreach (string s in commands) {
                string[] comp = s.Split(' ');
                MethodInfo mi = this.GetType().GetMethod(comp[0]);
                if (mi == null) throw new ArgumentNullException(comp[0] + " function doesn't exist in EventController");
                object[] parameters = { tmp, actor, comp };
                mi.Invoke(this, parameters);
            }
            scriptsToAdd.Add(tmp);
        }

        public void Update(GameTime gameTime) {
            foreach (Script script in scriptsToAdd)
                activeScripts.Add(script);

            scriptsToAdd.Clear();

            foreach (Script s in activeScripts) {
                //if the script is completed, remove the script. 
                //otherwise, run the script. When rnning the script, if 
                //the script just finished than delete.
                if (s.Completed) scriptsToDelete.Add(s);
                else if (!s.Run(gameTime)) scriptsToDelete.Add(s);

#warning HACK: If we can clean this up, do so. Currently this works only for dialog, but prompt will also be a gamescreen
                GameScreenEvent(s);
                SpawnOrRemoveActorEvent(s);
                SpawnOrRemoveItemEvent(s);
                CameraLookAtEvent(s);
            }

            foreach (Script s in scriptsToAdd) activeScripts.Add(s);
            scriptsToAdd.Clear();

            foreach (Script s in scriptsToDelete) {
                activeScripts.Remove(s);
            }
        }
        
        private void GameScreenEvent(Script s) {
            if (s.GameScreen && !s.GameScreenActive) {
                s.GameScreenActive = true;
                Dialog dialog = (Dialog)s.GetActiveEvent();
                director.ScreenController.AddScreen(
                    new DialogBox(
                        dialog.Portrait,
                        dialog.Font,
                        dialog.Text,
                        dialog.TextColor,
                        dialog.Window,
                        dialog.WindowColor,
                        dialog.BorderColor,
                        dialog.BorderSize),
                    null);
            }
        }

        private void SpawnOrRemoveActorEvent(Script s) {
            if (s.CurrentEvent is SpawnActor) {
                SpawnActor spawn = (SpawnActor)s.CurrentEvent;
                director.AddActorToGameWorld(spawn.Actor);
                spawn.Completed = true;
            }
            else if (s.CurrentEvent is RemoveActor) {
                RemoveActor remove = (RemoveActor)s.CurrentEvent;
                director.RemoveActorFromGameWorld(remove.Actor);
                remove.Completed = true;
            }
        }

        private void SpawnOrRemoveItemEvent(Script s) {
            if (s.CurrentEvent is GiveItemTo) {
                GiveItemTo spawn = (GiveItemTo)s.CurrentEvent;
                spawn.Actor.GiveItem(spawn.Item);
                spawn.Completed = true;
            }
            else if (s.CurrentEvent is RemoveItem) {
                RemoveItem remove = (RemoveItem)s.CurrentEvent;
                remove.Actor.RemoveItem(remove.ItemAsset);
                remove.Completed = true;
            }
        }

        private void CameraLookAtEvent(Script s) {
            if (s.CurrentEvent is CameraLookAt) {
                CameraLookAt c = (CameraLookAt)s.CurrentEvent;
                if (c.Scroll && !c.ActivelyScrolling) director.MapController.ScrollTo(c.Location, c.Speed);
                else if (!c.Scroll) director.MapController.LookAt(c.Location);
            }
        }

        public void CameraScrollingCompleted() {
            if (activeScripts.Count == 0) return;
            Event ev = activeScripts[activeScripts.Count - 1].GetActiveEvent();
            CameraLookAt look = null;
            if (ev is CameraLookAt) look = (CameraLookAt)ev;
            if (look != null) look.Completed = true;
        }

        public void DialogCompleted() {
            if (activeScripts.Count == 0) return;
            Event ev = activeScripts[activeScripts.Count - 1].GetActiveEvent();
            Dialog dialog = null;
            if (ev is Dialog) dialog = (Dialog)ev;
            if (dialog != null) dialog.Completed = true;

            GameScreen screen = director.ScreenController.GetTopScreen();
            director.ScreenController.RemoveScreen(screen);
        }

        #region Script Commands

        public void command_MoveTo(Script script, Actor actor, string[] comp) {
            Vector2 location = Vector2.Zero;
            float speed = 0f;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "x") location.X = float.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "y") location.Y = float.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "speed") speed = float.Parse(tmp[1]);
            }
            script.Events.Add(new MoveTo(actor, location, speed));
        }

        public void command_Wait(Script script, Actor actor, string[] comp) {
            float delay = 0;
            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "delay") delay = float.Parse(tmp[1]);
            }
            script.Events.Add(new Wait(delay));
        }

        public void command_Dialog(Script script, Actor actor, string[] comp) {
            ContentManager Content = director.ScreenController.Game.Content;

            SpriteFont font = null;
            string text = String.Empty;
            bool deleteAfterRender;
            Texture2D portrait = null;
            
            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "texture2d") portrait = Content.Load<Texture2D>(tmp[1]);
                else if (tmp[0].ToLower() == "font") font = Content.Load<SpriteFont>("Fonts/" + tmp[1]);
                else if (tmp[0].ToLower() == "text") text = tmp[1].Replace('_', ' ');
                else if (tmp[0].ToLower() == "deleteafterrender") deleteAfterRender = bool.Parse(tmp[1]);
            }

            script.Events.Add(new Dialog(actor, portrait, font, text));
        }

        public void command_Prompt(Script script, Actor actor, string[] comp) {
            throw new NotImplementedException();
            //command_Prompt Text:string Decisions:promptChoicesAssetname.script
            string text = string.Empty;
#warning TODO: command_ScriptPrompt
            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "text") text = tmp[1].Replace('_', ' ');
            }
            //script.Events.Add(new Wait(delay));
        }

        public void command_PromptChoices(Script script, Actor actor, string[] comp) {
            throw new NotImplementedException();
#warning TODO: command_PromptChoices
            //note for all text, word_word should read in-game as 'word word'
        }

        public void command_SpawnActor(Script script, Actor actor, string[] comp) {
            ContentManager Content = director.ScreenController.Game.Content;
            Actor spawn = null;
            Vector2 location = Vector2.Zero;
            string asset = string.Empty;
            string classType = string.Empty;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "x") location.X = float.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "y") location.Y = float.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "asset") asset = tmp[1];
                else if (tmp[0].ToLower() == "type") classType = tmp[1];
            }

            if (classType == "Actor") spawn = Content.Load<Actor>("Actors/" + asset);
            else if (classType == "Enemy") spawn = Content.Load<Enemy>("Actors/Enemies/" + asset);
            else if (classType == "Playable") spawn = Content.Load<Playable>("Actors/Playable/" + asset);
            else throw new Exception(classType + " Not Implemented or Null");

            spawn.Location = location;
            script.Events.Add(new SpawnActor(spawn));
        }

        public void command_RemoveActor(Script script, Actor actor, string[] comp) {
            string id = string.Empty;
            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "actorid") id = tmp[1];
            }

            script.Events.Add(new RemoveActor(director.GetActorByID(id)));
        }

        public void command_SpawnItem(Script script, Actor actor, string[] comp) {
            string itemType = string.Empty;
            string itemAsset = string.Empty;
            string actorId = string.Empty;
            Actor giveItemTo = null;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "itemtype") itemType = tmp[1];
                else if (tmp[0].ToLower() == "itemasset") itemAsset = tmp[1];
                else if (tmp[0].ToLower() == "actorid") actorId = tmp[1];
            }
            giveItemTo = director.GetActorByID(actorId);
            MethodInfo mi = this.GetType().GetMethod("Spawn" + itemType);
            if (mi == null) throw new ArgumentNullException(comp[0] + " function doesn't exist in EventController");
            object[] parameters = { script, itemAsset, giveItemTo };
            mi.Invoke(this, parameters);
        }

        public void SpawnItem(Script script, string asset, Actor actor) {
            Item item = director.ScreenController.Game.Content.Load<Item>("Items/" + asset);
            actor.GiveItem(item);
            script.Events.Add(new GiveItemTo(item, actor));
        }

        public void command_RemoveItem(Script script, Actor actor, string[] comp) {
            string itemAsset = string.Empty;
            string actorId = string.Empty;
            Actor removeItemFrom = null;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "itemasset") itemAsset = tmp[1];
                else if (tmp[0].ToLower() == "actorid") actorId = tmp[1];
            }
            removeItemFrom = director.GetActorByID(actorId);
            removeItemFrom.RemoveItem(itemAsset);
            script.Events.Add(new RemoveItem(itemAsset, removeItemFrom));
        }

        public void command_CameraLookAt(Script script, Actor actor, string[] comp) {
            bool scroll = false;
            int x = 0;
            int y = 0;
            float speed = 0;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "scroll") scroll = bool.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "x") x = int.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "y") y = int.Parse(tmp[1]);
                else if (tmp[0].ToLower() == "speed") speed = float.Parse(tmp[1]);
            }

            script.Events.Add(new CameraLookAt(new Vector2(x, y), scroll, speed));
        }

        public void command_PlayAnimation(Script script, Actor actor, string[] comp) {
            string name = string.Empty;
            string id = string.Empty;

            foreach (string str in comp) {
                string[] tmp = str.Split(':');
                if (tmp[0].ToLower() == "actorid") id = tmp[1];
                else if (tmp[0].ToLower() == "animationname") name = tmp[1];
            }

            Actor a = director.GetActorByID(id);
            script.Events.Add(new PlayAnimation(a, name));
        }

        #endregion

    }
}
