using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class WanderState : State
    {
        int randX, randY;
        Condition atTarg = new AtTargetCondition();
        MapUnit myTarg;
        public override void doAction(Spectre spectre, Player player)
        {
            //How long they stay neutralized.
            if (spectre.neutralized && spectre.neutralizedTimer > 4)
                spectre.neutralized = false;
            // If the spot for the spectre to wander to is inaccessible, then it will find a new spot.
            if (spectre.GetPath() == null)
            {
                myTarg = spectre.getCurrentUnit();
                spectre.SetTarget(myTarg);
                //spectre.FindPath();
                //spectre.ClearPath();
            }
            // When it reaches its target, it will find a new random spot.
            if (atTarg.test(spectre, player))
            {
                randX = Game1.random.Next(0, spectre.GetMap().GetUpperBound(0));
                randY = Game1.random.Next(0, spectre.GetMap().GetUpperBound(1));
                //Console.WriteLine(randX + " " + randY);
                myTarg = spectre.GetMap()[randX, randY];
                if (myTarg.isWalkable)
                {
                    spectre.SetTarget(spectre.GetMap()[randX, randY]);
                    spectre.ClearPath();
                }
                else
                {
                    spectre.SetTarget(spectre.getCurrentUnit());
                    doAction(spectre, player);
                }
            }
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            Metrics.getInstance().addMetric("Stalker Wander", Player.getInstance().gethealth(), null, spectre.position);
            spectre.speed = 100;
        }

        public override void doExitAction(Spectre spectre, Player player)
        {
        }
    }
}
