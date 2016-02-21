using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar {
    public class WrathWaitState : State {

        GameVector2 randVal;
        Condition atTarg = new AtTargetCondition();
        MapUnit myTarg;

        public override void doEntryAction(Spectre spectre, Player player) {
            Metrics.getInstance().addMetric("Wrath Wait", Player.getInstance().gethealth(), null, spectre.position);
            spectre.ClearPath();
            spectre.speed = 100;
        }

        //This will only grab a spot near Wrath, causing it to just essentially turn around every 3 seconds waiting for the player.
        public override void doAction(Spectre spectre, Player player) {
            // If the spot for the spectre to wander to is inaccessible, then it will find a new spot.
            if (spectre.GetPath() == null) {
                myTarg = spectre.getCurrentUnit();//spectre.GetMap()[(int)spectre.getCurrentUnit().x, (int)spectre.getCurrentUnit().y];
                spectre.SetTarget(myTarg);
                //spectre.FindPath();
                //spectre.ClearPath();
            }
            // When it reaches its target, it will find a new random spot.
            if (atTarg.test(spectre, player) && spectre.fleeTimer > 3) { //Using flee timer here because Wrath never flees and I need a timer.
                randVal = spectre.findValidRandomVal(spectre.homePosition, 6);
                //randX = Game1.random.Next((int)(spectre.homePosition.X - 10), (int)(spectre.homePosition.X + 10));
                //randY = Game1.random.Next((int)(spectre.homePosition.Y - 4), (int)(spectre.homePosition.Y + 4));
                //Console.WriteLine(randX + " " + randY);
                spectre.SetTarget(spectre.GetMap()[(int)randVal.X / MapUnit.MAX_SIZE, (int)randVal.Y / MapUnit.MAX_SIZE]);
                spectre.ClearPath();
                /*if (myTarg.isWalkable) {
                    spectre.SetTarget(spectre.GetMap()[randX / MapUnit.MAX_SIZE, randY / MapUnit.MAX_SIZE]);
                    spectre.ClearPath();
                }
                else {
                    spectre.SetTarget(spectre.getCurrentUnit());
                    doAction(spectre, player);
                }*/
                spectre.fleeTimer = 0;
            }
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player) {
        }
    }
}
