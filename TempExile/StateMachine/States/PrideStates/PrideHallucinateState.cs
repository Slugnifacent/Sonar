using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    public class PrideHallucinateState : PossessState {
        public override void doEntryAction(Spectre spectre, Player player) {
            Metrics.getInstance().addMetric("Pride Hallucinate", Player.getInstance().gethealth(), null, spectre.position);
            base.doEntryAction(spectre, player);
            spectre.dmgCooldown = 0;
            player.SetPossessor(Player.possessor.Pride);
        }

        // Depending on value of Spectre's dmgCooldown either damage the player,
        // make her hallucinate, make her see objects that do not exist, or change the player's footstep sounds
        public override void doAction(Spectre spectre, Player player) {
            base.doAction(spectre, player);
            //Pure Damage, will do this once 15 seconds has passed.
            if (spectre.dmgCooldown >= 25) {
                player.LoseHealth(75);
                spectre.dmgCooldown = 0;
            }
            // Does a random hallucination every 3 seconds.
            if (spectre.abilityCooldown >= 5) {
                spectre.randomAction = Game1.random.Next(0, 6);
                player.Hallucinate(spectre.randomAction);
                spectre.abilityCooldown = 0;
            }
            // Resets the fake pictures to get ready for the next hallucination.
            if (spectre.abilityCooldown >= 2 && spectre.randomAction == 1) {
                player.drawFake = false;
            }
            // This hallucination effect has to be taken care of here, the rest is in player.
            if (spectre.randomAction == 5) {
                spectre.HallucinateLightNoise(player);
            }
        }
        //Resets the hallucination effects before exiting.
        public override void doExitAction(Spectre spectre, Player player) {
            base.doExitAction(spectre, player);
            player.resetHallucinate();
        }
    }
}
