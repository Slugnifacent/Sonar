using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class CollidingCondition : Condition
    {
        // Determine if Spectre is colliding with player
        public override bool test(Spectre spectre, Player player)
        {
            return spectre.inPlayer;
        }
    }
}
