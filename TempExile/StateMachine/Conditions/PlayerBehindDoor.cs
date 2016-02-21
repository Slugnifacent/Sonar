using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class PlayerBehindDoorCondition : Condition {
        // Check if player is behind a door to the spectre.
        public override bool test(Spectre spectre, Player player) {
            if (spectre.GetPath() == null) return false;
            foreach (MapUnit m in spectre.GetPath()) {
                if (m.getObject().GetType() == typeof(Door)) {
                    foreach (Door d in spectre.doors) {
                        if (d.getIndex().X == m.x && d.getIndex().Y == m.y ||
                                    d.getOtherHalfOfDoorIndex().X == m.x && d.getOtherHalfOfDoorIndex().Y == m.y) {
                            if (!d.isOpen && !d.locked) {
                                spectre.theDoor = m;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
