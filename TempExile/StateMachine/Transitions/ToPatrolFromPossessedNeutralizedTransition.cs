using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToPatrolFromPossessedNeutralizedTransition : Transition
    {
        //if player says proper word to neutralize spectre
        public ToPatrolFromPossessedNeutralizedTransition(State s)
            : base(s)
        {
            condition = new NeutralizedCondition();
        }

        // neutralizes spectre for 8s
        public override void doAction(Spectre spectre, Player player)
        {
            spectre.ClearPath();
            spectre.neutralizedTimer = 0;
            spectre.neutralized = true;
            return;
        }

        // Can only be taken out of the player after at least a second has passed.
        public override bool isTriggered(Spectre spectre, Player player) {
            if (spectre.fleeTimer > .75) {
                return condition.test(spectre, player);
            }
            else {
                return false;
            }
        }
    }
}
