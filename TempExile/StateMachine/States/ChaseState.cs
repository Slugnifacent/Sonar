using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ChaseState : State
    {
        // Target the player
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            if (spectre.GetTarget() != spectre.getCurrentUnit())
                spectre.ClearPath();
            if (spectre.dmgCooldown >= .75) {
                spectre.dmgCooldown = 0;
            }
            if (spectre.abilityCooldown > 1)
            {
                spectre.SetTarget(spectre.GetMap()[(int)player.position.X / MapUnit.MAX_SIZE, (int)player.position.Y / MapUnit.MAX_SIZE]);

                //Debugging for the spot the player is in.
                spectre.targetPosition = spectre.GetMap()[(int)player.position.X / MapUnit.MAX_SIZE, (int)player.position.Y / MapUnit.MAX_SIZE];
                if (spectre.GetTarget() != spectre.getCurrentUnit())
                    spectre.ClearPath();

                spectre.isMoving = true;

            }
            else
            {
                spectre.isMoving = false;
            }
        }

        // Assign speed to Spectre based on which one it is
        public override void doEntryAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            //The different chase speeds for the different types.
            spectre.isChasing = true;
            if (spectre.GetType() == typeof(Pride)) {
                spectre.speed = 400;
            }
            else if (spectre.GetType() == typeof(Stalker)) {
                spectre.speed = 300;
            }
            else if (spectre.GetType() == typeof(Wrath)) {
                spectre.speed = 300;
            }
            else {
                spectre.speed = 220;
            }
            spectre.fleeTimer = 0;
        }

        // Sets the speed back just in case and resets the boolean.
        public override void doExitAction(Spectre spectre, Player player)
        {
            spectre.isChasing = false;
            //spectre.speed = 100;
        }
    }
}
