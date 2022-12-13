using Automachine.Scripts.Enums;
using System;

namespace Automachine.Scripts.Models
{
    public class StateTransition<TState> where TState : Enum
    {
        private TState fromState;
        private TState toState;
        private Func<bool> condition = null;
        private float transitionDelay = 0f;
        public TState FromState => fromState;
        public TState ToState => toState;
        public Func<bool> Condition => condition;
        public float TransitionDelay => transitionDelay;

        /// <summary>
        /// Constructor for creating state transitions between given states
        /// </summary>
        /// <param name="from">First state of transition</param>
        /// <param name="to">Second state of transition/param>
        /// <param name="condition">Condition to be met to perform transition</param>
        /// <param name="delay">Delay of switching transition</param>
        public StateTransition(TState from, TState to, Func<bool> condition, float delay = 0f)
        {
            this.fromState = from;
            this.toState = to;
            this.condition = condition;
            this.transitionDelay = delay;
        }
        /// <summary>
        /// Constructor for creating any state transitions
        /// </summary>
        /// <param name="desiredState">Desired state of transition</param>
        /// <param name="condition">Condition to be met to perform transition</param>
        public StateTransition(TState desiredState, Func<bool> condition)
        {
            this.toState = desiredState;
            this.condition = condition;
        }

        /// <summary>
        /// Changes condition of transition
        /// </summary>
        /// <param name="newCondition">New condition of transition</param>
        public void ChangeCondition(Func<bool> newCondition)
        {
            this.condition = newCondition;
        }
        
        /// <summary>
        /// Changes delay of transition
        /// </summary>
        /// <param name="newDelay">New delay of transition</param>
        public void ChangeTransitionDelay(float newDelay)
        {
            this.transitionDelay = newDelay;
        }

        public void Rebind(TState newState, StateRebindOption rebindOption)
        {
            if (rebindOption == StateRebindOption.REBIND_DESIRED_STATE)
                this.toState = newState;
            else if (rebindOption == StateRebindOption.REBIND_ORIGINAL_STATE)
                this.fromState = newState;
        }
    }
}
