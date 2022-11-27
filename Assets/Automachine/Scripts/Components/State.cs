using Automachine.Scripts.Interfaces;
using Automachine.Scripts.Models;
using Automachine.Scripts.Signals;
using System;
using UnityEngine;
using Zenject;

namespace Automachine.Scripts.Components
{
    public abstract class State<TState> : MonoBehaviour, IAutomachineState<TState> where TState : Enum
    {
        [Inject] protected readonly AutomachineCore<TState> stateMachine;
        [Inject] protected readonly TState connectedState;
        [Inject] protected readonly SignalBus signalBus;

        protected bool isActive;

        public bool IsActive => isActive;
        public TState ConnectedState => connectedState;
        public AutomachineCore<TState> StateMachine => stateMachine;

        /// <summary>
        /// Called on state initialized
        /// </summary>
        public virtual void Initialize()
        {
            isActive = false;
        }

        /// <summary>
        /// Same as void Update in MonoBehvaiour
        /// </summary>
        public virtual void Tick()
        {
            if (!this.IsActive)
            {
                return;
            }
        }

        /// <summary>
        /// Same as void FixedUpdate in MonoBehvaiour
        /// </summary>
        public virtual void FixedTick()
        {
            if (!this.IsActive)
            {
                return;
            }
        }

        /// <summary>
        /// Same as void LateUpdate in MonoBehvaiour
        /// </summary>
        public virtual void LateTick()
        {
            if (!this.IsActive)
            {
                return;
            }
        }

        /// <summary>
        /// Whenever we override we should remember about calling base type or activate manually
        /// </summary>
        public virtual void Dispose()
        {
            this.isActive = false;
            signalBus.Fire(new OnStateExit<TState>()
            {
                signalStateDisposed = this.ConnectedState,
                connectedStateMachine = this.StateMachine,
                connectedEntity = this.StateMachine.ConnectedEntity
            });
        }

        /// <summary>
        /// Whenever we override we should remember about calling base type or activate manually
        /// </summary>
        public virtual void StartState()
        {
            this.isActive = true;
            signalBus.Fire(new OnStateEnter<TState>()
            {
                signalStateStarted = this.ConnectedState,
                connectedStateMachine = this.StateMachine,
                connectedEntity = this.StateMachine.ConnectedEntity
            });
        }

    }
}