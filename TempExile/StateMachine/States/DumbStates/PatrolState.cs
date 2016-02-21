using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class PatrolState : State
    {
        // Walk to predetermined patrol points
        public override void doAction(Spectre spectre, Player player)
        {
            //How long they stay neutralized.
            if (spectre.neutralized && spectre.neutralizedTimer > 4)
                spectre.neutralized = false;
            if (spectre.getCurrentUnit() != spectre.GetTarget())
            {
                // If Spectre cannot access the next point it wishes to approach through a certain path, do not take the path
                if (spectre.FindNext() == null || !spectre.FindNext().isWalkable) {
                    spectre.ClearPath();
                }
            }
            else
            {
                if (spectre.GetCurrentPathIndex() == spectre.GetPatrolPath().Count - 1)
                {
                    spectre.SetCurrentPathIndex(0);
                }
                else
                {
                    spectre.SetCurrentPathIndex(spectre.GetCurrentPathIndex() + 1);
                }
                spectre.ClearPath();
                spectre.SetTarget(spectre.GetPatrolPath()[spectre.GetCurrentPathIndex()]);
            }   
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {

            Metrics.getInstance().addMetric("Dumb patrol", Player.getInstance().gethealth(), null, spectre.position);
            Console.Out.WriteLine("Entering Patrol");
            spectre.fleeTimer = 0;
            spectre.speed = 100;
            spectre.behindDoor = false;
            //spectre.SetTarget(spectre.getCurrentUnit());
            //spectre.ClearPath();
            return;
        }

        public override void doExitAction(Spectre spectre, Player player)
        {
            return;
        }
    }
}
