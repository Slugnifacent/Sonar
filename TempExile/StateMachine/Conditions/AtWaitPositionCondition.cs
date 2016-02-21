using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AtWaitPositionCondition : Condition
    {
        // Check if Spectre has reached destination
        public override bool test(Spectre spectre, Player player)
        {
            return (spectre.getCurrentUnit().x == 3 && spectre.getCurrentUnit().y == 2);
        }
    }
}
