using Automachine.Scripts.Models;
using System;
using UnityEngine;
using Zenject;


namespace Automachine.Scripts.Components
{
    public abstract class State<TState> : IInitializable, ITickable, IFixedTickable, ILateTickable, IDisposable where TState : Enum
    {
        [Inject] private readonly Automachine<TState> connectedMachine;

        protected readonly TState connectedState;

        protected bool isActive;
        public bool IsActive => isActive;
        public TState ConnectedState => connectedState;

        /// <summary>
        /// Passes params connected to the entity that handles states
        /// </summary>
        /// <param name="inputMachine">State machine as param</param>
        public void UpdateMachine()
        {
        }

        public State(TState state)
        {
            this.connectedState = state;
        }

        public virtual void Initialize()
        {
            // optionally overridden
        }

        /// <summary>
        /// Same as void Update in MonoBehvaiour
        /// </summary>
        public virtual void Tick()
        {
            if (!this.IsActive) return;
            // optionally overridden
        }

        /// <summary>
        /// Same as void FixedUpdate in MonoBehvaiour
        /// </summary>
        public virtual void FixedTick()
        {
            if (!this.IsActive) return;
            // optionally overridden
        }

        /// <summary>
        /// Same as void LateUpdate in MonoBehvaiour
        /// </summary>
        public virtual void LateTick()
        {
            if (!this.IsActive) return;
            // optionally overridden
        }

        /// <summary>
        /// Whenever we override we should remember about calling base type or activate manually
        /// </summary>
        public virtual void Dispose()
        {
            this.isActive = false;
        }

        /// <summary>
        /// Whenever we override we should remember about calling base type or activate manually
        /// </summary>
        public virtual void Start()
        {
            this.isActive = true;
        }

    }
}