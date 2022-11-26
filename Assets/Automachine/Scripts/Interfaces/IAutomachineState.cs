
using Automachine.Scripts.Models;
using System;
using Zenject;

namespace Automachine.Scripts.Interfaces
{
    public interface IAutomachineState<TState> where TState : Enum
    {
        public bool IsActive { get; }

        public void Start();

    }
}
