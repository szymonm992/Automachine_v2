using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Automachine.Scripts.Models;
using Automachine.Scripts.Interfaces;
using System;
using System.Linq;
using Automachine.Scripts.Signals;

namespace Automachine.Scripts.Components
{
    public class StateManager<TState> : IInitializable, IDisposable where TState : Enum
    {
        [Inject] private readonly SignalBus signalBus;
        [Inject] private readonly AutomachineCore<TState> stateMachine;
        [Inject] private readonly IAutomachineState<TState>[] allStates;
        [Inject] private readonly AutomachineDebugSettings debugSettings;
        [Inject(Id = "AutomachineDefaultState", Optional = true)] private TState defaultState;

        private TState currentState;
        private TState previousState;

        private IAutomachineState<TState> currentStateEntity = null;

        public IAutomachineState<TState> CurrentStateEntity => currentStateEntity;
        public TState CurrentState => currentState;
        public TState PreviousState => previousState;
        public TState DefaultState => defaultState;
        public bool IsChangingState { get; private set; }

        public void Initialize()
        {
            if (defaultState == null)
            {
                //setting default state as first element of enum in case we didnt find any with DefaultState attribute
                SetDefaultState(allStates.First().ConnectedState);
                AutomachineLogger.Log("[Automachine missing attribute] " + "There is no state with <color=white>[DefaultState]</color> attribute. Selecting first of the list, which is: " + defaultState);
            }

            if (debugSettings.logCreatedStatesAmount)
            {
                AutomachineLogger.Log("<color=green>Successfully</color> created machine with <b>" + allStates.Length + "</b> states based on enum: <color=white>" + typeof(TState).Name + "</color>");
            }
            ChangeState(defaultState, true);
        }

        public void ChangeStateDelayed(TState nextState, float delayInSeconds)
        {
            if (IsChangingState)
            {
                AutomachineLogger.LogError("Cannot change state while the state is being changed already");
                return;
            }

            IsChangingState = true;
            stateMachine.ConnectedEntity.StartNewCoroutine(() => {
                IsChangingState = false;
                ChangeState(nextState);
            }, delayInSeconds);
        }

        public void ChangeState(TState state, bool firstRun = false)
        {
            if (IsChangingState)
            {
                AutomachineLogger.LogError("Cannot change state while the state is being changed already");
                return;
            }

            if (!firstRun && currentState.Equals(state))
            {
                return;
            }

            IsChangingState = true;
            if (currentStateEntity != null)
            {
                currentStateEntity.Dispose();
                currentStateEntity = null;
            }

            previousState = currentState;
            currentState = state;
            currentStateEntity = GetStateFromList(state);

            if (currentStateEntity == null)
            {
                AutomachineLogger.LogError("Entity of state <color=white>" + state + "</color> was not found!");
            }

            currentStateEntity.StartState();
            IsChangingState = false;

            signalBus.Fire(new OnStateChangedSignal<TState>()
            {
                signalPreviousState = previousState,
                signalNextState = currentState,
                connectedStateMachine = stateMachine,
                connectedEntity = stateMachine.ConnectedEntity
            });

            if (!firstRun)
            {
                if (debugSettings.logSwitchingState)
                {
                    AutomachineLogger.Log("Current state was switched from: <color=white>" + previousState + "</color> to: <color=white>" + currentState + "</color>");
                }
            }
            else
            {
                if (debugSettings.logLaunchingDefaultState)
                {
                    AutomachineLogger.Log("started default state: <color=white>" + defaultState + "</color>");
                }
            }
        }

        /// <summary>
        /// Sets the default state for current Automachine instance
        /// </summary>
        /// <param name="newDefaultState">New default state</param>
        public void SetDefaultState(TState newDefaultState) => defaultState = newDefaultState;

        /// <summary>
        /// Disposes state manager
        /// </summary>
        public void Dispose()
        {
        }

        private IAutomachineState<TState> GetStateFromList(TState inputState)
        {
            foreach (var state in allStates)
            {
                if (state.ConnectedState.Equals(inputState))
                {
                    return state;
                }
            }
            return null;
        }


       
    }
}
