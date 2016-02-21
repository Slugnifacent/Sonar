using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AlertedTimeCondition : Condition
    {
        // 1 second timer to pause after spotting the player
        public override bool test(Spectre spectre, Player player)
        {
            return spectre.abilityCooldown > 0;
        }
    }
}
