using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class DumbPossessState : PossessState
    {
        public override void doEntryAction(Spectre spectre, Player player)
        {
            base.doEntryAction(spectre, player);
            Metrics.getInstance().addMetric("Dumb possession", Player.getInstance().gethealth(), null, spectre.position);
            player.SetPossessor(Player.possessor.Dumb);
        }

        // Excecute possession effects on player
        public override void doAction(Spectre spectre, Player player)
        {
            base.doAction(spectre, player);
            player.decreaseMovementSpeed();

            // Regulates speed in which player loses health
            if (spectre.dmgCooldown >= 2.5)
            {
                player.LoseHealth(20);
                spectre.dmgCooldown = 0;
            }
        }
    }
}
