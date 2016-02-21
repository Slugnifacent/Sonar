using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class OrCondition : Condition
    {
        Condition condA, condB;

        //Allows two conditions to be put with an Or for the one transition.
        public OrCondition(Condition a, Condition b)
        {
            condA = a;
            condB = b;
        }

        public override bool test(Spectre spectre, Player player)
        {
            return (condA.test(spectre, player) || condB.test(spectre, player));
        }
    }
}
