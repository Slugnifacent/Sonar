using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AlertedState : State
    {
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
        }

        // causes the spectre to pause after spotting the player
        public override void doEntryAction(Spectre spectre, Player player)
        {
            spectre.active = true;
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            spectre.SetTarget(spectre.getCurrentUnit());

            //Debugging for the spot the player is in.
            spectre.targetPosition = spectre.GetMap()[(int)player.position.X / MapUnit.MAX_SIZE, (int)player.position.Y / MapUnit.MAX_SIZE];
            
            spectre.ClearPath();
            spectre.alertedCount++;
        }

        // Sets the speed back just in case and resets the boolean.
        public override void doExitAction(Spectre spectre, Player player)
        {
        }
    }
}
