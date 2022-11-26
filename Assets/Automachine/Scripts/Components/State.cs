using Automachine.Scripts.Interfaces;
using Automachine.Scripts.Models;
using System;
using UnityEngine;
using Zenject;


namespace Automachine.Scripts.Components
{
    public abstract class State<TState> : MonoBehaviour, IAutomachineState<TState> where TState : Enum
    {
        [Inject] protected readonly Automachine<TState> stateMachine;

        protected bool isActive;
        public bool IsActive => isActive;

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