using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class DoorInWayCondition : Condition {
        // Determines if there is a door between the spectre and the player. Currently unused.
        public override bool test(Spectre spectre, Player player) {
            float angle = Util.getInstance().Angle(spectre.orientation, player.position - spectre.position);
            if (angle < 50) {
                List<GameVector2> coordinates = spectre.componentIntercept(spectre.position, player.position);
                if (coordinates.Count() != 0) {
                    // If there is a solid object blocking line of sight, the player is not visible
                    foreach (GameVector2 coord in coordinates) {
                        if (!spectre.GetMap()[(int)coord.X, (int)coord.Y].isWalkable) {
                            spectre.SetTarget(spectre.GetMap()[(int)coord.X, (int)coord.Y]);
                            spectre.behindDoor = true;
                            return true;
                        }
                    }
                }
                return false;
            }

            //if (spectre.GetPath() != null) {
            //    foreach (MapUnit m in spectre.GetPath()) {
            //        if (m.getObject().GetType() == typeof(Door)) {
            //            if (!m.isWalkable) {
            //                Console.WriteLine("Found Door.");
            //                spectre.behindDoor = true;
            //                spectre.SetTarget(m);
            //                return true;
            //            }
            //        }
            //    }
            //}
            return false;
        }
    }
}
