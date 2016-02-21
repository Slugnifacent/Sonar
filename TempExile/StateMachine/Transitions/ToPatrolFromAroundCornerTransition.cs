using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class ToPatrolFromAroundCornerTransition : Transition {
        //Player not in range to break door but can't make it to target anymore.
        PlayerInSightCondition sight = new PlayerInSightCondition();
        public ToPatrolFromAroundCornerTransition(State s)
            : base(s) {
            condition = new PlayerBehindDoorCondition();
        }

        // Transition to chase after having paused from alerted
        public override void doAction(Spectre spectre, Player player) {
            spectre.behindDoor = false;
            spectre.SetTarget(spectre.patrolPaths[0]);
            spectre.ClearPath();
            return;
        }

        // Condition checked to see if the player is no longer close or in sight, then it will go to investigate.
        public override bool isTriggered(Spectre spectre, Player player) {
            if (!sight.test(spectre, player)) {
                return condition.test(spectre, player);
            }
            else {
                return false;
            }
        }
    }
}
