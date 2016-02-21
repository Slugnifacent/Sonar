﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    class NeutralizedCondition : Condition
    {
        // determines whether spectre has been neutralized or not
        public override bool test(Spectre spectre, Player player)
        {
            return player.GetNoise() == Player.loudness.neutralizing;
        }
    }
}
