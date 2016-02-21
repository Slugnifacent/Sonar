using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToPatrolTransition : Transition
    {
        // Transition to go from investigate to patrol. This is done after the spectre is done with the searching part of investigate.
        public ToPatrolTransition(State s)
            : base(s)
        {
            condition = new FinishedSearchingCondition();
        }

        // When given Spectre has finished investigating, change speed to patrol speed and transition to patrol
        public override void doAction(Spectre spectre, Player player)
        {
            spectre.speed = 100;
            Console.WriteLine("to patrol");
            return;
        }
    }
}
