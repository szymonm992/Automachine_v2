using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Zenject;

using Automachine.Scripts.Attributes;
using Automachine.Scripts.Components;
using Automachine.Scripts.Models;
using Automachine.Scripts.Interfaces;
using Automachine.Scripts.Enums;
using Automachine.Scripts.Signals;

namespace Automachine.Scripts.Models
{
    public class AutomachineCore<TState> : IInitializable, ITickable, IDisposable where TState : Enum
    {
        [Inject] private readonly SignalBus signalBus;
        [Inject] private readonly IAutomachineState<TState>[] allStates;
        [Inject] private readonly AutomachineEntity<TState> connectedEntity;
        [Inject] private readonly AutomachineDebugSettings debugSettings;
        [Inject] private readonly StateManager<TState> stateManager;
        [Inject] private readonly TransitionsManager<TState> transitionsManager;

        private bool isReady = false;

        public bool IsReady => isReady;
        public AutomachineEntity<TState> ConnectedEntity => connectedEntity;

        public void Initialize()
        {
            if (debugSettings.logConnectedMonoType)
            {
                AutomachineLogger.Log("Connected MonoBehaviour script is of type <color=white>" + (connectedEntity ? connectedEntity.GetType() : "null") + "</color>");
            }

            isReady = true;
            signalBus.Fire(new OnStateMachineInitialized<TState>()
            {
                connectedStateMachine = this,
            });
        }

        #region TRANSITIONS HANDLING

        /// <summary>
        /// Changes current state, skips transitions.
        /// </summary>
        /// <param name="desiredState">Desired new state</param>
        /// <param name="delay">Delay of execution</param>
        public void ChangeState(TState desiredState, float delay = 0f)
        {
            transitionsManager.ChangeState(desiredState, delay);
        }

        /// <summary>
        /// Creates transition from given to desired state
        /// </summary>
        /// <param name="fromState">First state of transition</param>
        /// <param name="toState">Second state of transition</param>
        /// <param name="condition">Condition to be met to perform transition</param>
        /// <param name="OnTransitionComplete">Action to be executed on transition complete</param>
        /// <param name="delay">If not 0 then transition manager will wait for given amount of seconds</param>
        public void AddTransition(TState fromState, TState toState, Func<bool> condition, float delay = 0f, Action OnTransitionComplete = null)
        {
            if (fromState.Equals(toState))
            {
                AutomachineLogger.LogError("Start and final states are equal!");
                return;
            }
            string id = transitionsManager.CreateTransitionId(fromState, toState);
            StateTransition<TState> stateTransition = new StateTransition<TState>(fromState, toState, condition, delay);
            AddTransition(id, stateTransition, OnTransitionComplete);
        }

        /// <summary>
        /// Checks whether a transition between given state exists
        /// </summary>
        /// <param name="fromState">First statek</param>
        /// <param name="toState">Second state</param>
        /// <returns></returns>
        public bool HasTransition(TState fromState, TState toState)
        {
            string transitionId = transitionsManager.CreateTransitionId(fromState, toState);
            return transitionsManager.HasTransition(transitionId);
        }

        /// <summary>
        /// Checks whether a transition between given state with given condition exists in TransitionsManager
        /// </summary>
        /// <param name="fromState">First statek</param>
        /// <param name="toState">Second state</param>
        /// <param name="condition">Condition to be checked</param>
        /// <returns></returns>
        public bool HasTransition(TState fromState, TState toState, Func<bool> condition)
        {
            string transitionId = transitionsManager.CreateTransitionId(fromState, toState);
            return transitionsManager.HasTransition(transitionId, condition);
        }

        /// <summary>
        /// Checks whether anystate transition to target state exists in TransitionsManager
        /// </summary>
        /// <param name="targetState">Target state to check</param>
        /// <returns></returns>
        public bool HasAnyStateTransition(TState targetState)
        {
            return transitionsManager.HasAnyStateTransition(targetState);
        }

        /// <summary>
        /// Checks whether anystate transition to target state with given condition exists in TransitionsManager
        /// </summary>
        /// <param name="targetState">Target state to be checked</param>
        /// <param name="condition">Condition to be checked</param>
        /// <returns></returns>
        public bool HasAnyStateTransition(TState targetState, Func<bool> condition)
        {
            return transitionsManager.HasAnyStateTransition(targetState, condition);
        }

        /// <summary>
        /// Creates transition from any of available states towards desired state
        /// </summary>
        /// <param name="desiredState">Desired state of transition</param>
        /// <param name="condition">Condition to be met to perform transition</param>
        /// <param name="OnTransitionComplete">Action to be executed on transition complete</param>
        public void AddAnyStateTransition(TState desiredState, Func<bool> condition, Action OnTransitionComplete = null)
        {
            StateTransition<TState> anyStateTransition = new StateTransition<TState>(desiredState, condition);
            if (!transitionsManager.AddAnyStateTransition(desiredState, anyStateTransition))
            {
                Debug.Log("Couldn't create anystate transition!");
            }
            else
            {
                OnTransitionComplete?.Invoke();
            }
        }

