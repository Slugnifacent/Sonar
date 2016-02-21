using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class PlayerHidingCondition : Condition
    {
        // Determines if the player in within a hiding spot
        public override bool test(Spectre spectre, Player player)
        {
            return player.isHiding;
        }
    }
}
