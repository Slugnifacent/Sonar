using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class SoundHeardCondition : Condition
    {
        // Determines if the given Spectre has heard a sound
        public override bool test(Spectre spectre, Player player)
        {
            if (spectre.playerBeingHeard)//|| spectre.objectHeard)
            {
                //Console.Out.WriteLine("Player was heard");
                return true;
            }
            if (spectre.objectHeard)
            {
                //Console.Out.WriteLine("Object was heard");
                return true;
            }
            return false;
        }
    }
}
