using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class FinishedSearchingCondition : Condition
    {
        // Determines if Spectre has finished investigating a sound or player sighting
        public override bool test(Spectre spectre, Player player)
        {
            //Console.Out.WriteLine("Finished Searching for Player");
            return spectre.finishedSearching == true; ;
        }
    }
}
