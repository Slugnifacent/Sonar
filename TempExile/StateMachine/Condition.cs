using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    // Abstract class to deal with test cases for state changes
    public abstract class Condition
    {
        /// <summary>
        /// DJ and Corey
        /// This is the abstract class for any conditions used in the state machine.
        /// Conditions are in transitions and are essentially the check to see if that transition is true.
        /// These are mainly boolean tests that see if that transition should trigger.
        /// </summary>
        /// <param name="spectre">Used to calculate checks for the spectres.</param>
        /// <param name="player">Used to calculate checks for the player's location and sound.</param>
        /// <returns></returns>
        public abstract bool test(Spectre spectre, Player player);
    }
}
