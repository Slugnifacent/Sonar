using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class WakeUpTransition : Transition
    {
        // Transition to go from sleeping to awake. Checks if a sound is heard or if player collides with Siren.
        public WakeUpTransition(State s)
            : base(s)
        {
            condition = new OrCondition(new SoundHeardCondition(), new CollidingCondition());
        }

        // Transition to alert state when a sound is heard nearby or the player is colliding with the given Spectre
        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("wake up");
            //spectre.SetSprite("Awake");
            LookAtPlayer (spectre, player);
            return;
        }

        /// <summary>
        /// Steven Ekejiuba 5/15/2012
        /// Spectre faces the general direction that the player is in
        /// </summary>
        /// <param name="spectre"></param>
        /// <param name="player"></param>
        public void LookAtPlayer(Spectre spectre, Player player)
        {
            // Player is not to the left or right of Spectre
            if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height / 2 &&
                player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height / 2)
            {
                // Player is to right of Spectre
                if (player.getCurrPos().X > spectre.getCurrPos().X + spectre.getBox().Width / 2)
                {
                    // Player is to the northeast of Spectre
                    if (player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeU");
                        spectre.orientation = new GameVector2(0, -1);
                    }
                    // Player is to the southeast of Spectre
                    else if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeD");
                        spectre.orientation = new GameVector2(0, 1);
                    }
                }
                // Player is to left of Spectre
                else if (player.getCurrPos().X < spectre.getCurrPos().X - spectre.getBox().Width / 2)
                {
                    // Player is to the northwest of Spectre
                    if (player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeU");
                        spectre.orientation = new GameVector2(0, -1);
                    }
                    // Player is to the southwest of Spectre
                    else if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeD");
                        spectre.orientation = new GameVector2(0, 1);
                    }
                }
                else
                {
                    // Player is to the right of Spectre
                    if (player.getCurrPos().X > spectre.getCurrPos().X)
                    {
                        spectre.SetSprite("AwakeR");
                        spectre.orientation = new GameVector2(1, 0);
                    }
                    // Player is to the left of Spectre
                    else if (player.getCurrPos().X < spectre.getCurrPos().X)
                    {
                        spectre.SetSprite("AwakeL");
                        spectre.orientation = new GameVector2(-1, 0);
                    }
                }
            }
        }
    }
}
