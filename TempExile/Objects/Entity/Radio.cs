﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Sonar
{

    /* Code Created by Joshua Ray AKA: Slugnifacent
     * Date: Sunday Febuary 14th 2010
     * Updated: 3/31/2012
     */

    public class Radio : Object
    {

        #region Fields

        Sound radioCue;
        public int radioNum;
        float volume = 1;
        AudioEmitter emitter;

        float soundDelay;
        float soundDelayReset = 7;
        bool startOn;
        bool visible;

        GameTexture outline;

        public bool lastHeardState;

        #endregion

        #region Constructor 

        public Radio(GameVector2 Position)
        {

            position = Position;
            color = GameColor.White;

            String version = "Textures/Objects/Entity/Radio/desk_radio"; // Variation of radio to display
            String outlineVersion = "Textures/Objects/Entity/Radio/desk_radio_outline";
            Random rand = Game1.random;
            radioNum = rand.Next(1, 3);
            version += radioNum;
            outlineVersion += radioNum;

            texture = Game1.contentManager.Load<GameTexture>(@"" + version);
            outline = Game1.contentManager.Load<GameTexture>(@"" + outlineVersion);
            //texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Radio/radio");
            boundingBox = new GameRectangle((int)position.X, (int)position.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);
 
            radioCue = SoundManager.getCue(SoundManager.RADIO.RADIO);
            emitter = new AudioEmitter();
            soundDelay = 0;
            lastHeardState = false;

            visible = false;
        }

        #endregion Constructor 

        #region Commands

        public void Play()
        {
            SoundManager.Play3D(ref radioCue, emitter, SoundManager.RADIO.RADIO);
        }

        public void Pause()
        {
            if (radioCue != null)
                SoundManager.Puase(ref radioCue);
        }

        public void unPause()
        {
            if (radioCue != null)
            {
                if (radioCue.IsPaused)
                    SoundManager.Play3D(ref radioCue, emitter, SoundManager.RADIO.RADIO);
            }
        }

        public void Stop()
        {
            SoundManager.Stop(ref radioCue);
        }

        public void decreaseVolume(float value)
        {
            increaseVolume(-value);
        }

        public void increaseVolume(float value)
        {
            volume = MathHelper.Clamp(volume + value, 0, 1);
            string cool = SoundManager.AudioCategory.RADIO.ToString();
            AudioCategory temp = SoundManager.Engine().GetCategory(cool);
            temp.SetVolume(volume);
        }
       
        public void Toggle() {
            if (radioCue.IsPlaying)
            {
                SoundManager.playSoundFX(SoundManager.RADIO.BUTTON_OFF);
                Stop();
            }
            else
            {
                SoundManager.playSoundFX(SoundManager.RADIO.BUTTON_ON);
                Play();
            }
        }

        #endregion Commands

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (/*isPlaying() &&*/ soundDelay <= 0)
            {
                if (soundDelay <= 0)
                {
                    if (isPlaying())
                        VisionManager.addVisionPoint(position, (float)Game1.random.Next((int)(400 * volume), (int)(600 * volume)), false);
                    SoundManager.createSound(position, (float)Game1.random.Next((int)(400 * volume), (int)(600 * volume)), (float)Game1.random.Next((int)(400 * volume), (int)(600 * volume)), 5f, null, true, this);
                    soundDelay = soundDelayReset;
                }
            }

            emitter.Position = new Vector3(position.X, 0, position.Y);
            SoundManager.cueUpdate(ref radioCue, emitter);
            if (soundDelay > 0) soundDelay--;
            if (startOn)
            {
                Play();
                startOn = false;
            }

            if (!visible)
            {
                if (startOn || isPlaying())
                {
                    visible = true;
                }
                else
                {
                    double xDist = Math.Pow((Player.getInstance().position.X - this.position.X), 2);
                    double yDist = Math.Pow((Player.getInstance().position.Y - this.position.Y), 2);
                    int dist = (int)Math.Sqrt(xDist + yDist);
                    if (dist < 100f || dist < VoiceEngine.getInstance().VOLUME * 1000f)
                    {
                        visible = true;
                    }
                }
            }
        }


        #endregion Update

        #region Collide
        /// <summary>
        /// Steven Ekejiuba 5/9/2012
        /// Determines if there is an object close to Radio, without intersecting it
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Collide(GameRectangle rect)
        {
            if (boundingBox.Intersects(new GameRectangle(rect.X - rect.Width / 4,
                                                     rect.Y - rect.Height / 4,
                                               (int)(rect.Width + rect.Width / 2),
                                               (int)(rect.Height + rect.Height / 2))))
            {
                return true;
            }
            return false;
        }

        #endregion


        public void DefualtON() {
            startOn = true;
            lastHeardState = true;
        }

        /// <summary>
        /// Getter for startOn
        /// </summary>
        /// <returns></returns>
        public bool DoesItStartOn() {
            return startOn;
        }

        #region Draw

        public override void Draw(object spriteBatch)
        {
            spriteBatch.Draw(texture, boundingBox, color);
        }

        #endregion Draw

        public void DrawOutline(object spriteBatch, GameColor color)
        {
            if (visible)
            {
                //if(Util.getInstance().wallsBetween(Player.getInstance().position, this.position) == 0 || isPlaying())
                    spriteBatch.Draw(outline, boundingBox, color);
            }
        }

        /// <summary>
        /// Steven Ekejiuba 5/10/2012
        /// Returns Radio back to its initial state
        /// </summary>
        public void Reset()
        {
            /*if (isPlaying())
            {
                Toggle();
            }

            if (startOn)
            {
                if (!isPlaying())
                {
                    Toggle();
                }
            }*/

            visible = false;
        }


        #region Return Functions

        public byte ALPHA
        {
            get { return color.A; }
        }

        public bool isPlaying()
        {
            return radioCue.IsPlaying;
        }

        public float GetSoundDelay()
        {
            return soundDelay;
        }

        #endregion Return Functions

    }
}
