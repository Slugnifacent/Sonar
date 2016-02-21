using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class SleepState : State
    {
        // Play sleeping noise
        public override void doAction(Spectre spectre, Player player)
        {
            //spectre.playCue();
        }

        public override void doEntryAction(Spectre spectre, Player player)
        {
            Metrics.getInstance().addMetric("Siren Sleep", Player.getInstance().gethealth(), null, spectre.position);
            spectre.playCue();
            return;
        }

        public override void doExitAction(Sonar.Spectre spectre, Sonar.Player player)
        {
            return;
        }
    }
}
