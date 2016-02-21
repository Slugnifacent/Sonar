using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class ToSleepTransition : Transition
    {
        //Transition back to sleep state. Checks if Siren has been awake long enough.
        public ToSleepTransition(State s)
            : base(s)
        {
            condition = new AwakeTimeCondition();
        }

        // Transition to sleep state when a set amount of time passes
        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("to sleep");
            //spectre.SetSprite("Sleep");
            FacePlayer(spectre, player);
            return;
        }

        /// <summary>
        /// Steven 5/15/2012
        /// When Spectre falls asleep it sleeps facing the direction it was last looking
        /// </summary>
        /// <param name="spectre"></param>
        /// <param name="player"></param>
        public void FacePlayer (Spectre spectre, Player player)
        {
            String currAnimation = spectre.GetAnimation().CurrentAnimation();
            switch (currAnimation)
            {
                case "AwakeU":
                    spectre.SetSprite("SleepU");
                    break;
                case "AwakeD":
                    spectre.SetSprite("SleepD");
                    break;
                case "AwakeL":
                    spectre.SetSprite("SleepL");
                    break;
                case "AwakeR":
                    spectre.SetSprite("SleepR");
                    break;
            }
        }
    }
}
