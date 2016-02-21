using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class StalkerToStalkTransition : Transition
    {
        // Transition to go to stalking from chase.
        public StalkerToStalkTransition(State s)
            : base(s)
        {
            condition = new AndCondition(new PlayerInSightCondition(), new PlayerFacingCondition());
        }

        // Transition to stalk when the player is in the given Spectre's line of sight and the player's orientation is toward this Spectre
        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("to stalk");
            return;
        }
    }
}
