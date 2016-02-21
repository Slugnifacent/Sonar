using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class AlertState : State
    {
        // If player is too close to the Spectre, damage the player
        public override void doAction(Spectre spectre, Player player)
        {
            //spectre.SetAlertCue("sirenScream");
            spectre.playAlertCue();
            GameScreen.shaders[0].addPoint(spectre.position, 500f, 1900f, 1f);
            if (spectre.dmgCooldown >= 1 && GameVector2.Distance(spectre.position, player.position) < 72)
            {
                player.LoseHealth(15);
                spectre.dmgCooldown = 0;
            }
        }
        //Sets ability cooldown to 0 so it can start counting up to the amount of time required.
        public override void doEntryAction(Spectre spectre, Player player)
        {
            spectre.abilityCooldown = 0;
            return;
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player)
        {
            return;
        }
    }
}
