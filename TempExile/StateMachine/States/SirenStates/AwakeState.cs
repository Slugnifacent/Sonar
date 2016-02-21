using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class AwakeState : State
    {
        PlayerInSirenRangeCondition sight = new PlayerInSirenRangeCondition();
        // If player is too close to the Spectre, damage the player
        public override void doAction(Spectre spectre, Player player)
        {
            int currentTimeElapsed = getTimeElapsed() + 1;
            setTimeElapsed(currentTimeElapsed);

            LookAtPlayer(spectre, player);
            if (spectre.fleeTimer >= 1)
            {
                spectre.playAlertCue();
                spectre.fleeTimer = 0;
            }
            spectre.storeSound(1900f);
            if (spectre.dmgCooldown >= 1.25)
            {
                if (GameVector2.Distance(spectre.position, player.position) < MapUnit.MAX_SIZE * 3 && sight.test(spectre, player)) {
                    if (currentTimeElapsed > 100)
                    {
                        player.LoseHealth(15);
                    }
                    spectre.dmgCooldown = 0;
                }
            }
        }
        //Sets ability cooldown to 0 so it can start counting up to the amount of time required.
        public override void doEntryAction(Spectre spectre, Player player)
        {
            //LookAtPlayer(spectre, player);
            Metrics.getInstance().addMetric("Siren Awake", Player.getInstance().gethealth(), null, spectre.position);
            spectre.abilityCooldown = 0;
            spectre.fleeTimer = 0;
            spectre.alertedCount++;
            setTimeElapsed(0);
            return;
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player)
        {
            setTimeElapsed(0);
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
            if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height * 2 ||
                player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height * 2)
            {
                Console.Out.WriteLine (".");
                // Player is to right of Spectre
                if (player.getCurrPos().X > spectre.getCurrPos().X + spectre.getBox().Width / 2)
                {
                    // Player is to the northeast of Spectre
                    if (player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeD");
                        spectre.orientation = new GameVector2(0, -1);
                    }
                    // Player is to the southeast of Spectre
                    else if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeU");
                        spectre.orientation = new GameVector2(0, 1);
                    }
                }
                // Player is to left of Spectre
                else if (player.getCurrPos().X < spectre.getCurrPos().X - spectre.getBox().Width / 2)
                {
                    // Player is to the northwest of Spectre
                    if (player.getCurrPos().Y > spectre.getCurrPos().Y + spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeD");
                        spectre.orientation = new GameVector2(0, -1);
                    }
                    // Player is to the southwest of Spectre
                    else if (player.getCurrPos().Y < spectre.getCurrPos().Y - spectre.getBox().Height / 2)
                    {
                        spectre.SetSprite("AwakeU");
                        spectre.orientation = new GameVector2(0, 1);
                    }
                }
            }
            else /*if (player.getCurrPos().Y >= spectre.getCurrPos().Y - spectre.getBox().Height / 2 &&
                         player.getCurrPos().Y <= spectre.getCurrPos().Y + spectre.getBox().Height / 2)*/
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
