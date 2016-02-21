using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ToWaitFromPatrolTransition : Transition
    {
        //
        public ToWaitFromPatrolTransition(State s)
            : base(s)
        {
            condition = new AtWaitPositionCondition();
        }

        // 
        public override void doAction(Spectre spectre, Player player)
        {
        }
    }
}
