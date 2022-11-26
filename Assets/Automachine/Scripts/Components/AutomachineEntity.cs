using System;
using UnityEngine;
using Zenject;
using Automachine.Scripts.Models;

namespace Automachine.Scripts.Components
{
    public abstract class AutomachineEntity<TState> : MonoBehaviour, IInitializable where TState : Enum
    {
        
        [Inject] protected readonly Automachine<TState> automachine;

        public virtual void Initialize()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}
