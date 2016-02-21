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
        private GameVector2 index;
        private GameVector2 otherHalfOfDoorIndex;
        private String password;
        public char direction;
        private String stringVal;
        private float animationInterval;
        private AnimationCollection animation;
        SpriteFont font;
        private float colorValue;
        GameTexture floorTexture;
        GameTexture doorMat;
        GameTexture lockSymbol;
        GameTexture outline;
        public GameRectangle doorHitBox;
        public GameRectangle doorThreshold;


        public Door(GameVector2 init_Pos, GameVector2 ind, String pwd, char direction, String strVal, GameVector2 otherHalfOfDoorInd)
        {
            colorValue = 1f;
            font = Game1.contentManager.Load<SpriteFont>(@"Fonts/Skyrim");
            position = init_Pos;
            floorTexture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/floorTileTest3");
            doorMat = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Floor/doorMat");
            if (pwd == null)
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                lockSymbol = Game1.contentManager.Load<GameTexture>("Textures/Objects/Environment/Door/Outlines/doorPasswordedOutlineC" + direction);
                locked = false;
            }
            else
            {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                lockSymbol = Game1.contentManager.Load<GameTexture>("Textures/Objects/Environment/Door/Outlines/doorPasswordedOutlineC" + direction);
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
            boundingBox = new GameRectangle((int)init_Pos.X + texture.Width / 2 - 1, (int)init_Pos.Y, texture.Width, texture.Height);
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
                doorHitBox = new GameRectangle((int)(getBox().X + MapUnit.MAX_SIZE / 2.7),
                                              getBox().Y - MapUnit.MAX_SIZE / 2,
                                              (int)(getBox().Width / 1.5),
                                              (int)(getBox().Height + MapUnit.MAX_SIZE));
                doorThreshold = new GameRectangle((int)(getBox().X),
                                              (int)(getBox().Y + MapUnit.MAX_SIZE / 2.5),
                                              (int)(getBox().Width),
                                              (int)(getBox().Height / 8));
            }
            else {
                doorHitBox = new GameRectangle(getBox().X - MapUnit.MAX_SIZE / 2,
                                              (int)(getBox().Y + MapUnit.MAX_SIZE / 2.7),
                                              (int)(getBox().Width + MapUnit.MAX_SIZE),
                                              (int)(getBox().Height / 1.5));
                doorThreshold = new GameRectangle((int)(getBox().X + MapUnit.MAX_SIZE / 2.5),
                                              (int)(getBox().Y),
                                              (int)(getBox().Width / 4),
                                              (int)(getBox().Height));
            }

        }

        private void InitializeAnimations()
        {
            animationInterval = 200;
            animation = new AnimationCollection(texture, boundingBox);
            animation.add("Close", 0, spriteColNum, GameVector2.Zero, animationInterval, false);
            animation.add("Open", 0, spriteColNum, new GameVector2(0, spriteHeight), animationInterval, false);
            animation.RUN("Close");
        }

        public override void Update(GameTime time)
        {
            //animation.Update(time, position);
        }

        public override void Draw(object spriteBatch)
        {

            if (Player.getInstance().GetNoise() > Player.loudness.whispering)
            {
                GameVector2 distance = Player.getInstance().position - this.position;
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
                /*GameTexture tex = Game1.contentManager.Load<GameTexture>(@"Player/player_down_fast_2");
                spriteBatch.Draw(tex, new GameRectangle(boundingBox.X - boundingBox.Width / 4,
                                                                  boundingBox.Y - boundingBox.Height / 4,
                                                                  boundingBox.Width + boundingBox.Width / 2,
                                                                  boundingBox.Height + boundingBox.Height / 2), GameColor.Yellow);*/
                spriteBatch.Draw(floorTexture, boundingBox, GameColor.White);
                
                spriteBatch.Draw(texture, boundingBox, GameColor.Gray);
                //animation.Draw(spriteBatch);
                if (password != null) {
                    if (locked)
                    {                                                
                        /*spriteBatch.Draw(lockSymbol, 
                                        position + new GameVector2(-MapUnit.MAX_SIZE/5, 0), 
                                        null, 
                                        GameColor.White, 
                                        0, 
                                        GameVector2.Zero, 
                                        new GameVector2(.75f, .75f), 
                                        SpriteEffects.None, 
                                        0);*/
                        //spriteBatch.Draw(lockSymbol, position + new GameVector2(-MapUnit.MAX_SIZE, MapUnit.MAX_SIZE / 2), GameColor.White);
                        //spriteBatch.DrawString(font, "Say the Password", position + new GameVector2(-MapUnit.MAX_SIZE, MapUnit.MAX_SIZE / 2), GameColor.Red);
                    }
                    // Play sound for when a Passworded Door is Open
                    else if (!playedPasswordSound)
                    {
                        SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                        playedPasswordSound = true;
                        Interact();
                    }
                }
            }

        }

        public void DrawDoorOnly(object spriteBatch)
        {
            if (stringVal == "D" || stringVal == "P")
            {
                if (!isBroken)
                {
                    spriteBatch.Draw(texture, boundingBox, GameColor.White);
                }  
            }
        }

        /// <summary>
        /// Steven Ekejiuba 5/17/2012
        /// Draw tiles on both entrances to a Door
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawDoorMats(object spriteBatch)
        {
            // If Door is a horizontal one, draw doormats above and below door
            if (direction == 'U' || direction == 'D')
            {
                spriteBatch.Draw(doorMat, new GameRectangle((int)(boundingBox.X + MapUnit.MAX_SIZE / 2.7), boundingBox.Y - MapUnit.MAX_SIZE / 2,
                                                        (int)(boundingBox.Width / 1.5), boundingBox.Height / 2), GameColor.White);
                spriteBatch.Draw(doorMat, new GameRectangle((int)(boundingBox.X + MapUnit.MAX_SIZE / 2.7), (int)(boundingBox.Y + (1 * MapUnit.MAX_SIZE)),
                                                          (int)(boundingBox.Width / 1.5), boundingBox.Height / 2), GameColor.White);
            }
            // If Door is a vertical one, draw doormats to the left of and right of door
            else if (direction == 'L' || direction == 'R')
            {
                spriteBatch.Draw(doorMat, new GameRectangle(boundingBox.X - MapUnit.MAX_SIZE / 2, (int)(boundingBox.Y + MapUnit.MAX_SIZE / 2.7),
                                                        boundingBox.Width / 2, (int)(boundingBox.Height / 1.5)), GameColor.White);
                spriteBatch.Draw(doorMat, new GameRectangle((int)(boundingBox.X + (1 * MapUnit.MAX_SIZE)), (int)(boundingBox.Y + MapUnit.MAX_SIZE / 2.7),
                                                          boundingBox.Width / 2, (int)(boundingBox.Height / 1.5)), GameColor.White);
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
                        texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                        outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                        UpdateBoundingBox();
                        isOpen = false;
                        animation.RUN("Close");
                        SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_CLOSE, false);
                    }
                    else {
                        //Console.Out.WriteLine("Door was closed");
                        texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorBasicO" + direction);
                        outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicO" + direction + "Outline");
                        UpdateBoundingBox();
                        opening = true;
                        animation.RUN("Open");
                        SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
                    }
                }
                // Passworded Door
                else {
                    //.Out.WriteLine("Other Door");
                    if (isOpen) {
                        //Console.Out.WriteLine("Passworded Door was open");
                        texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                        outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                        UpdateBoundingBox();
                        isOpen = false;
                        SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_CLOSE, false);
                    }
                    else {
                        //Console.Out.WriteLine("Passworded Door was closed");
                        if (!locked) {
                            texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorPasswordedO" + direction);
                            outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicO" + direction + "Outline");
                            UpdateBoundingBox();
                            opening = true;
                            SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
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
                    texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorBroken" + direction);
                    outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBroken" + direction + "Outline");
                    UpdateBoundingBox();
                    opening = true;
                    isBroken = true;
                    SoundManager.createSound(position, 100, 100, 2, SoundManager.ENVIRONMENT.DOOR_OPEN, false);
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
        public GameVector2 getIndex()
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

        public GameVector2 getOtherHalfOfDoorIndex()
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
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorBasicC" + direction);
                outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                locked = false;
            }
            else {
                texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/doorPasswordedC" + direction);
                outline = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Environment/Door/Outlines/doorBasicC" + direction + "Outline");
                locked = true;
            }
            UpdateBoundingBox();
        }

        public void DrawOutline(object spriteBatch, GameColor color)
        {
            if (!isBroken)
            {
                if (!locked)
                    spriteBatch.Draw(outline, boundingBox, color);
                else
                {
                    if (Player.getInstance().carriedPassword == null)
                    {
                        spriteBatch.Draw(outline, boundingBox, GameColor.Red);
                        spriteBatch.Draw(lockSymbol, boundingBox, GameColor.Red);
                    }
                    else
                    {
                        spriteBatch.Draw(outline, boundingBox, new GameColor(200, 110, 0, 255));
                        spriteBatch.Draw(lockSymbol, boundingBox, new GameColor(200, 100, 0, 255));
                    }
                }
            }
        }

        public void DrawLock(object spriteBatch)
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