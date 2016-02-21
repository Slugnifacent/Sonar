using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    public class ExorcisedCondition : Condition
    {
        // Determine if player is making sound loud enough to excorsize a Spectre
        public override bool test(Spectre spectre, Player player)
        {
            if (player.GetNoise() == Player.loudness.exorcising)
            {
                spectre.playExcorcismCue();
                return true;
            }
            return false;
        }
    }
}
