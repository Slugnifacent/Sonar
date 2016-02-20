using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class Door : Environment
    {
        public bool isOpen;
        public bool opening;
        public bool locked;
        public bool isOccupied;
        public bool isBroken;
        private bool playedPasswordSound; // Played sound for opening passworded door
        private Vector2 index;
        private Vector2 otherHalfOfDoorIndex;
        private String password;
        public char direction;
        private String stringVal;
        private float animationInterval;
        private AnimationCollection animation;
        SpriteFont font;
        private float colorValue;
        Texture2D floorTexture;
        Texture2D doorMat;
        Texture2D lockSymbol;
        Texture2D outline;
        public Rectangle doorHitBox;
        public Rectangle doorThreshold;


        public Door(Vector2 init_Pos, Vector2 ind, String pwd, char direction, String strVal, Vector2 otherHalfOfDoorInd)
        {
            colorValue = 1f;
            font = Game1.contentManager.Load<SpriteFont>(@"Fonts/Skyrim");
            position = init_Pos;
            floorTexture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Floor/floorTileTest3");
            doorMat = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Floor/doorMat");
            if (pwd == null)
            {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                lockSymbol = Game1.contentManager.Load<Texture2D>("Textures/Objects/Environment/Door/Outlines/doorPasswordedOutlineC" + direction);
                locked = false;
            }
            else
            {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                lockSymbol = Game1.contentManager.Load<Texture2D>("Textures/Objects/Environment/Door/Outlines/doorPasswordedOutlineC" + direction);
                locked = true;
            }
            if (direction == 'L' || direction == 'D')
            {
                position.X += texture.Width / 2 + 1;
                position.Y += texture.Height / 2 + 1;
            }
            else if (direction == 'R')
            {
                position.X += texture.Width / 2 + 1;
            }
            else if (direction == 'U')
            {
                position.Y += texture.Height / 2 + 1;
            }
            boundingBox = new Rectangle((int)init_Pos.X + texture.Width / 2 - 1, (int)init_Pos.Y, texture.Width, texture.Height);
            InitializeAnimations();
            isOpen = false;
            opening = false;
            isOccupied = false;
            playedPasswordSound = false;
            index = ind;
            password = pwd;
            this.direction = direction;
            stringVal = strVal;
            otherHalfOfDoorIndex = otherHalfOfDoorInd;
            UpdateBoundingBox();
            if (direction == 'U' || direction == 'D') {
                doorHitBox = new Rectangle((int)(getBox().X + MapUnit.MAX_SIZE / 2.7),
                                              getBox().Y - MapUnit.MAX_SIZE / 2,
                                              (int)(getBox().Width / 1.5),
                                              (int)(getBox().Height + MapUnit.MAX_SIZE));
                doorThreshold = new Rectangle((int)(getBox().X),
                                              (int)(getBox().Y + MapUnit.MAX_SIZE / 2.5),
                                              (int)(getBox().Width),
                                              (int)(getBox().Height / 8));
            }
            else {
                doorHitBox = new Rectangle(getBox().X - MapUnit.MAX_SIZE / 2,
                                              (int)(getBox().Y + MapUnit.MAX_SIZE / 2.7),
                                              (int)(getBox().Width + MapUnit.MAX_SIZE),
                                              (int)(getBox().Height / 1.5));
                doorThreshold = new Rectangle((int)(getBox().X + MapUnit.MAX_SIZE / 2.5),
                                              (int)(getBox().Y),
                                              (int)(getBox().Width / 4),
                                              (int)(getBox().Height));
            }

        }

        private void InitializeAnimations()
        {
            animationInterval = 200;
            animation = new AnimationCollection(texture, boundingBox);
            animation.add("Close", 0, spriteColNum, Vector2.Zero, animationInterval, false);
            animation.add("Open", 0, spriteColNum, new Vector2(0, spriteHeight), animationInterval, false);
            animation.RUN("Close");
        }

        public override void Update(GameTime time)
        {
            //animation.Update(time, position);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            if (Player.getInstance().GetNoise() > Player.loudness.whispering)
            {
                Vector2 distance = Player.getInstance().position - this.position;
                float distanceValue = (float)Math.Pow((double)distance.X, 2) + (float)Math.Pow((double)distance.Y, 2);
                distanceValue = 0.015f - (distanceValue / 10000000f);

                if (colorValue + 0.25 < 1) colorValue += distanceValue;
            }
            else
            {
                if (colorValue > 0) colorValue -= 0.01f;
            }

            if (stringVal == "D" || stringVal == "P")
            {
                /*Texture2D tex = Game1.contentManager.Load<Texture2D>(@"Player/player_down_fast_2");
                spriteBatch.Draw(tex, new Rectangle(boundingBox.X - boundingBox.Width / 4,
                                                                  boundingBox.Y - boundingBox.Height / 4,
                                                                  boundingBox.Width + boundingBox.Width / 2,
                                                                  boundingBox.Height + boundingBox.Height / 2), Color.Yellow);*/
                spriteBatch.Draw(floorTexture, boundingBox, Color.White);
                
                spriteBatch.Draw(texture, boundingBox, Color.Gray);
                //animation.Draw(spriteBatch);
                if (password != null) {
                    if (locked)
                    {                                                
                        /*spriteBatch.Draw(lockSymbol, 
                                        position + new Vector2(-MapUnit.MAX_SIZE/5, 0), 
                                        null, 
                                        Color.White, 
                                        0, 
                                        Vector2.Zero, 
                                        new Vector2(.75f, .75f), 
                                        SpriteEffects.None, 
                                        0);*/
                        //spriteBatch.Draw(lockSymbol, position + new Vector2(-MapUnit.MAX_SIZE, MapUnit.MAX_SIZE / 2), Color.White);
                        //spriteBatch.DrawString(font, "Say the Password", position + new Vector2(-MapUnit.MAX_SIZE, MapUnit.MAX_SIZE / 2), Color.Red);
                    }
                    // Play sound for when a Passworded Door is Open
                    else if (!playedPasswordSound)
                    {
                        SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                        playedPasswordSound = true;
                        Interact();
                    }
                }
            }

        }

        public void DrawDoorOnly(SpriteBatch spriteBatch)
        {
            if (stringVal == "D" || stringVal == "P")
            {
                if (!isBroken)
                {
                    spriteBatch.Draw(texture, boundingBox, Color.White);
                }  
            }
        }

        /// <summary>
        /// Steven Ekejiuba 5/17/2012
        /// Draw tiles on both entrances to a Door
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawDoorMats(SpriteBatch spriteBatch)
        {
            // If Door is a horizontal one, draw doormats above and below door
            if (direction == 'U' || direction == 'D')
            {
                spriteBatch.Draw(doorMat, new Rectangle((int)(boundingBox.X + MapUnit.MAX_SIZE / 2.7), boundingBox.Y - MapUnit.MAX_SIZE / 2,
                                                        (int)(boundingBox.Width / 1.5), boundingBox.Height / 2), Color.White);
                spriteBatch.Draw(doorMat, new Rectangle((int)(boundingBox.X + MapUnit.MAX_SIZE / 2.7), (int)(boundingBox.Y + (1 * MapUnit.MAX_SIZE)),
                                                          (int)(boundingBox.Width / 1.5), boundingBox.Height / 2), Color.White);
            }
            // If Door is a vertical one, draw doormats to the left of and right of door
            else if (direction == 'L' || direction == 'R')
            {
                spriteBatch.Draw(doorMat, new Rectangle(boundingBox.X - MapUnit.MAX_SIZE / 2, (int)(boundingBox.Y + MapUnit.MAX_SIZE / 2.7),
                                                        boundingBox.Width / 2, (int)(boundingBox.Height / 1.5)), Color.White);
                spriteBatch.Draw(doorMat, new Rectangle((int)(boundingBox.X + (1 * MapUnit.MAX_SIZE)), (int)(boundingBox.Y + MapUnit.MAX_SIZE / 2.7),
                                                          boundingBox.Width / 2, (int)(boundingBox.Height / 1.5)), Color.White);
            }
        }

        public virtual void Interact()
        {
            //Console.Out.WriteLine("Interacting");
            // Basic Door
            if (!isBroken) {
                if (password == null) {
                    if (isOpen) {
                        //Console.Out.WriteLine("Door was open");
                        texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                        outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                        UpdateBoundingBox();
                        isOpen = false;
                        animation.RUN("Close");
                        SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_CLOSE, false);
                    }
                    else {
                        //Console.Out.WriteLine("Door was closed");
                        texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorBasicO" + direction);
                        outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicO" + direction + "Outline");
                        UpdateBoundingBox();
                        opening = true;
                        animation.RUN("Open");
                        SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                    }
                }
                // Passworded Door
                else {
                    //.Out.WriteLine("Other Door");
                    if (isOpen) {
                        //Console.Out.WriteLine("Passworded Door was open");
                        texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                        outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                        UpdateBoundingBox();
                        isOpen = false;
                        SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_CLOSE, false);
                    }
                    else {
                        //Console.Out.WriteLine("Passworded Door was closed");
                        if (!locked) {
                            texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorPasswordedO" + direction);
                            outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicO" + direction + "Outline");
                            UpdateBoundingBox();
                            opening = true;
                            SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                        }
                    }
                }
            }
        }

        public virtual void Break() {
            // Basic Door
            if (password == null) {
                if (!isOpen) {
                    //Console.Out.WriteLine("Door was closed");
                    texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorBroken" + direction);
                    outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBroken" + direction + "Outline");
                    UpdateBoundingBox();
                    opening = true;
                    isBroken = true;
                    SoundManager.GetInstance().createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                }
            }
            // Passworded Door doesn't know
        }

        public void UpdateBoundingBox()
        {
            //Console.Out.WriteLine(stringVal);
            if (texture != null)
            {
                if (stringVal == "D" || stringVal == "P")
                {
                    if (direction == 'L')
                    {
                        boundingBox.Width = (int)MapUnit.MAX_SIZE;
                        boundingBox.Height = (int)MapUnit.MAX_SIZE * 2;
                        boundingBox.X = (int)position.X - texture.Width / 2 - 1;
                        boundingBox.Y = (int)position.Y - texture.Height / 2 - 1;
                    }
                    else if (direction == 'D')
                    {
                        boundingBox.Width = (int)MapUnit.MAX_SIZE * 2;
                        boundingBox.Height = (int)MapUnit.MAX_SIZE;
                        boundingBox.X = (int)position.X - texture.Width / 2 - 1;
                        boundingBox.Y = (int)position.Y - texture.Height / 2 - 1;
                    }
                    else if (direction == 'R')
                    {
                        boundingBox.Width = (int)MapUnit.MAX_SIZE;
                        boundingBox.Height = (int)MapUnit.MAX_SIZE * 2;
                        boundingBox.X = (int)position.X - texture.Width / 2 - 1;
                        boundingBox.Y = (int)position.Y - boundingBox.Height / 2;
                    }
                    else if (direction == 'U')
                    {
                        boundingBox.Width = (int)MapUnit.MAX_SIZE * 2;
                        boundingBox.Height = (int)MapUnit.MAX_SIZE;
                        boundingBox.X = (int)position.X - boundingBox.Width / 2;
                        boundingBox.Y = (int)position.Y - texture.Height / 2 - 1;
                    }
                }
                else
                {
                    boundingBox.Width = 0;
                    boundingBox.Height = 0;
                }
            }
        }

        public void ApplyFloorTexture(Map map)
        {
            // Assign the proper floor tile for the current room
            floorTexture = map.AssignRoomTile((int)getIndex().X, (int)getIndex().Y).GetTexture();
        }

        // Returns map x/y values for Door
        public Vector2 getIndex()
        {
            return index;
        }

        public char getDirection()
        {
            return direction;
        }

        public String getPassword()
        {
            return password;
        }

        public Vector2 getOtherHalfOfDoorIndex()
        {
            return otherHalfOfDoorIndex;
        }

        public void SetPassword (string key)
        {
            password = key;
        }

        public void reset() {
            isOpen = false;
            opening = false;
            isOccupied = false;
            isBroken = false;
            playedPasswordSound = false;
            if (password == null) {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                locked = false;
            }
            else {
                texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                outline = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                locked = true;
            }
            UpdateBoundingBox();
        }

        public void DrawOutline(SpriteBatch spriteBatch, Color color)
        {
            if (!isBroken)
            {
                if (!locked)
                    spriteBatch.Draw(outline, boundingBox, color);
                else
                {
                    if (Player.getInstance().carriedPassword == null)
                    {
                        spriteBatch.Draw(outline, boundingBox, Color.Red);
                        spriteBatch.Draw(lockSymbol, boundingBox, Color.Red);
                    }
                    else
                    {
                        spriteBatch.Draw(outline, boundingBox, new Color(200, 110, 0, 255));
                        spriteBatch.Draw(lockSymbol, boundingBox, new Color(200, 100, 0, 255));
                    }
                }
            }
        }

        public void DrawLock(SpriteBatch spriteBatch)
        {

        }

        #region Testing

        /// <summary>
        /// Chris Peterson - 1/19/12
        /// </summary>
        /// <returns></returns>
        public new string toString()
        {
            return stringVal;
        }
        #endregion
    }
}