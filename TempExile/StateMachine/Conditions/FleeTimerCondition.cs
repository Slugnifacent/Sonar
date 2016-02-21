using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class FleeTimerCondition : Condition {
        //Determines if the spectre has been fleeing for long enough.
        public override bool test(Spectre spectre, Player player) {
            return spectre.fleeTimer > 3;
        }
    }
}
