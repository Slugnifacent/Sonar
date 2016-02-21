using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







namespace Sonar
{
    public class ExcorcismPhrase
    {
        /*Text*/object phrase;
        bool spoken;
        int fadeValue;
        bool active;
        Sound whisper;


        public ExcorcismPhrase()
        {
            spoken = false;
            active = false;
            whisper = SoundManager.getCue(null,SoundType.XENIA.XENIA_WHISPER.ToString());
        }

        public void initialize(string Phrase, /*GameVector2*/object Position,/*Sound*/object cue)
        {
            //phrase = new Text(Phrase);
            //spoken = false;
            //phrase.color = GameColor.White;
            //phrase.color.A = 1;
            //fadeValue = 5;
            //phrase.position = Position;
            //active = true;
            //whisper = cue;
        }


        public void Update(/*GameTime*/object time)
        {
            //if (!active) return;
            //if (!whisper.IsPaused)
            //{
            //    SoundManager.Play(ref whisper, SoundManager.XENIA.WHISPER);
            //    Fade(fadeValue);
            //}
            //phrase.position.X = Player.getInstance().position.X;
            //phrase.position.Y = Player.getInstance().position.Y +32;
        }

        public void Draw(/*SpriteBatch*/object batch)
        {
            //if (!active) return;
            //if (spoken)
            //{
            //    phrase.color.R = 0;
            //    phrase.color.G = 0;
            //    phrase.color.B = 255;
            //}
            //else
            //{
            //    phrase.color.R = 255;
            //    phrase.color.G = 255;
            //    phrase.color.B = 255;
            //}
            //phrase.Draw(batch);

        }

        public bool SPOKEN
        {
            get { return spoken; }
        }

        public bool CheckSpeech(string String)
        {
            //if (phrase.word == String)
            //{
            //    fadeValue = -fadeValue;
            //    spoken = true;
            //    return true;
            //}
            return false;
        }

        public string Phrase()
        {
            //return phrase.word;
            return "";
        }

        public bool Visable()
        {
            return true;
            //return phrase.color.A == 0;
        }

        public bool ACTIVE
        {
            get { return active; }
            set { 
                active = value;
                    if (active) SoundManager.Play(ref whisper, SoundType.XENIA.XENIA_WHISPER.ToString());
                    else SoundManager.Stop(ref whisper);
            }
        }

        void Fade(int Value)
        {

            //if (phrase.color.A + Value > 255)
            //{
            //    phrase.color.A = 255;
            //    return;
            //}

            //if (phrase.color.A + Value < 0)
            //{
            //    phrase.color.A = 0;
            //    return;
            //}

            //phrase.color.A += (byte)Value;
        }

        public void pause()
        {
            if (whisper != null)
                SoundManager.Puase(ref whisper);
        }
        public void unpause()
        {
            //if (whisper != null)
            //{
            //    if (whisper.IsPaused)
            //        SoundManager.Play(ref whisper, SoundManager.XENIA.WHISPER);
            //}
        }

    }
}
