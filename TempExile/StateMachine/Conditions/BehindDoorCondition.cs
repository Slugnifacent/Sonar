using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class BehindDoorCondition : Condition {
        // Check if spectre is behind a door.
        public override bool test(Spectre spectre, Player player) {
            return spectre.behindDoor;
        }
    }
}
