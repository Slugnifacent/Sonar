﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace Sonar
{
    public class WalkieTalkie : Throwable
    {

        public bool collected;
        public bool on = true;
        public bool makingNoise;
        public WalkieTalkie()
        {
        }
        public WalkieTalkie(GameVector2 pos, ContentManager content)
        {
            color = GameColor.White;
            position = pos;
            texture = content.Load<GameTexture>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            boundingBox = new GameRectangle((int)position.X , (int)position.Y , (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            readyForPickUp = true;
            health = 8;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            on = true;
        }
        public WalkieTalkie(GameVector2 pos, ContentManager content,bool Collected)
        {
            color = GameColor.White;
            position = pos;
            texture = content.Load<GameTexture>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            boundingBox = new GameRectangle((int)position.X , (int)position.Y , (int)(MapUnit.MAX_SIZE) + 2, (int)(MapUnit.MAX_SIZE) + 2);
            readyForPickUp = true;
            health = 8;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            collected = Collected;
            on = true;
        }
        public WalkieTalkie(GameVector2 pos, ContentManager content, bool Collected, float Health)
        {
            color = GameColor.White;
            position = pos;
            texture = content.Load<GameTexture>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            //boundingBox = new GameRectangle((int)(position.X - MapUnit.MAX_SIZE / 2 - 1), (int)(position.Y - MapUnit.MAX_SIZE / 2 - 1), (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            boundingBox = new GameRectangle((int)position.X, (int)position.Y, (int)(MapUnit.MAX_SIZE) + 2, (int)(MapUnit.MAX_SIZE) + 2);
            readyForPickUp = true;
            health = Health;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            collected = Collected;
            on = true;
        }

        public override void Draw(object spriteBatch)
        {
            spriteBatch.Draw(texture, boundingBox, color);
            //Console.Out.WriteLine("Walkie BoundingBox: " + boundingBox);
        }

        public void createSound(float volume) {
            color.G = 0;
            // SoundManager.playSoundFX(position, volume, volume,1,1,500,null);
            makingNoise = true;
        }

        public void WalkieUpdate()
        {
            updateBoundingBox(position);
            if (on)
            {
                if (!collected)
                {
                    createSound(VoiceEngine.getInstance().VOLUME * 1000);
                }
                else
                {
                    createSound(3);
                }
            }
            else
            {
                makingNoise = false;
                color.G = 255;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
