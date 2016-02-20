using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sonar
{
    /// <summary>
    /// Travis Carlson
    /// 2/7/12
    /// The Exit Tile representing the end of the level.
    /// </summary>

    public enum DoorPosition : sbyte { Left, Right, Top, Bottom, Unclassified }

    public class Exit : Environment
    {

        public DoorPosition doorPosition;
        Color exitColor;
        float range;
        static Cue ElevatorMusic;
        static Cue ElevatorClose;
        static Cue ElevatorOpen;
        static AudioEmitter ElevatorEmitter;
        static float elevatorVolume;
        static private bool isLocked;
        static private bool isOpen;
        public bool playerNear;
        private int soundDelay;
        private int soundDelayReset = 60;
        static float elevatorLowestVolume;
        private Texture2D outlineOpen;
        private Texture2D outlineClosed;
        public Texture2D underTile;
        private GameVector2 index;


        public Exit(GameVector2 init_Pos)
        {
            doorPosition = DoorPosition.Unclassified;

            exitColor = new Color(255f, 255f, 255f);

            range = 125;
            elevatorVolume = elevatorLowestVolume = 0;
            position = init_Pos;
            texture = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTile");
            outlineOpen = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutline");
            ElevatorMusic = SoundManager.GetInstance().getCue(SoundManager.ELEVATOR.MUSIC);
            ElevatorClose = SoundManager.GetInstance().getCue(SoundManager.ELEVATOR.CLOSE);
            ElevatorOpen = SoundManager.GetInstance().getCue(SoundManager.ELEVATOR.OPEN);
            ElevatorMusic.SetVariable("Volume", elevatorVolume);

            boundingBox = new GameRectangle((int)init_Pos.X, (int)init_Pos.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);
            ElevatorEmitter = new AudioEmitter();
            SoundManager.GetInstance().cueUpdate(ref ElevatorMusic, ElevatorEmitter);
            isLocked = true;
            playerNear = false;
            soundDelay = 0;

            index = init_Pos / MapUnit.MAX_SIZE;
            underTile = null;
        }

        public static void Update(GameTime gameTime, GameVector2 playerPos, ref List<Exit> exits, bool isLockdownPressed)
        {
            SoundManager.GetInstance().cueUpdate(ref ElevatorOpen, ElevatorEmitter);
            SoundManager.GetInstance().cueUpdate(ref ElevatorClose, ElevatorEmitter);

            isOpen = false;

            if (isLockdownPressed)
            {
                isLocked = false;
                elevatorLowestVolume = 80;
                for (int i = 0; i + 1 < exits.Count; i += 2)
                {
                    float a = exits[i].position.X - playerPos.X;
                    float b = exits[i].position.Y - playerPos.Y;


                    ElevatorEmitter.Position = new Vector3(exits[i].position.X, 0, exits[i].position.Y);


                    float dist = (float)Math.Sqrt(a * a + b * b);
                    if (dist <= exits[i].range)
                    {
                        if (exits[i].doorPosition == DoorPosition.Top)
                        {
                            if (exits[i].boundingBox.Height == MapUnit.MAX_SIZE)
                            {
                                SoundManager.GetInstance().playSoundFX(SoundManager.ELEVATOR.DING);
                                SoundManager.GetInstance().Play3D(ref ElevatorOpen, ElevatorEmitter, SoundManager.ELEVATOR.OPEN);
                                SoundManager.GetInstance().Stop(ref ElevatorClose);
                            }
                            if (elevatorVolume < 100) elevatorVolume += 5f;
                            if (exits[i].boundingBox.Height > 0)
                            {
                                exits[i].boundingBox.Height--;
                                exits[i + 1].boundingBox.Height--;
                                exits[i + 1].boundingBox.Y++;
                                isOpen = true;
                            }
                        }
                        if (exits[i].doorPosition == DoorPosition.Bottom)
                        {
                            if (exits[i].boundingBox.Height > 0)
                            {
                                exits[i].boundingBox.Height--;
                                exits[i].boundingBox.Y++;
                                exits[i + 1].boundingBox.Height--;
                                isOpen = true;
                            }
                        }
                        if (exits[i].doorPosition == DoorPosition.Left)
                        {
                            if (exits[i].boundingBox.Width == MapUnit.MAX_SIZE)
                            {
                                SoundManager.GetInstance().playSoundFX(SoundManager.ELEVATOR.DING);
                                SoundManager.GetInstance().Play3D(ref ElevatorOpen, ElevatorEmitter, SoundManager.ELEVATOR.OPEN);
                                SoundManager.GetInstance().Stop(ref ElevatorClose);
                            }
                            if (elevatorVolume < 100) elevatorVolume += 5f;
                            if (exits[i].boundingBox.Width > 0)
                            {
                                exits[i].boundingBox.Width--;
                                exits[i + 1].boundingBox.Width--;
                                exits[i + 1].boundingBox.X++;
                                isOpen = true;
                            }
                        }
                        if (exits[i].doorPosition == DoorPosition.Right)
                        {
                            if (exits[i].boundingBox.Width > 0)
                            {
                                exits[i].boundingBox.Width--;
                                exits[i].boundingBox.X++;
                                exits[i + 1].boundingBox.Width--;
                                isOpen = true;
                            }
                        }
                    }
                    else
                    {
                        if (exits[i].doorPosition == DoorPosition.Top)
                        {
                            if (exits[i].boundingBox.Height < MapUnit.MAX_SIZE)
                            {
                                exits[i].boundingBox.Height++;
                                exits[i + 1].boundingBox.Height++;
                                exits[i + 1].boundingBox.Y--;
                                SoundManager.GetInstance().Play3D(ref ElevatorClose, ElevatorEmitter, SoundManager.ELEVATOR.CLOSE);
                                SoundManager.GetInstance().Stop(ref ElevatorOpen);
                                isOpen = true;
                            }
                            if (elevatorVolume > elevatorLowestVolume) elevatorVolume -= 5f;
                        }
                        if (exits[i].doorPosition == DoorPosition.Bottom)
                        {
                            if (exits[i].boundingBox.Height < MapUnit.MAX_SIZE)
                            {
                                exits[i].boundingBox.Height++;
                                exits[i].boundingBox.Y--;
                                exits[i + 1].boundingBox.Height++;
                                isOpen = true;
                            }
                        }
                        if (exits[i].doorPosition == DoorPosition.Left)
                        {
                            if (exits[i].boundingBox.Width < MapUnit.MAX_SIZE)
                            {
                                exits[i].boundingBox.Width++;
                                exits[i + 1].boundingBox.Width++;
                                exits[i + 1].boundingBox.X--;
                                isOpen = true;
                            }
                            if (elevatorVolume > elevatorLowestVolume) elevatorVolume -= 5f;
                        }
                        if (exits[i].doorPosition == DoorPosition.Right)
                        {
                            if (exits[i].boundingBox.Width < MapUnit.MAX_SIZE)
                            {
                                exits[i].boundingBox.Width++;
                                exits[i].boundingBox.X--;
                                exits[i + 1].boundingBox.Width++;
                                isOpen = true;
                            }
                        }
                    }
                }
            }
            else
            {
                isLocked = true;
                elevatorLowestVolume = 0;
                elevatorVolume = 0;
            }
            ElevatorMusic.SetVariable("Volume", elevatorVolume);

            SoundManager.GetInstance().cueUpdate(ref ElevatorMusic, ElevatorEmitter);
            if (!ElevatorMusic.IsPaused) SoundManager.GetInstance().Play3D(ref ElevatorMusic, ElevatorEmitter, SoundManager.ELEVATOR.MUSIC);
        }

        public static void ElevatorVolume(float value) {
            elevatorVolume = value;
        }

        public override void Draw(SpriteBatch batch)
        {
            // Draw tile under Exit
            if (underTile != null)
            {
                batch.Draw(underTile, new Rectangle ((int)getCurrPos().X, (int)getCurrPos().Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE), Color.White);
            }

            batch.Draw(texture, boundingBox, /*exitColor*/Color.White);

            if (soundDelay <= 0)
            {
                if (elevatorVolume != 0) VisionManager.addVisionPoint(this.position + new Vector2(this.boundingBox.Width, this.boundingBox.Height), (float)Game1.random.Next(50, 200), false);
                soundDelay = soundDelayReset;
            }

            soundDelay--;
        }

        public void DrawOutline(SpriteBatch spriteBatch)
        {
            if (isLocked)
            {
                spriteBatch.Draw(outlineClosed, boundingBox, Color.Red);
            }
            else
            {
                if (isOpen)
                {
                    spriteBatch.Draw(outlineOpen, boundingBox, Color.Green);
                }
                else
                {
                    spriteBatch.Draw(outlineClosed, boundingBox, Color.Green);
                }
            }
        }

        // Draw message for player to find lockdown switch
        public void DrawText(SpriteBatch batch)
        {
            // If player is near exit and lockdown switch has not been triggered, display message to find switch
            if (isLocked && playerNear)
            {
                batch.DrawString(Game1.contentManager.Load<SpriteFont>(@"Fonts/Skyrim"),
                                  "Find Switch", position + new Vector2(MapUnit.MAX_SIZE,
                                                                        MapUnit.MAX_SIZE / 2), Color.Black);
            }
        }

        public static Vector2 getDoorCenter(ref List<Exit> exits)
        {
            for (int i = 0; i < exits.Count; i += 2)
            {
                if (exits[i].position.X == exits[i + 1].position.X)  //Doors are spaced vertically
                {
                    if (exits[i].position.Y < exits[i + 1].position.Y)
                    {
                        exits[i].doorPosition = DoorPosition.Top;
                        exits[i].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCU");
                        exits[i + 1].doorPosition = DoorPosition.Bottom;
                        exits[i + 1].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCD");
                        //Console.WriteLine("Door One = Top (" + exits[i].position.Y + ") \n Door Two = Bottom (" + exits[i + 1].position.Y + ")");
                    }
                    else
                    {
                        exits[i].doorPosition = DoorPosition.Bottom;
                        exits[i].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCD");
                        exits[i + 1].doorPosition = DoorPosition.Top;
                        exits[i + 1].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCU");
                        //Console.WriteLine("Door One = Bottom (" + exits[i].position.Y + ") \n Door Two = Top (" + exits[i + 1].position.Y + ")");
                    }
                }
                else //Doors are spaced horizontally
                {
                    if (exits[i].position.X < exits[i + 1].position.X)
                    {
                        exits[i].doorPosition = DoorPosition.Left;
                        exits[i].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCL");
                        exits[i + 1].doorPosition = DoorPosition.Right;
                        exits[i + 1].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCR");
                        //Console.WriteLine("Door One = Left (" + exits[i].position.X + ") \n Door Two = Right (" + exits[i + 1].position.X + ")");
                    }
                    else
                    {
                        exits[i].doorPosition = DoorPosition.Right;
                        exits[i].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCR");
                        exits[i + 1].doorPosition = DoorPosition.Left;
                        exits[i + 1].outlineClosed = Game1.contentManager.Load<Texture2D>(@"Textures/Objects/Environment/Exit/ExitTileOutlineCL");
                        //Console.WriteLine("Door One = Right (" + exits[i].position.X + ") \n Door Two = Left (" + exits[i + 1].position.X + ")");
                    }
                }
            }

            return Vector2.Zero;
        }

        #region Testing

        public new string toString()
        {
            return "E";
        }
        #endregion

        public static void Play()
        {
            SoundManager.GetInstance().Play3D(ref ElevatorMusic, ElevatorEmitter, SoundManager.ELEVATOR.MUSIC);
        }

        public static void Pause()
        {

            SoundManager.GetInstance().Puase(ref ElevatorMusic);
        }

        public static void Stop()
        {
            SoundManager.GetInstance().Stop(ref ElevatorMusic);
        }

        public static void setVolume(float value)
        {
            elevatorVolume = value;
        }

        public static void closeDoors(ref List<Exit> exits)
        {
            for (int i = 0; i + 1 < exits.Count; i += 2)
            {
                if (exits[i].doorPosition == DoorPosition.Top)
                {
                    while (exits[i].boundingBox.Height < MapUnit.MAX_SIZE)
                    {
                        exits[i].boundingBox.Height++;
                        exits[i + 1].boundingBox.Height++;
                        exits[i + 1].boundingBox.Y--;
                    }
                }
                if (exits[i].doorPosition == DoorPosition.Bottom)
                {
                    while (exits[i].boundingBox.Height < MapUnit.MAX_SIZE)
                    {
                        exits[i].boundingBox.Height++;
                        exits[i].boundingBox.Y--;
                        exits[i + 1].boundingBox.Height++;
                    }
                }
                if (exits[i].doorPosition == DoorPosition.Left)
                {
                    while (exits[i].boundingBox.Width < MapUnit.MAX_SIZE)
                    {
                        exits[i].boundingBox.Width++;
                        exits[i + 1].boundingBox.Width++;
                        exits[i + 1].boundingBox.X--;
                    }
                }
                if (exits[i].doorPosition == DoorPosition.Right)
                {
                    while (exits[i].boundingBox.Width < MapUnit.MAX_SIZE)
                    {
                        exits[i].boundingBox.Width++;
                        exits[i].boundingBox.X--;
                        exits[i + 1].boundingBox.Width++;
                    }
                }
            }
        }

        public bool GetIsOpen()
        {
            return isOpen;
        }

        public bool GetIsLocked()
        {
            return isLocked;
        }

        // Returns map x/y values for Exit
        public Vector2 getIndex()
        {
            return index;
        }
    }
}
