using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    public class OpenDoorInvestigateState : State {
        // Opens the door when it is near it.
        int count = 0;
        public override void doAction(Spectre spectre, Player player) {
            Player.getInstance().beingSearchedFor = true;
            /*if (spectre.getCurrentUnit() != spectre.GetTarget()) {
                if (spectre.FindNext() == null || !spectre.FindNext().isWalkable) {
                    spectre.ClearPath();
                    spectre.SetTarget(spectre.theDoor.neighbors[count]);
                    spectre.FindPath();
                    count++;
                }
            }
            else {*/
            if (spectre.getCurrentUnit() == spectre.GetTarget()) {
                spectre.nextToDoor = true;
                //spectre.behindDoor = false;
                spectre.returningFromOpenDoor = true;
            }
        }

        public override void doEntryAction(Spectre spectre, Player player) {
            spectre.unSetWalkable();
            if (spectre.theDoor.neighbors[1].isWalkable) {
                spectre.SetTarget(spectre.theDoor.neighbors[1]);
                spectre.FindPath();
                if (spectre.GetPath() == null) {
                    spectre.SetTarget(spectre.theDoor.neighbors[5]);
                }
            }
            else {
                spectre.SetTarget(spectre.theDoor.neighbors[3]);
                spectre.FindPath();
                if (spectre.GetPath() == null) {
                    spectre.SetTarget(spectre.theDoor.neighbors[7]);
                }
            }
            spectre.dmgCooldown = 0;
            spectre.isChasing = true;
            spectre.setWalkable();
            spectre.ClearPath();
        }

        public override void doExitAction(Spectre spectre, Player player) {
            spectre.isChasing = false;
            return;
        }
    }
}
