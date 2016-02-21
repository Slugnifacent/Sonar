using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    /// <summary>
    /// Derrick
    /// State for spectres chasing around corners
    /// </summary>
    class ChaseAroundCornerState : State
    {
        public override void doAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
        }

        // Assign speed to Spectre based on which one it is
        public override void doEntryAction(Spectre spectre, Player player)
        {
            Player.getInstance().beingChased = Player.getInstance().beingSearchedFor = true;
            Console.WriteLine("Around Corner.");
            //The different chase speeds for the different types.
            spectre.isChasing = true;
            if (spectre.GetType() == typeof(Pride))
            {
                spectre.speed = 400;
            }
            else if (spectre.GetType() == typeof(Stalker))
            {
                spectre.speed = 300;
            }
            else if (spectre.GetType() == typeof(Wrath))
            {
                spectre.speed = 300;
            }
            else
            {
                spectre.speed = 220;
            }

            MapUnit closest = spectre.getCurrentUnit();
            float closestDist = GameVector2.Distance(closest.GetPosition(), player.position);
            MapUnit blockWall = null;

            // when spectre loses sight of the player around a corner, determines which wall cut off its sight
            List<GameVector2> coordinates = spectre.componentIntercept(spectre.position, player.position);
            if (coordinates.Count() != 0)
            {
                // If there is a solid object blocking line of sight, the player is not visible
                foreach (GameVector2 coord in coordinates)
                {
                    if (!spectre.GetMap()[(int)coord.X, (int)coord.Y].isWalkable)
                    {
                        blockWall = spectre.GetMap()[(int)coord.X, (int)coord.Y];
                        break;
                    }
                }
            }

            // determines which of the wall's neighbors is closest to the player and sets as the target, chasing around the corner
            if (blockWall != null)
            {
                foreach (MapUnit m in blockWall.neighbors)
                {
                    if (m.isWalkable)
                    {
                        float dist = GameVector2.Distance(m.GetPosition(), player.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closest = m;
                        }
                    }
                }
            }

            spectre.SetTarget(closest);
            spectre.targetPosition = spectre.GetMap()[(int)closest.x, (int)closest.y];

            spectre.ClearPath();
        }

        // Sets the speed back just in case and resets the boolean.
        public override void doExitAction(Spectre spectre, Player player)
        {
            spectre.isChasing = false;
            //spectre.speed = 100;
        }
    }
}
