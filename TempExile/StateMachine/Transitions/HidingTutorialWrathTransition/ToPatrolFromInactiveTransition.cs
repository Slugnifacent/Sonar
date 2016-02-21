using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToPatrolFromInactiveTransition : Transition
    {
        //
        public ToPatrolFromInactiveTransition(State s)
            : base(s)
        {
            condition = new ActivateCondition();
        }

        // 
        public override void doAction(Spectre spectre, Player player)
        {
            spectre.active = true;
        }
    }
}
