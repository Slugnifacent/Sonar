using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class StalkerToInvestigateFromStalkTransition : Transition
    {
        //Transition to go to investigate from stalking.
        public StalkerToInvestigateFromStalkTransition(State s)
            : base(s)
        {
            //condition = new OrCondition(new PlayerInSightCondition(), new AndCondition(new PlayerFacingCondition(), new PlayerCloseCondition()));
            condition = new OrCondition(new PlayerInSightCondition(), new PlayerFacingCondition());
        }

        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingSearchedFor = true;
            //Console.WriteLine("to invest from stalk");
            return;
        }

        // Transition to investigate if the player is not the given Spectre's line of sight and the player is not facing the given Spectre
        public override bool isTriggered(Spectre spectre, Player player)
        {
            return !condition.test(spectre, player);
        }
    }
}
