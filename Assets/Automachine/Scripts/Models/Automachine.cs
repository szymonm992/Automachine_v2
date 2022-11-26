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

namespace Automachine.Scripts.Models
{
    public class Automachine<TState> : IInitializable, ITickable, IDisposable where TState : Enum
    {
        [Inject] private readonly IAutomachineState<TState>[] allStates;
        [Inject] private AutomachineEntity<TState> connectedEntity;


        private bool isReady = false;

        public bool IsReady => isReady;
        public AutomachineEntity<TState> ConnectedEntity => connectedEntity;
        public void Initialize()
        {
            isReady = true;
            Debug.Log("initialized automachine with "+allStates.Count() + " states" + connectedEntity.gameObject.name);
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

