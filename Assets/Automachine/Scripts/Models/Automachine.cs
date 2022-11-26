using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Zenject;

using Automachine.Scripts.Attributes;
using Automachine.Scripts.Components;
using Automachine.Scripts.Models;

namespace Automachine.Scripts.Models
{
    public class Automachine<TState> : IInitializable, ITickable, IDisposable where TState : Enum
    {
        [Inject] private readonly TickableManager tickableManager;

        private readonly MonoBehaviour connectedMono;

        private bool isReady = false;

        public bool IsReady => isReady;
        public MonoBehaviour ConnectedMono => connectedMono;

        public void Initialize()
        {
            isReady = true;
            Debug.Log("initialized automachine");
        }

        public void Tick()
        {
            if (isReady)
            {
            }
        }

        public void Dispose()
        {
        }
    }
}

