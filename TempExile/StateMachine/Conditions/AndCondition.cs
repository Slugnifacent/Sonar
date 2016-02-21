using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class AndCondition : Condition
    {
        Condition condA, condB;

        //This Condition is used to see if two different conditions are true for the same transition.
        public AndCondition(Condition a, Condition b)
        {
            condA = a;
            condB = b;
        }

        public override bool test(Spectre spectre, Player player)
        {
            return (condA.test(spectre, player) && condB.test(spectre, player));
        }
    }
}
