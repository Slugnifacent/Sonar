using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class PossessState : State
    {
        // Bind the Spectre's position to that of the player
        public override void doAction(Spectre spectre, Player player)
        {
            spectre.position = player.position;
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            spectre.possessing = true;                       
            Game1.camera.Shake(10f, 1.0f);
            player.Reveal();
            spectre.fleeTimer = 0;
            return;
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player)
        {
            spectre.possessing = false;
            spectre.inPlayer = false;
            //Resetting the color and other possess things.
            player.exorcismPhrase.ACTIVE = false;
            player.setSlowDown(false);
            VisionManager.setVisionColor(GameColor.CornflowerBlue);
            VisionManager.addVisionPoint(player.position, 300f, true);
            player.SetPossessor(Player.possessor.none);
            //player.SetPossessor(Player.possessor.none);
            return;
        }
    }
}
