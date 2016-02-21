using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using System.Threading;

namespace Sonar
{
    class SoundManager
    {

        #region Fields
        static SoundManager soundManager;

        static List<Sound> sounds;
        List<Sound> soundless;
        List<ElevatorAccess> elevatorAccess;
        Dictionary<Type, Func<string, object>> cueDictionary;


        /*AudioListener*/object playerListener;

        bool tension;
        InteractiveMusic music;

        #endregion Fields

        /// <summary>
        /// SoundManager Constructor
        /// </summary>
        public SoundManager(object gObject) {
            Audio.LoadAudio();

            sounds = new List<Sound>();
            soundless = new List<Sound>();
            elevatorAccess = new List<ElevatorAccess>();
            playerListener = new /*AudioListener*/object();

            music = new InteractiveMusic(gObject);
        }

        #region GetCue

        /// <summary>
        /// Grabs and returns the specified/*Sound*/object from sound bank
        /// </summary>
        /// <param name="sound">Sound Enumeration to play</param>
        /// <returns>Sound from the THROWABLE sound bank</returns>
        public static Sound getCue(object gObject, string item)
        {
            Sound sound = Audio.CreateAudio(gObject, item) as Sound;
            return sound; 
        }

        #endregion GetGue

        #region makeSound <Functionality to make different types of sounds in the game>

        /// <summary>
        /// Creates a General Sound.
        /// </summary>
        /// <param name="Sound">Sound to be played</param>
        public static void createSound(object gObject, string Sound)
        {
            createSound(gObject, 0, 0, 0, Sound, false);
        }

        #region Play   <Safely plays a given/*Sound*/object until it is stopped or ends with/without 3D Effects. Useful for when cues need to be reused.>
        /// <summary>
        /// Safely plays a given/*Sound*/object until it is stopped or ends without 3D Effects. 
        /// Then replaces it with a SoundManager Enumeration.* sound. Useful for when 
        /// cues need to be reused.
        /// </summary>
        /// <param name="SoundCue">Sound to be played</param>
        /// <param name="Enumeration">The next sound to play</param>
        public static void Play(ref Sound sound, object SoundManagerEnumeration)
        {
            sound.PLAY();
        }
   
        /// <summary>
        /// Safely plays a given/*Sound*/object until it is stopped or ends with 3D Effects. 
        /// Then replaces it with a SoundManager Enumeration.* sound. Useful for when 
        /// cues need to be reused.
        /// </summary>
        /// <param name="SoundCue">Sound to played</param>
        /// <param name="Emmitter">Emmitter associated with the given Sound</param>
        /// <param name="Enumeration">The next sound to play</param>
        public void Play3D(ref Sound sound, object Emmitter, object SoundManagerEnumeration)
        {
            sound.PLAY();
        }

        #endregion Play 

        #region playSoundFX <Plays a sound effect that doesn't register as an ingame sound. Useful for Hud/Menu sound effects>
        /// <summary>
        /// Plays a SoundManager.Environment.* sound effect that doesn't register as an in game sound it
        /// has no 3D sound capability and can't be detected by spectres or the player.
        /// Usefull for Hud/Menu sound effects.
        /// </summary>
        /// <param name="sound">Sound to play</param>
        public void playSoundFX(Sound sound)
        {
            sound.PLAY();
        }
        #endregion playSoundFX

        #region createSound <Creates and ingame Sound that can be seen, heard and be suspicious to spectres.>

        /// <summary>
        /// Creates and ingame Sound that can be seen, heard and be suspicious to spectres.
        /// </summary>
        /// <param name="Position">Position of the sound to be created</param>
        /// <param name="Radius">The visual radius of the sound to be created</param>
        /// <param name="health">The Health of the sound to be created. When the health reaches Zero the sound will be destroyed</param>
        /// <param name="minimizeSpeed">The speed at which the health of the sound will deplete </param>
        /// <param name="DampFactor">Value = 1; Currently Unsupported</param>
        /// <param name="Range">How far (in pixel space) can this sound be heard</param>
        /// <param name="sound">The sound to be played</param>
        /// <param name="Suspicious">If true, Spectres will go to the source of the sound if heard. If false, spectres will ignore this sound completely</param>
        public static void createSound(object gObject, float Radius, float health, float minimizeSpeed, string sound, bool Suspicious)
        {
            createSound(gObject, Radius, health, minimizeSpeed, sound, Suspicious, null);
        }

