using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToInvestigateFromChaseTransition : Transition
    {
        //The transition given when the spectre has lost sight of the player while chasing and is now investigating.
        public ToInvestigateFromChaseTransition(State s)
            : base(s)
        {
            condition = new AndCondition(new PlayerCloseCondition(), new PlayerInSightCondition());
        }

        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingSearchedFor = true;
            //Console.WriteLine("to investigate from chase");
            spectre.fleeTimer = 0;
            return;
        }

        // Condition checked to see if the player is no longer close or in sight, then it will go to investigate.
        public override bool isTriggered(Spectre spectre, Player player)
        {
            //Flee timer used to make sure it still chases even after player runs out of sight.
            if (spectre.fleeTimer > 2) {
                return !condition.test(spectre, player);
            }
            else {
                return false;
            }
        }
    }
}
