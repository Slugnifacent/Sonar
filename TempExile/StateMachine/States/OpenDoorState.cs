using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    public class OpenDoorState : State {
        // Opens the door when it is near it.
        int openDoorTime = Game1.random.Next(1, 3);
        Door door;
        public override void doAction(Spectre spectre, Player player) {
            if (spectre.getCurrentUnit() != spectre.GetTarget()) {
                spectre.fleeTimer = 0;
            }
            else {
                spectre.ClearPath();
                door = (Door)spectre.theDoor.getObject();
                if (door.isOpen || door.isBroken) {
                    spectre.behindDoor = false;
                }
                else if (spectre.fleeTimer > openDoorTime) {
                    spectre.nextToDoor = true;
                    spectre.behindDoor = true;
                }
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
            spectre.fleeTimer = 0;
            spectre.isChasing = true;
            spectre.setWalkable();
            spectre.ClearPath();
        }

        public override void doExitAction(Spectre spectre, Player player) {
            spectre.theDoor = null;
            spectre.SetTarget(spectre.getCurrentUnit());
            spectre.ClearPath();
            spectre.isChasing = false;
        }
    }
}