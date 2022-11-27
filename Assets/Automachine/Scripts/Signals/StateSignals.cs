using Automachine.Scripts.Components;
using Automachine.Scripts.Models;
using System;

namespace Automachine.Scripts.Signals
{
    /// <summary>
    /// Signal raised after each state change.
    /// </summary>
    public class OnStateChangedSignal<TState> where TState : Enum
    {
        public TState signalPreviousState;
        public TState signalNextState;
        public AutomachineCore<TState> connectedStateMachine;
        public AutomachineEntity<TState> connectedEntity;
    }

    /// <summary>
    /// Signal raised on each state exit.
    /// </summary>
    public class OnStateExit<TState> where TState : Enum
    {
        public TState signalStateDisposed;
        public AutomachineCore<TState> connectedStateMachine;
        public AutomachineEntity<TState> connectedEntity;
    }

    /// <summary>
    /// Signal raised on each state enter.
    /// </summary>
    public class OnStateEnter<TState> where TState : Enum
    {
        public TState signalStateStarted;
        public AutomachineCore<TState> connectedStateMachine;
        public AutomachineEntity<TState> connectedEntity;
    }

}