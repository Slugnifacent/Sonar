using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class ToPossessTransition : Transition
    {
        // Transition to from chase to possess.
        public ToPossessTransition(State s)
            : base(s)
        {
            condition = new CollidingCondition();
        }

        // Transition to possess when the given Spectre is colliding with the player
        public override void doAction(Spectre spectre, Player player)
        {
            Console.WriteLine("to possess");
            return;
        }
    }
}
