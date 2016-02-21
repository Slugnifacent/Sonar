using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class StalkState : State
    {
        public override void doAction(Spectre spectre, Player player)
        {
            if (GameVector2.Distance(player.position, spectre.position) >= 180)
                return;
            MapUnit curr = spectre.getCurrentUnit();
            float highDist, currDist;
            MapUnit furthTarg = curr;
            MapUnit myTarg = curr;
            highDist = currDist = GameVector2.Distance(player.position, spectre.position);
            for (int i = 0; i < curr.neighbors.Count; i++)
            {
                currDist = GameVector2.Distance(player.position, curr.neighbors[i].GetPosition());
                if (curr.neighbors[i].isWalkable && currDist > highDist)
                {
                    highDist = currDist;
                    furthTarg = curr.neighbors[i];
                }
            }
            // Moves around the player, and tries to reposition every second.
            if (spectre.abilityCooldown > 1)
            {
                int i;
                
                //MapUnit furthTarg;
                //MapUnit myTarg = curr;// furthTarg = curr;
                float highAngle, currAngle;//, highDist, currDist;
                /*for (i = 0; i < curr.neighbors.Count; i++)
                {
                    if (curr.neighbors[i].isWalkable)// && GameVector2.Distance(player.position, curr.neighbors[i].GetPosition()) > GameVector2.Distance(player.position, curr.GetPosition()))
                    {
                        myTarg = curr.neighbors[i];
                        break;
                    }
                }*/
                highAngle = currAngle = Util.getInstance().Angle(player.facing, curr.GetPosition() - player.position);
                
                for (i = 0; i < curr.neighbors.Count; i++)
                {
                    currAngle = Util.getInstance().Angle(player.facing, curr.neighbors[i].GetPosition() - player.position);
                    if (curr.neighbors[i].isWalkable && currAngle > highAngle && GameVector2.Distance(player.position, curr.neighbors[i].GetPosition()) > GameVector2.Distance(player.position, curr.GetPosition()))
                    {
                        highAngle = currAngle;
                        myTarg = curr.neighbors[i];
                    }
                    
                }
                if (myTarg == curr)
                    myTarg = furthTarg;
                if (myTarg != curr)
                {
                    spectre.SetTarget(myTarg);
                    spectre.ClearPath();
                }
                spectre.abilityCooldown = 0;
            }
            if (GameVector2.Distance(player.position, spectre.position) < GameVector2.Distance(player.positionPrevious, spectre.position))
                myTarg = furthTarg;
            if (myTarg != curr)
            {
                spectre.SetTarget(myTarg);
                spectre.ClearPath();
            }
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            Metrics.getInstance().addMetric("Stalker Stalk", Player.getInstance().gethealth(), null, spectre.position);
            spectre.abilityCooldown = 0;
            spectre.SetTarget(spectre.getCurrentUnit());
            spectre.ClearPath();
            spectre.speed = 100;
            spectre.isChasing = false;
        }

        public override void doExitAction(Spectre spectre, Player player)
        {

        }
    }
}
