using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;






//using System.Speech.AudioFormat;
//using System.Speech.Recognition;
//using System.Threading;
//using SpeechLib;


namespace Sonar
{
    /* This Voice engine uses the Microsoft Speech API (SAPI) for Speech recognition
     * It currently supports the defualt microphone of the Windows OS and the Kinect Specifically
     * The major flaw in the system is the delay in the Kinect and the Speach recognition in general. 
     * With the kinect it takes time for the kinect to perform it's audio anaylsis 
     */
    
    public class VoiceEngine
    {
        #region Fields
        private static VoiceEngine engine;

        //Bar Volume;
        //Microphone mic;
        //Stream audioStream;
        //SoundEffect shoutEffect;
        //SoundEffectInstance shoutInstance;
        //SpeechRecognitionEngine speechRecognizer;

        object Volume;
        object mic;
        Stream audioStream;
        object shoutEffect;
        object shoutInstance;
        object speechRecognizer;

        byte[] soundData;
        List<object> spokenWords;
        String[] possessionWords;
        String[] passwordWords;
        string spokenWord;
        int timeSinceSpoken;
        int maxTimeSinceSpoken = 2;
        #endregion Fields

        #region Dictionary
        Dictionary<string, /*Cue*/object> shout = new Dictionary<string,/*Cue*/object >()
        {
            {" ",null},
            /*
            {"Raan Mir Tah" ,"Animal Allegieance"},
            {"Laas Yah Nir" ,"Aura Whisper"},
            {"Feim Zii Gron","Become Ethereal"},
            {"Od Ah Viing"  ,"Call Dragon"},
            {"Hun Kaal Zoor","Call of Valor"},
            {"Lok Vah Koor" ,"Clear Skies"},
            {"Zun Haal Viik","Disarm"},
            {"Faas Ru Maar" ,"Dismay"},
            {"Joor Zah Frul","Dragonrend"},
            {"Su Grah Dun"  ,"Elemental Fury"},
            {"Yol Toor Shol","Fire Breath"},
            {"Fo Krah Diin" ,"Frost Breath"},
            {"Liz Slen Nuz" ,"Ice Form "},
            {"Kaan Drem Ov" ,"Kyne's Peace "},
            {"Krii Lun Aus" ,"Marked for Death"},
            {"Tiid Klo Ui"  ,"Slow Time"},
            {"Strun Bah Qo" ,"Storm Call"},
            {"Zul Mey Gut"  ,"Throw Voice"},
            {"Fus Ro Dah"   ,"Unrelenting Force"},
            {"Fuuuuuuus Ro Dah","Unrelenting Force 2"},
            {"Fu u u u us Ro Dah","Unrelenting Force 3"},
            {"Rotten Banana","Thu'um"},
            {"Sonar Effect Activate", "Suicide Activate"},
            {"Purple Monkey Dishwasher","Chris is the shit!!"},
            {"Wuld Nah Kest" ,"Whirlwind Sprint"},
            {"Doh vah kin", "Dragonborn"},
            {"Ha Ha Ha Ha Ha","I am Evil!!"},
            {"Kah Me Ha Me Ha", "GOKU!"},
            {"Shinku Tatsumaki Sempu Kyaku","Twisting Dragon Kick"},
            {"Orey No Kacheta","Victory is mine"}   
            */          
   

            //Words I am making up, to sound like an "alien" language(some work, some arent tested)
             
            {"Dar Koh Tawn",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString().ToString())},
            {"Daw Tar",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                   
            //{"Xenia",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)}, 
            {"Jar Tawn",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},             
            //{"Ko Tar Xenia",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            {"Boh Sun",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                    
            {"Tawn Dar",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                              
            //{"Ech Korgi",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},

            {"Mutt Ma Een",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                              
            {"Pey Thah Bah",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            {"An Go Tea",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},      
            {"Zel Za Lah",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                   
            
            {"Kah Fee Lah",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                                                               
            {"Bah Lish",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                                                                                 


            //Words that I know Work(for me and Josh)
            
            {"Epsilon",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            {"Alpha",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            {"Beta",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            //{"Sound",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            //{"Touch",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},            
            //{"Sense",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},                        
            //{"Scream",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            //{"Lockdown",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            //{"Sight",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            //{"Xenia",SoundManager.getCue(null,SoundManager.XENIA.XENIA_WHISPER)},
            {"Teras",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},                        
            {"Omilia",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            {"Trecho",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())},
            
             {"Sonar",SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString())}
 
        };


        #endregion Dictionary

        private VoiceEngine()
        {
            loadGenericMicrophone();
            //spokenWords = new List<Text>();
            //spokenWords.Add(new Text("Test"));
            //Volume = new Bar(100, new /*Vector2*/object(50, 300), 400, 20, true);

            spokenWords = null;
            spokenWords.Add(null);
            Volume = null;

            possessionWords = new String[]
                { "Dar Koh Tawn", 
                  "Daw Tar",
                  "Jar Tawn",
                  "Boh Sun",
                  "Tawn Dar",
                  "Mutt Ma Een",
                  "Pey Thah Bah",
                  "An Go Tea",
                  //"Zel Za Lah",
                  "Kah Fee Lah",
                  "Bah Lish"};

            passwordWords = new String[]
                { "Epsilon", 
                  "Alpha",
                  "Beta",
                  "Teras",
                  "Omilia",
                  "Gamma"};
        }

        public static  VoiceEngine getInstance(){
            if (engine == null) engine = new VoiceEngine();
                return engine;
        }


        public void Update()
        {
            //for (int index = 0; index < spokenWords.Count; index++)
            //{
            //    if (spokenWords[index].color.A - 1 > 0) spokenWords[index].color.A--;
            //    else
            //    {
            //        spokenWords.RemoveAt(index);
            //        index--;
            //    }
            //}
            //Volume.HEALTH = detectVolume() * 200;

            ////Console.Out.WriteLine("Timer val = " + timeSinceSpoken);
            //if (timeSinceSpoken > 0)
            //{
            //    timeSinceSpoken--;
            //}
        }

        public void Draw()
        {
           // if (spokenWords.Count > 0)
                //foreach (Text word in spokenWords)
                //{
                //    word.Draw(batch);
                //}
           // Volume.Draw(batch);
        }

        public float VOLUME {
            get { /*return Volume.HEALTH / 200;*/ return 0; }
            set { /*Volume.HEALTH = value;*/ }
        }

        public bool SPEAKING
        {
            get { return /*(Volume.HEALTH/200 > .04f)*/false; }
        }

        public string RandomPhrase() {
            //  int index = Game1.random.Next(1,shout.Count());
            // return shout.ElementAt(index).Key;
            return null;
        }

        public string CalibratedPhrase()
        {
            int index;

            //if (AdvancedCalibrationScreen.isCalibrated)
            //{
            //    index = Game1.random.Next(1, AdvancedCalibrationScreen.calibratedWords.Count());
            //    return AdvancedCalibrationScreen.calibratedWords[index];
            //}

            //index = Game1.random.Next(1, possessionWords.Length);
            //return possessionWords[index];
            return "";
        }

        public string PasswordPhrase()
        {
            //int index = Game1.random.Next(1, passwordWords.Length);
            //return passwordWords[index];
            return "";
        }

        public List<String> getExcorcismDictionary()
        {
            List<String> output = new List<String>();
            for (int i = 0; i < possessionWords.Length; i++)
            {
                output.Add(possessionWords[i]);
            }
            return output;
        }

        public string SpokenWord() {
            return spokenWord;
        }

        public void startTimer()
        {
            timeSinceSpoken = maxTimeSinceSpoken;
        }

        public int getTimeSinceSpoken()
        {
            return timeSinceSpoken;
        }

        public/*Cue*/object excorcismAudio(string Phrase)
        {
            return shout[Phrase]; 
        }

        /// <summary>
        /// Steven Ekejiuba
        /// 4/6/2012
        /// Returns an array of every element in dictionary
        /// </summary>
        /// <returns></returns>
        public List<string> GetDictionary()
        {
            List<string> elements = new List<string>();
            for (int i = 0; i < shout.Count; i++)
            {
                elements.Add(shout.ElementAt(i).Key);
            }
            return elements;
        }

        #region Speech Recognition

        private void createGrammar()
        {
            //speechRecognizer = new SpeechRecognitionEngine();
            //Choices SHOUTS = new Choices();
            //Choices trap = new Choices();
            //trap.Add(" ");
            //SHOUTS.Add(" ");
            //foreach (var item in shout)
            //{
            //    string[] split = item.Key.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            //    if (split.Length == 3)
            //    {
            //        trap.Add(split[0]);
            //        trap.Add(split[1]);
            //        trap.Add(split[2]);
            //        trap.Add(split[0] + " " + split[1]);
            //        trap.Add(split[1] + " " + split[2]);
            //    }
            //    SHOUTS.Add(item.Key);
            //}

            //GrammarBuilder traplanguage = new GrammarBuilder();
            //traplanguage.Append(trap);

            //GrammarBuilder language = new GrammarBuilder();
            //language.Append(SHOUTS);
            //language.Append(SHOUTS);

            //Grammar grammartrap = new Grammar(traplanguage);
            //Grammar grammarlanguage = new Grammar(language);

            //grammartrap.Priority = 127;
            //grammartrap.Weight = 1;
            //grammarlanguage.Priority = -128;
            //grammarlanguage.Weight = .1f;
            //speechRecognizer.LoadGrammar(grammartrap);
            //speechRecognizer.LoadGrammar(grammarlanguage);
        }

        private void loadSpeechEngine()
        {
            //createGrammar();
            //speechRecognizer.SetInputToDefaultAudioDevice();
            //speechRecognizer.SpeechRecognized += SreSpeechRecognized;
            //speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            //speechRecognizer.EndSilenceTimeout = TimeSpan.FromMilliseconds(50);
            //speechRecognizer.EndSilenceTimeoutAmbiguous = TimeSpan.FromMilliseconds(50);
        }

        private /*RecognizerInfo*/object GetKinectRecognizer()
        {
            //Func<RecognizerInfo, bool> matchingFunc = r =>
            //{
            //    string value;
            //    r.AdditionalInfo.TryGetValue("Kinect", out value);
            //    return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            //};
            //return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
            return null;
        }

        void SreSpeechRecognized(object sender, /*SpeechRecognizedEventArgs*/object e)
        {
            ////Console.WriteLine("\nSpeech Recognized: \t{0}", e.Result.Text);
            //if (shout.ContainsKey(e.Result.Text))
            //{
            //    //Console.WriteLine("\nSpeech Recognized: \t{0}", e.Result.Text);
            //    //Console.WriteLine("\nSpeech Recognized: \t{0}", shout[e.Result.Text]);
            //    spokenWord = e.Result.Text;
            //    spokenWords.Add(new Text(spokenWord, new /*Vector2*/object(Game1.random.Next(20, 250), Game1.random.Next(0, 400))));
            //    startTimer();
            //    return;
            //}
        }

        public /*SoundEffect*/object soundEffect() {
            return shoutEffect;
        }

        #endregion Speech Recognition

        #region Microphone Processing

        private float detectVolume()
        {
            // http://social.msdn.microsoft.com/Forums/en-US/kinectsdkaudioapi/thread/ed1af567-166d-4b0c-8ab6-d9db6bd1f957
            // Eddy Escardo-Raffo Volume code
            long totalSquare = 0;
            for (int i = 0; i < soundData.Length; i += 2)
            {
                short sample = (short)(soundData[i] | (soundData[i + 1] << 8));
                totalSquare += sample * sample;
            }
            long meanSquare = 2 * totalSquare / soundData.Length;
            double rms = Math.Sqrt(meanSquare);
            return (float)(rms / 32768.0);
        }

        private bool loadGenericMicrophone()
        {
            //mic = Microphone.Default;
            //mic.BufferDuration = TimeSpan.FromMilliseconds(100);
            //mic.BufferReady += new EventHandler<EventArgs>(bufferReady);
            //soundData = new byte[2 * mic.GetSampleSizeInBytes(mic.BufferDuration)];
            //int size = 2 * mic.GetSampleSizeInBytes(mic.BufferDuration);
            //mic.Start();
            //loadSpeechEngine();
            return true;
        }

        public void bufferReady(object sender, EventArgs e)
        {
           // mic.GetData(soundData);
        }

        #endregion Microphone Processing
    }
     
}
