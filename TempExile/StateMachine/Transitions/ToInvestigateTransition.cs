using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    class ToInvestigateTransition : Transition
    {
        public ToInvestigateTransition(State s) : base(s) {}

        public override void doAction(Spectre spectre, Player player)
        {
            throw new NotImplementedException();
        }
    }
}
