using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class PlayerInSirenRangeCondition : Condition {
        // Determines if Spectre has player in its line of sight
        public override bool test(Spectre spectre, Player player) {
            //if (!player.isHiding)
            //{
            float angle = Util.getInstance().Angle(spectre.orientation, player.position - spectre.position);
            List<GameVector2> coordinates = spectre.componentIntercept(spectre.position, player.position);
            if (coordinates.Count() != 0) {
                // If there is a solid object blocking line of sight, the player is not visible
                foreach (GameVector2 coord in coordinates) {
                    if (!spectre.GetMap()[(int)coord.X, (int)coord.Y].isSeeThrough) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

