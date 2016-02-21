using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sonar
{
    /// <summary>
    /// DJ and Corey
    /// Abstract class for transitions. These are added to states and have conditions that are checked to see if it should
    /// go to the State that it is given.
    /// </summary>
    public abstract class Transition
    {
        protected State targetState;
        protected Condition condition;

        public Transition(State s)
        {
            targetState = s;
        }

        public abstract void doAction(Spectre spectre, Player player);

        public State getTargetState()
        {
            return targetState;
        }

        // Determine if this transition has all of its conditions met
        public virtual bool isTriggered(Spectre spectre, Player player)
        {
            return condition.test(spectre, player);
        }
    }
}