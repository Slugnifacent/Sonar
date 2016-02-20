using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public WalkieTalkie(Vector2 pos, ContentManager content)
        {
            color = Color.White;
            position = pos;
            texture = content.Load<Texture2D>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            boundingBox = new Rectangle((int)position.X , (int)position.Y , (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            readyForPickUp = true;
            health = 8;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            on = true;
        }
        public WalkieTalkie(Vector2 pos, ContentManager content,bool Collected)
        {
            color = Color.White;
            position = pos;
            texture = content.Load<Texture2D>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            boundingBox = new Rectangle((int)position.X , (int)position.Y , (int)(MapUnit.MAX_SIZE) + 2, (int)(MapUnit.MAX_SIZE) + 2);
            readyForPickUp = true;
            health = 8;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            collected = Collected;
            on = true;
        }
        public WalkieTalkie(Vector2 pos, ContentManager content, bool Collected, float Health)
        {
            color = Color.White;
            position = pos;
            texture = content.Load<Texture2D>(@"Textures/Objects/Entity/WalkieTalkie/idle");
            //boundingBox = new Rectangle((int)(position.X - MapUnit.MAX_SIZE / 2 - 1), (int)(position.Y - MapUnit.MAX_SIZE / 2 - 1), (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            boundingBox = new Rectangle((int)position.X, (int)position.Y, (int)(MapUnit.MAX_SIZE) + 2, (int)(MapUnit.MAX_SIZE) + 2);
            readyForPickUp = true;
            health = Health;
            speed = 4;
            weight = 4;
            orientation.Normalize();
            collected = Collected;
            on = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, boundingBox, color);
            //Console.Out.WriteLine("Walkie BoundingBox: " + boundingBox);
        }

        public void createSound(float volume) {
            color.G = 0;
            // SoundManager.GetInstance().playSoundFX(position, volume, volume,1,1,500,null);
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
