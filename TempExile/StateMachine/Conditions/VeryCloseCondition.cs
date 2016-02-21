using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class VeryCloseCondition : Condition {
        // Determines if the player is near the given Spectre
        public override bool test(Spectre spectre, Player player) {
            if (GameVector2.Distance(player.position, spectre.position) < 200)
                return true;
            return false;
        }
    }
}
