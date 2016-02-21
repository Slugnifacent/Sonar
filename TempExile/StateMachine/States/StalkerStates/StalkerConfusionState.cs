using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class StalkerConfusionState : PossessState
    {
        public override void doEntryAction(Spectre spectre, Player player)
        {
            Metrics.getInstance().addMetric("Stalker Confusion", Player.getInstance().gethealth(), null, spectre.position);
            base.doEntryAction(spectre, player);
            player.ConfusionEffect();
            player.SetPossessor(Player.possessor.Stalker);
        }

        // Depending on value for dmgCooldown either damage the player or randomly choose a confusion effect to apply to her
        public override void doAction(Spectre spectre, Player player)
        {
            base.doAction(spectre, player);
            // Damages the player if they make too loud of a noise.
            if (player.GetNoise() >= Player.loudness.talking && spectre.dmgCooldown >= 2) {
                player.LoseHealth(20);
                spectre.dmgCooldown = 0;
            }
            // Does a new confusion effecdt every 3 seconds.
            /*if (spectre.abilityCooldown >= 3) {
                spectre.randomAction = Game1.random.Next(0, 5);
                player.ConfusionEffect(spectre.randomAction);
                spectre.abilityCooldown = 0;
            }*/
        }

        // Resets the confusion.
        public override void doExitAction(Spectre spectre, Player player) {
            base.doExitAction(spectre, player);
            player.resetConfusion();
        }
    }
}
