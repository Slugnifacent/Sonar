using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class ToOpenDoorFromInvestigateTransition : Transition {
        //The transition given when the spectre has lost sight of the player while chasing and is now investigating.
        public ToOpenDoorFromInvestigateTransition(State s)
            : base(s) {
            condition = new AndCondition(new BehindDoorCondition(), new SoundBehindDoorCondition());
        }

        public override void doAction(Spectre spectre, Player player) {
            //Console.WriteLine("to investigate from chase");
            return;
        }

        // Condition checked to see if the player is no longer close or in sight, then it will go to investigate.
        public override bool isTriggered(Spectre spectre, Player player) {
            return condition.test(spectre, player);
        }
    }
}
