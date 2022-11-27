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
    public class AutomachineCore<TState> : IInitializable, ITickable, IDisposable where TState : Enum
    {
        [Inject] private readonly IAutomachineState<TState>[] allStates;
        [Inject] private readonly AutomachineEntity<TState> connectedEntity;
        [Inject] private readonly AutomachineDebugSettings debugSettings;
        [Inject] private readonly StateManager<TState> stateManager;
        [Inject(Id = "AutomachineDefaultState")] private readonly TState defaultState;

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
        }

        public void Tick()
        {
            if (isReady)
            {

            }
        }

        public void Dispose()
        {
            stateManager.Dispose();
        }
    }
}

