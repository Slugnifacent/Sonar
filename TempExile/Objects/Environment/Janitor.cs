﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace Sonar
{
    class Janitor: Landmark
    {
        string password;
        public bool taken;

        public Janitor(GameVector2 Position, Sonar.Landmark.Type type)
            : base (Position, type) 
        {
        }

        public override void Draw(object batch)
        {
            batch.Draw(sprite, position, null, GameColor.White, 0, GameVector2.Zero, scale, SpriteEffects.None, 1);
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
