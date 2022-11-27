using System;
using UnityEngine;
using Zenject;
using Automachine.Scripts.Models;
using System.Collections;

namespace Automachine.Scripts.Components
{
    public abstract class AutomachineEntity<TState> : MonoBehaviour, IInitializable where TState : Enum
    {
        [Inject] protected readonly AutomachineCore<TState> stateMachine;

        /// <summary>
        /// Launches new coroutine
        /// </summary>
        /// <param name="callbackOnFinish">Callback, invoked whjen coroutine ends</param>
        /// <param name="delaySeconds">A delay of coroutine</param>
        public void StartNewCoroutine(Action callbackOnFinish, float delaySeconds) => StartCoroutine(WaitForAndLaunch(callbackOnFinish, delaySeconds));

        private IEnumerator WaitForAndLaunch(Action callbackOnFinish, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            callbackOnFinish?.Invoke();
        }

        /// <summary>
        /// Called when zenject successfully initialized entity
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Called on each frame
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Called on destroy of entity
        /// </summary>
        protected virtual void OnDestroy()
        {
            stateMachine.Dispose();
        }

        
    }
}
