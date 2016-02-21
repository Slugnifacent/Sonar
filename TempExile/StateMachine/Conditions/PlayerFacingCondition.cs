using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class PlayerFacingCondition : Condition
    {
        // Determines if the player has Spectre in her line of sight
        public override bool test(Spectre spectre, Player player)
        {
            float angle = Util.getInstance().Angle(player.facing, spectre.position - player.position);
            if (angle < 60)
                return true;
            return false;
        }
    }
}

