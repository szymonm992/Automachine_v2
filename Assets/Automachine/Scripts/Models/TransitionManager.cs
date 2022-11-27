using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;
using System;
using System.Linq;
using Automachine.Scripts.Enums;

namespace Automachine.Scripts.Models
{
    public class TransitionsManager<TState> : IInitializable, IDisposable where TState : Enum
    {
        [Inject] private readonly StateManager<TState> stateManager;
        [Inject] private readonly Type selectedEnumType;

        private readonly Dictionary<string, StateTransition<TState>> allConditionalTransitions = new Dictionary<string, StateTransition<TState>>();
        private readonly Dictionary<string, StateTransition<TState>> allAnyStateTransitions = new Dictionary<string, StateTransition<TState>>();

        private Dictionary<string, int> selectedEnumProperties = new Dictionary<string, int>();

        public Dictionary<string, StateTransition<TState>> AllConditionalTransitions => allConditionalTransitions;
        public Dictionary<string, StateTransition<TState>> AllAnyStateTransitions => allAnyStateTransitions;

        public void Initialize()
        {
            Debug.Log("init");
            selectedEnumProperties = GetEnumValuesAndNames(selectedEnumType);
        }

        internal void UpdateTransitions()
        {
            if (stateManager.IsChangingState)
            {
                return;
            }

            UpdateAnyStateTransitions();
            UpdateConditionalTransitions();
        }

        private void UpdateAnyStateTransitions()
        {
            if (allConditionalTransitions.Count > 0)
            {
                foreach (var kvp in allConditionalTransitions)
                {
                    if (kvp.Value.FromState.Equals(stateManager.CurrentState) && kvp.Value.Condition?.Invoke() == true)
                    {
                        stateManager.ChangeState(kvp.Value.ToState);
                        return;
                    }
                }
            }
        }

        private void UpdateConditionalTransitions()
        {
            if (allConditionalTransitions.Count > 0)
            {
                foreach (var kvp in allConditionalTransitions)
                {
                    if (kvp.Value.FromState.Equals(stateManager.CurrentState) && kvp.Value.Condition?.Invoke() == true)
                    {
                        stateManager.ChangeState(kvp.Value.ToState);
                        return;
                    }
                }
            }
        }

        #region HANDLING TRANSITIONS
        internal bool AddTransition(string transitionId, StateTransition<TState> transitionData)
        {
            if (!HasTransition(transitionId, transitionData.Condition))
            {
                allConditionalTransitions.Add(transitionId, transitionData);
                return true;
            }
            return false;
        }

        internal bool AddAnyStateTransition(TState desiredState, StateTransition<TState> transitionData)
        {
            if (!HasAnyStateTransition(desiredState, transitionData.Condition))
            {
                allAnyStateTransitions.Add(desiredState.ToString(), transitionData);
                return true;
            }
            return true;
        }

        internal bool HasTransition(string transitionId)
        {
            return allConditionalTransitions.ContainsKey(transitionId);
        }

        internal bool HasTransition(string transitionId, Func<bool> condition)
        {
            if (allConditionalTransitions.ContainsKey(transitionId))
            {
                return CheckConditionsEqual(allConditionalTransitions[transitionId].Condition, condition);
            }
            return false;
        }

        internal bool HasAnyStateTransition(TState targetState)
        {
            return allAnyStateTransitions.ContainsKey(targetState.ToString());
        }

        internal bool HasAnyStateTransition(TState targetState, Func<bool> condition)
        {
            if (allAnyStateTransitions.ContainsKey(targetState.ToString()))
            {
                return CheckConditionsEqual(allAnyStateTransitions[targetState.ToString()].Condition, condition);
            }
            return false;
        }

        internal bool RemoveTransition(string transitionId)
        {
            if (HasTransition(transitionId))
            {
                allConditionalTransitions.Remove(transitionId);
                return true;
            }
            return false;
        }

        internal bool RebindTransition(string transitionId, TState newState, StateRebindOption rebindOption)
        {
            if (HasTransition(transitionId) && !newState.Equals(allConditionalTransitions[transitionId].FromState)
                && !newState.Equals(allConditionalTransitions[transitionId].ToState))
            {
                allConditionalTransitions[transitionId].Rebind(newState, rebindOption);
                return true;
            }
            return false;
        }

        internal bool ChangeTransitionCondition(string transitionId, Func<bool> newCondition)
        {
            if (HasTransition(transitionId))
            {
                allConditionalTransitions[transitionId].ChangeCondition(newCondition);
                return true;
            }
            return false;
        }

        internal string CreateTransitionId(TState from, TState to)
        {
            string result = selectedEnumProperties[from.ToString()] + "|" + selectedEnumProperties[to.ToString()];
            return result;
        }
        #endregion  

        public void Dispose()
        {

        }

        private Dictionary<string, int> GetEnumValuesAndNames(Type enumType)
        {
            if (!typeof(Enum).IsAssignableFrom(enumType))
            {
                throw new ArgumentException("Given type should describe enum and is assigning from " + enumType.Name);
            }

            var names = Enum.GetNames(enumType).Cast<object>();
            var values = Enum.GetValues(enumType).Cast<int>();

            return names.Zip(values, (name, value) => new { name, value })
                .ToDictionary(x => x.name.ToString(), y => y.value);
        }

        private bool CheckConditionsEqual(Delegate a, Delegate b)
        {
            while (a.Target is Delegate)
            {
                a = (a.Target as Delegate);
            }

            while (b.Target is Delegate)
            {
                b = (b.Target as Delegate);
            }

            if (a == b)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Target != b.Target)
            {
                return false;
            }

            byte[] a_body = a.Method.GetMethodBody().GetILAsByteArray();
            byte[] b_body = b.Method.GetMethodBody().GetILAsByteArray();

            if (a_body.Length != b_body.Length)
            {
                return false;
            }

            for (int i = 0; i < a_body.Length; i++)
            {
                if (a_body[i] != b_body[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
