using System;
using UnityEngine;
using Zenject;
using Automachine.Scripts.Models;
using System.Collections;
using Automachine.Scripts.Signals;

namespace Automachine.Scripts.Components
{
    public abstract class AutomachineEntity<TState> : MonoBehaviour, IInitializable where TState : Enum
    {
        [Inject] protected readonly AutomachineCore<TState> stateMachine;
        [Inject] protected readonly SignalBus signalBus;

        /// <summary>
        /// Launches new coroutine
        /// </summary>
        /// <param name="callbackOnFinish">Callback, invoked when coroutine ends</param>
        /// <param name="delaySeconds">A delay of coroutine</param>
        public void StartNewCoroutine(Action callbackOnFinish, float delaySeconds) => StartCoroutine(WaitForAndLaunch(callbackOnFinish, delaySeconds));

        private IEnumerator WaitForAndLaunch(Action callbackOnFinish, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            callbackOnFinish?.Invoke();
        }

        /// <summary>
        /// Raised when state machine was successfully initialized and marked as ready.
        /// </summary>
        /// <param name="OnStateMachineInitialized"></param>
        public virtual void OnStateMachineInitialized(OnStateMachineInitialized<TState> OnStateMachineInitialized)
        {
            //REVIEW: Czemu nazwa argumentu wielką literą
        }

        public virtual void Initialize()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {
            stateMachine.Dispose();
        }
        //REVIEW: w kilku plikach są takie endline'y z dupy na końcu klasy.
        
    }
}
