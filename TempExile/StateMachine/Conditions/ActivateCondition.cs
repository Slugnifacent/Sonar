using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class ActivateCondition : Condition
    {
        // Check if Spectre has reached destination
        public override bool test(Spectre spectre, Player player)
        {
            return spectre.GetMap()[((int)player.position.X) / MapUnit.MAX_SIZE, ((int)player.position.Y) / MapUnit.MAX_SIZE].getObject().GetType() == typeof(SpectreActivate);//player
        }
    }
}
