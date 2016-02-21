using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToPatrolFromFleeTransition : Transition
    {
        // The transition for the spectre to start going back to patrol after it has ran away for a set amount of time or gotten
        // far enough away.
        public ToPatrolFromFleeTransition(State s)
            : base(s)
        {
            //condition = new OrCondition(new FleeTimerCondition(), new AtTargetCondition());
            condition = new FleeTimerCondition();
        }

        // Transition to patrol when the given Spectre has reached a set destination, slightly redundant but back to patrol speed.
        public override void doAction(Spectre spectre, Player player)
        {
            spectre.speed = 100;
            Console.WriteLine("to patrol");
            return;
        }
    }
}
