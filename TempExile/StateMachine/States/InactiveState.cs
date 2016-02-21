using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class InactiveState : State
    {
        
        public override void doAction(Spectre spectre, Player player)
        {
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            spectre.active = false;
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player)
        {
        }
    }
}