        /// <summary>
        /// Removes transition between states
        /// </summary>
        /// <param name="fromState">First state of transition</param>
        /// <param name="toState">Second state of transition</param>
        /// <param name="OnTransitionComplete">Action to be executed when transition has been removed</param>
        public void RemoveTransition(TState fromState, TState toState, Action OnTransitionComplete = null)
        {
            string id = transitionsManager.CreateTransitionId(fromState, toState);
            RemoveTransition(id, OnTransitionComplete);
        }

        /// <summary>
        /// Rebinds current transition's desired state
        /// </summary>
        /// <param name="fromState">First state of transition</param>
        /// <param name="toState">Original state of transition</param>
        /// <param name="newState">New desired state of transition</param>
        /// <param name="rebindOption">Defines if function should rebind beginning or final state</param>
        /// <param name="OnTransitionComplete">Action to be executed when transition has been rebound</param>
        public void RebindTransitionStates(TState fromState, TState toState, TState newState, StateRebindOption rebindOption, Action OnTransitionComplete = null)
        {
            if (fromState.Equals(newState))
            {
                AutomachineLogger.LogError("Start and final states are equal!");
                return;
            }
            string id = transitionsManager.CreateTransitionId(fromState, toState);
            RebindTransitionStates(id, newState, rebindOption, OnTransitionComplete);
        }

        /// <summary>
        /// Changes current condition of transition
        /// </summary>
        /// <param name="fromState">First state of transition</param>
        /// <param name="toState">Desired state of transition</param>
        /// <param name="newCondition">New condition of transition</param>
        /// <param name="OnTransitionComplete">Action to be executed when transition's condition has been changed</param>
        public void ChangeTransitionCondition(TState fromState, TState toState, Func<bool> newCondition, Action OnTransitionComplete = null)
        {
            string id = transitionsManager.CreateTransitionId(fromState, toState);
            ChangeTransitionCondition(id, newCondition, OnTransitionComplete);
        }

        /// <summary>
        /// Returns all conditional transitions
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, StateTransition<TState>> GetAllConditionalTransitions()
        {
            return transitionsManager.AllConditionalTransitions;
        }

        /// <summary>
        /// Returns all any state transitions
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, StateTransition<TState>> GetAllAnyStateTransitions()
        {
            return transitionsManager.AllAnyStateTransitions;
        }

        private void RebindTransitionStates(string transitionId, TState newState, StateRebindOption rebindOption, Action OnTransitionComplete = null)
        {
            if (!transitionsManager.RebindTransition(transitionId, newState, rebindOption))
            {
                Debug.Log("Couldn't rebind transition from/to given states!");
            }
            else
            {
                OnTransitionComplete?.Invoke();
            }
        }

        private void ChangeTransitionCondition(string transitionId, Func<bool> newCondition, Action OnTransitionComplete = null)
        {
            if (!transitionsManager.ChangeTransitionCondition(transitionId, newCondition))
            {
                Debug.Log("Couldn't change condition of given states!");
            }
            else
            {
                OnTransitionComplete?.Invoke();
            }
        }

        private void RemoveTransition(string transitionId, Action OnTransitionComplete = null)
        {
            if (!transitionsManager.RemoveTransition(transitionId))
            {
                Debug.Log("Couldn't remove transition from given states!");
            }
            else
            {
                OnTransitionComplete?.Invoke();
            }
        }

        private void AddTransition(string transitionId, StateTransition<TState> transitionData, Action OnTransitionComplete = null)
        {
            if (!transitionsManager.AddTransition(transitionId, transitionData))
            {
                Debug.Log("Couldn't create transition from/to given states!");
            }
            else
            {
                OnTransitionComplete?.Invoke();
            }
        }


        #endregion

        public void Tick()
        {
            if (isReady)
            {
                transitionsManager.UpdateTransitions();
            }
        }

        public void Dispose()
        {
            stateManager.Dispose();
        }

        public State<TState> GetState(TState stateValue)
        {
            if (allStates.Any())
            {
                foreach (var state in allStates)
                {
                    if (state.ConnectedState.Equals(stateValue))
                    {
                        return (State<TState>)state;
                    }
                }
            }
            return null;
        }

        public State<TState> GetStateOfType(Type type)
        {
            if(allStates.Any())
            {
                foreach (var state in allStates)
                {
                    if(state.GetType() == type)
                    {
                        return (State<TState>)state;
                    }
                }
            }
            return null;
        }
    }
}

