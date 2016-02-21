using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToInvestigateFromAroundCornerTransition : Transition
    {
        //The transition given when the spectre has lost sight of the player after chasing around a corner and is now investigating.
        public ToInvestigateFromAroundCornerTransition(State s)
            : base(s)
        {
            condition = new AtTargetCondition();
        }

        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("to investigate from chase around corner");
            spectre.goingToInvestigate = true;
            Player.getInstance().beingSearchedFor = true;
            return;
        }

        // Condition checked to see if the player is no longer close or in sight, then it will go to investigate.
        public override bool isTriggered(Spectre spectre, Player player)
        {
            return condition.test(spectre, player);
        }
    }
}
