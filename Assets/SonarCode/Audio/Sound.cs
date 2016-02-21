using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;





namespace Sonar
{
    public class Sound
    {
        /* Sound Class
         * Created By Joshua ray
         * 2/15/2012
         * 
         * This class is intended to be used for instantaneous sounds that exist and die of quickly
         * i.e footstep, landing of throwable objects, walking of debris etc. this way there is a 
         * physical representation of sound that objects can reference.
         * This was not designed to be used for sources of continous sounds, like the Radio and 
         * Voice recognition. 
         */

        #region Fields
        bool alive;
        float radius;
        float radiusMax;
        float dropRate;
        bool suspicious;

        object source;
        object audio;
        GameVector2 position;
        /*GameTexture*/
        object tex;

        #endregion Fields
       
        /// TODO: Remove Minimize, HEALTH


        #region Constructors
        /// <summary>
        /// Sound Object. To be created for an individual soundless instance. i.e footsteps, objects thrown etc
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <param name="Position">The Position of the sound</param>
        /// <param name="Radius"> The Radius of the Sound</param>
        /// <param name="Suspicious">If true, Spectres will be raise suspicion upon hearing this sound.</param>
        public Sound(object Position, float Radius, bool Suspicious)
        {
            Initialize(Position, Radius, null, Suspicious, null);
        }


        /// <summary>
        /// Sound Object. To be created for an individual sound instance. i.e footsteps, objects thrown etc
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <param name="Position">The Position of the sound</param>
        /// <param name="Radius"> The Radius of the Sound</param>
        /// <param name="SFX">The soundeffect that this sound makes</param>
        /// <param name="Suspicious">If true, Spectres will be raise suspicion upon hearing this sound.</param>
        public Sound(object Position, float Radius, string SFX, bool Suspicious)
        {
            Initialize(Position, Radius, SFX, Suspicious, null);
        }

        /// <summary>
        /// Sound Object. To be created for an individual sound instance. i.e footsteps, objects thrown etc
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <param name="Position">The Position of the sound</param>
        /// <param name="Radius"> The Radius of the Sound</param>
        /// <param name="SFX">The soundeffect that this sound makes</param>
        /// <param name="Suspicious">If true, Spectres will be raise suspicion upon hearing this sound.</param>
        public Sound(object Position, float Radius, string SFX, bool Suspicious, Object source)
        {
            Initialize(Position, Radius, SFX, Suspicious, source);
        }

        /// <summary>
        /// Helper Constructor
        /// 3/5/2012
        /// </summary>
        /// <param name="gObject">GameObject reference</param>
        /// <param name="Radius"> The Radius of the Sound</param>
        /// <param name="SFX">The soundeffect that this sound makes</param>
        /// <param name="Suspicious">If true, Spectres will be raise suspicion upon hearing this sound.</param>
        void Initialize(object gObject, float Radius, string SFX, bool Suspicious, Object Source) 
        {
            radius = 0;
            alive = true;
            source = Source;
            position = null;
            suspicious = Suspicious;
            radiusMax = Radius / 2f;
            dropRate = radiusMax / 25;
            audio = Audio.CreateAudio(gObject, SFX);
        }

        #endregion Constructors

        #region Update
        /// <summary>
        /// Updates the Sound object.
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <param name="Time">The /*GameTime*/object</param>
        public void Update()
        {
            if (radius < radiusMax)
            {
                radius += dropRate;
               // soundSphere.Radius = radius;
                if (radius > radiusMax) radius = radiusMax;
            }
            else alive = false;
            if (audio != null)
            {
                PLAY();
            }
        }
        #endregion Update

        #region Commands
        /// <summary>
        /// Plays the sound.
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        public void PLAY()
        {
            Audio.Play(audio);
        }

        /// <summary>
        /// Stops the sound.
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        public void STOP()
        {
            Audio.Stop(audio);
        }

        /// <summary>
        /// Puases the sound.
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        public void PUASE()
        {
            Audio.Pause(audio);
        }
        #endregion Commands

        #region Getters & Setters

        public bool IsPlaying()
        {
            return false;
        }

        public bool IsPaused()
        {
            return false;
        }

        /// <summary>
        /// Returns True if Alive and False if Dead
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>True if Alive - False if Dead</returns>
        public bool ALIVE {
           get { return alive; }
        }

        /// <summary>
        /// Returns Radius of the Sound Object
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>Returns radius of the sound</returns>
        public float RADIUS {
            get { return radius; }
        }

        /// <summary>
        /// Returns Grid Coordinates of the Sound
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>Returns Grid Coordinates of the Sound</returns>
        public object GridCoordinates()
        {
            //  return Util.getInstance().GridCoordinates(position);
            return null;
        }

        /// <summary>
        /// Returns Grid Coordinates of the Sound
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>Returns Grid Coordinates of the Sound</returns>
        public string CueName()
        {
            if (audio != null)
                return "";
            else return "";
        }

        /// <summary>
        /// Returns Origin of the Sound
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>Returns origin of the Sound</returns>
        public GameVector2 POSITION
        {
            get {return position;}
            set { position = value; }
        }

        /// <summary>
        /// Returns Suspicion of the Sound
        /// Created by Joshua Ray
        /// 3/5/2012
        /// </summary>
        /// <returns>Returns Suspicion flag of the Sound</returns>
        public bool SUSPICIOUS
        {
            get { return suspicious; }
        }

        /// <summary>
        /// Derrick Huey
        /// Returns source of the sound
        /// </summary>
        public object SOURCE
        {
            get { return source; }
        }



        #endregion Getters & Setters
       
    }
}
