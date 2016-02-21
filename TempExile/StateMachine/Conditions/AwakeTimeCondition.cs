using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AwakeTimeCondition : Condition
    {
        //Determine if the siren has been awake long enough.
        public override bool test(Spectre spectre, Player player)
        {
            return spectre.abilityCooldown > 5;
        }
    }
}
