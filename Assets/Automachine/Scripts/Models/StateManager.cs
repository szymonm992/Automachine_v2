using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Automachine.Scripts.Models;
using Automachine.Scripts.Interfaces;
using System;
using System.Linq;

namespace Automachine.Scripts.Components
{
    public class StateManager<TState> : IInitializable, IDisposable where TState : Enum
    {
        [Inject] private readonly AutomachineCore<TState> stateMachine;

        public void Initialize()
        {

        }
        public void Dispose()
        {
        }
    }
}
