using Automachine.Scripts.Components;
using Automachine.Scripts.Models;
using Automachine.Scripts.Attributes;
using System;

namespace Automachine.Scripts.Signals
{
    /// <summary>
    /// Signal raised after automachine core was initialized successfully.
    /// Bound directly to a corresponding method on Automachine Entity
    /// </summary>
    [AutomachineSignal]
    public class OnStateMachineInitialized<TState> where TState : Enum
    {
        public AutomachineCore<TState> connectedStateMachine;
    }

    /// <summary>
    /// Signal raised after each state change.
    /// </summary>
    [AutomachineSignal]
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
    [AutomachineSignal]
    public class OnStateExit<TState> where TState : Enum
    {
        public TState signalStateDisposed;
        public AutomachineCore<TState> connectedStateMachine;
        public AutomachineEntity<TState> connectedEntity;
    }

    /// <summary>
    /// Signal raised on each state enter.
    /// </summary>
    [AutomachineSignal]
    public class OnStateEnter<TState> where TState : Enum
    {
        public TState signalStateStarted;
        public AutomachineCore<TState> connectedStateMachine;
        public AutomachineEntity<TState> connectedEntity;
    }
}