using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    class SoundBehindDoorCondition : Condition {
        // Check if sound heard to investigate is behind a door to the spectre.
        public override bool test(Spectre spectre, Player player) {
            spectre.SetTarget(spectre.investigateSpot);
            //spectre.targetPosition = spectre.GetTarget();
            spectre.FindPath();
            if (spectre.GetPath() == null) {
                spectre.holdTempPath();
                spectre.setWalkable();
                spectre.SetTarget(spectre.investigateSpot);
                spectre.FindPath();
                spectre.unSetWalkable();
                spectre.SetTarget(spectre.investigateSpot);
                if (spectre.GetPath() != null && spectre.tempPath == null) {
                    return true;
                }
            }
            return false;
        }
    }
}
