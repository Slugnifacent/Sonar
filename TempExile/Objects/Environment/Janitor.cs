using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Sonar
{
    class Janitor: Landmark
    {
        string password;
        public bool taken;

        public Janitor(Vector2 Position, Sonar.Landmark.Type type)
            : base (Position, type) 
        {
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(sprite, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
        }

        public string GetPassword()
        {
            return password;
        }

        public void SetPassword(string key)
        {
            password = key;
            taken = false;
        }

        public void Take()
        {
            taken = true;
        }
    }
}
