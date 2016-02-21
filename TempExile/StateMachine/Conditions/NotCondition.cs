using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class NotCondition : Condition
    {
        Condition cond;

        //Will simply take the inverse of the condition given to it.
        public NotCondition(Condition c)
        {
            cond = c;
        }

        public override bool test(Spectre spectre, Player player)
        {
            return !cond.test(spectre, player);
        }
    }
}
