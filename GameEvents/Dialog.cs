using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OcarinaData {
    public class Dialog : Event
    {
        //font, text, color, rectangle, color(text), color(border), borderBuffer
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Rectangle Window { get; set; }
        public Color WindowColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderSize { get; set; }
        public Texture2D Portrait { get; set; }
        public float TextDelay { get; set; }

        public Dialog(Actor actor, Texture2D portrait, SpriteFont font, string text) {
            Font = font;
            Text = text;
            TextColor = GameSettings.TextColor;
            Window = new Rectangle(25, 400, 500, 200);
            WindowColor = GameSettings.WindowColor;
            BorderColor = GameSettings.BorderColor;
            BorderSize = GameSettings.BorderSize;
            Portrait = portrait;

            GameScreen = true;
            UpdateGame = true;
        }

        public override void Execute() {
            base.Execute();
        }

        void DelayText(float delay) {
            TextDelay = delay;
        }

        public override void Update(GameTime gameTime) {
            if (TextDelay > 0) { }

            base.Update(gameTime);
        }
    }
}
