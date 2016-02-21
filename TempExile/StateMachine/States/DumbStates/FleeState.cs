using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class FleeState : State
    {
        int randX, randY;
        Condition atTarg = new AtTargetCondition();
        MapUnit myTarg;
        int fleeSoundTimer;
        int fleeSoundTimerReset = 100;

        // Run away!
        public override void doAction(Spectre spectre, Player player)
        {
            if (spectre.GetType() == typeof(Stalker) || spectre.GetType() == typeof(Pride)) {
                // If the spot for the spectre to flee to is inaccessible, then it will find a new spot.
                if (spectre.GetPath() == null) {
                    myTarg = spectre.getCurrentUnit();
                    spectre.SetTarget(myTarg);
                    //spectre.FindPath();
                    //spectre.ClearPath();
                }
                // When it reaches its target, it will find a new random spot.
                if (atTarg.test(spectre, player)) {
                    randX = Game1.random.Next(0, spectre.GetMap().GetUpperBound(0));
                    randY = Game1.random.Next(0, spectre.GetMap().GetUpperBound(1));
                    //Console.WriteLine(randX + " " + randY);
                    myTarg = spectre.GetMap()[randX, randY];
                    if (myTarg.isWalkable && GameVector2.Distance(myTarg.GetPosition(), spectre.position) > 100) {
                        spectre.SetTarget(spectre.GetMap()[randX, randY]);
                        spectre.ClearPath();
                    }
                    else {
                        spectre.SetTarget(spectre.getCurrentUnit());
                        doAction(spectre, player);
                    }
                }
            }
            else {
                if (spectre.getCurrentUnit() == spectre.GetTarget()) {
                    spectre.ClearPath();
                    if (spectre.GetCurrentPathIndex() + 1 < spectre.patrolPaths.Count) {
                        spectre.SetCurrentPathIndex(spectre.GetCurrentPathIndex() + 1);
                        spectre.SetTarget(spectre.GetPatrolPath()[spectre.GetCurrentPathIndex()]);
                    }
                    else {
                        spectre.SetCurrentPathIndex(0);
                        spectre.SetTarget(spectre.GetPatrolPath()[0]);
                    }
                    spectre.ClearPath();
                }
                if (spectre.FindNext() == null || !spectre.FindNext().isWalkable) {
                    //spectre.FindPath();
                    spectre.ClearPath();
                }
            }

            fleeSoundTimer--;
            if (fleeSoundTimer > 0)
            {
                spectre.storeSound(300);
            }
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {

            Metrics.getInstance().addMetric("Spectre run", Player.getInstance().gethealth(), null, spectre.position);
            fleeSoundTimer = fleeSoundTimerReset;
            //Increases the spectres speed and goes 1 patrol node up from the previous spot it was going to.
            spectre.speed = 250;
            spectre.fleeTimer = 0;
            spectre.ClearPath();

            

            //If dumb, they go to next next patrol point, or back to start.
            if (spectre.GetType() == typeof(Dumb)) {
                if (spectre.GetCurrentPathIndex() + 1 < spectre.patrolPaths.Count) {
                    spectre.SetCurrentPathIndex(spectre.GetCurrentPathIndex() + 1);
                    spectre.SetTarget(spectre.GetPatrolPath()[spectre.GetCurrentPathIndex()]);
                }
                else {
                    spectre.SetCurrentPathIndex(0);
                    spectre.SetTarget(spectre.GetPatrolPath()[0]);
                }
            }
            else {
                // If the spot for the spectre to flee to is inaccessible, then it will find a new spot.
                if (spectre.GetPath() == null) {
                    myTarg = spectre.getCurrentUnit();
                    spectre.SetTarget(myTarg);
                    //spectre.FindPath();
                    //spectre.ClearPath();
                }
                // When it reaches its target, it will find a new random spot.
                if (atTarg.test(spectre, player)) {
                    randX = Game1.random.Next(0, spectre.GetMap().GetUpperBound(0));
                    randY = Game1.random.Next(0, spectre.GetMap().GetUpperBound(1));
                    //Console.WriteLine(randX + " " + randY);
                    myTarg = spectre.GetMap()[randX, randY];
                    if (myTarg.isWalkable && GameVector2.Distance(myTarg.GetPosition(), spectre.position) > 100) {
                        spectre.SetTarget(spectre.GetMap()[randX, randY]);
                        spectre.ClearPath();
                    }
                    else {
                        spectre.SetTarget(spectre.getCurrentUnit());
                        doEntryAction(spectre, player);
                    }
                }
            }
        }

        // Return to normal Spectre speed
        public override void doExitAction(Spectre spectre, Player player)
        {
            spectre.speed = 100;
            spectre.fleeTimer = 0;
        }
    }
}
