using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class ToBreakDoorFromAroundCornerTransition : Transition {
        //Player just went behind a door.
        public ToBreakDoorFromAroundCornerTransition(State s)
            : base(s) {
            condition = new AndCondition(new PlayerBehindDoorCondition(), new VeryCloseCondition());
        }

        // Transition to chase after having paused from alerted
        public override void doAction(Spectre spectre, Player player) {
            Console.WriteLine("Gonna break door from the corner state.");
            //spectre.behindDoor = true;
            return;
        }

        // Condition checked to see if the player is no longer close or in sight, then it will go to investigate.
        public override bool isTriggered(Spectre spectre, Player player) {
            if (spectre.fleeTimer < 1) {
                return condition.test(spectre, player);
            }
            else {
                return false;
            }
        }
    }
}