        /// <summary>
        /// Creates and ingame Sound that can be seen, heard and be suspicious to spectres.
        /// </summary>
        /// <param name="Position">Position of the sound to be created</param>
        /// <param name="Radius">The visual radius of the sound to be created</param>
        /// <param name="health">The Health of the sound to be created. When the health reaches Zero the sound will be destroyed</param>
        /// <param name="minimizeSpeed">The speed at which the health of the sound will deplete </param>
        /// <param name="DampFactor">Value = 1; Currently Unsupported</param>
        /// <param name="Range">How far (in pixel space) can this sound be heard</param>
        /// <param name="sound">The sound to be played</param>
        /// <param name="Suspicious">If true, Spectres will go to the source of the sound if heard. If false, spectres will ignore this sound completely</param>
        /// <param name="source">Spectre that is the source of the sound, used to prevent spectres from alerting themselves</param>
        public static void createSound(object gObject, float Radius, float health, float minimizeSpeed, string sound, bool Suspicious, Object source)
        {
            //if (source == null || (source != null && source.GetType() != typeof(Radio)))
            //    VisionManager.addVisionPoint(Position, health, false);
            Sound temp = new Sound(gObject, Radius, sound, Suspicious, source);
            sounds.Add(temp);
        }

        #endregion createSound

        #endregion makeSound

        #region CueCommands

        /// <summary>
        /// The manual/*Sound*/object Update that Updates a given Sound's 3D sound.
        /// </summary>
        /// <param name="SoundCue">Sound to be updated</param>
        /// <param name="Emmitter">Emmitter Associated with the given Sound</param>
        //public void cueUpdate(ref object SoundCue, Object Emmitter)
        //{
        //}

        /// <summary>
        /// Puases a Given Sound
        /// </summary>
        /// <param name="SoundCue">Sound to Puase</param>
        public static void Puase(ref Sound sound)
        {
            sound.PUASE();
        }

        /// <summary>
        /// Stops a Given Sound
        /// </summary>
        /// <param name="SoundCue">Sound to stop</param>
        public static void Stop(ref Sound sound) 
        {
            sound.STOP();
        }

        /// <summary>
        /// Toggles a given/*Sound*/object On if off and off if on
        /// </summary>
        /// <param name="SoundCue">Sound to be toggle on or off</param>
        /// <param name="Emmitter">AudioEmitter used for the Sound</param>
        /// <param name="Enumeration">Sound that is to replace the given/*Sound*/object when the given/*Sound*/object stops</param>
        public void Toggle(ref Sound sound, object Emmitter)
        {
            if (sound.IsPlaying()) sound.STOP();
            else sound.PLAY(); 
        }

        #endregion CueCommands

        #region Update/Draw
        public void Update()
        {
            /// Updates Player listener for 3D Audio
            //playerListener.Position = new /*Vector3*/object(Player.getInstance().position.X, 0, Player.getInstance().position.Y);

            /// Updates the Ingame Sounds
            for (int index = 0; index < sounds.Count; index++)
            {
                    sounds[index].PLAY();
            }
            sounds.Clear();

        }
        #endregion Update/Draw

        #region Getters

        /// <summary>
        /// Returns the SoundManager's AudioEngine
        /// </summary>
        /// <returns>AudioEngine</returns>
        public object Engine() {
            return null;
        }
        #endregion Getters

        /// <summary>
        /// Stops all in/outgame sounds that the SoundManager has control over.
        /// </summary>
        public void Stop() {
            foreach (Sound sound in sounds)
                sound.STOP();
            music.Stop();
        }

        #region Ambient Sound Functions

        public void PostEvent()
        {

        }

        public void PlayerChase() {
            //Stop(ref PlayerBeingSearchedFor);
            //Play(ref PlayerBeingChased, SoundType.AMBIENT.PLAYER_SEEN);
            //tension = true;
        }

        public void PlayerSearch()
        {
            //Stop(ref PlayerBeingChased);
            //Play(ref PlayerBeingSearchedFor, SoundType.AMBIENT.SPECTRE_INVESTIGATE);
            //tension = true;
        }

        public void PlayerResolve()
        {
            //if (tension)
            //{
            //    Stop(ref PlayerBeingSearchedFor);
            //    Stop(ref PlayerBeingChased);
            //    Play(ref TensionResolve, SoundType.AMBIENT.TENSION_RESOLVE);
            //    tension = false;
            //}
        }

        public void ElevatorLevel(int Level) {
            //Array values = Enum.GetValues(SoundType.LOCKDOWN.ONE.GetType());
            //if (!GameScreen.inTutorial)
            //{
            //    if (Level != 0)
            //        elevatorAccess.Add(new ElevatorAccess((LOCKDOWN)values.GetValue(Level - 1)));
            //}
            //else elevatorAccess.Add(new ElevatorAccess((LOCKDOWN)values.GetValue(Level - 1)));
        }

        #endregion Ambient Sound Functions

        /// <summary>
        /// Adjust the volume of all sounds in the game
        /// </summary>
        /// <param name="value">a value between 0 and 100</param>
        public void MasterVolume(float value) {

        }
    }
}
