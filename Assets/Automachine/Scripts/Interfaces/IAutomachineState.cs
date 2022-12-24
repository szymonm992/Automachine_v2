
using Automachine.Scripts.Models;
using System;
using Zenject;

namespace Automachine.Scripts.Interfaces
{
    public interface IAutomachineState<TState> : IInitializable, ITickable, IFixedTickable, ILateTickable, IDisposable where TState : Enum
    {
        public bool IsActive { get; }
        public TState ConnectedState { get; }
        
        //REVIEW: stan nie powinien mieć referencji na całą maszynę stanów, bo sterowanie maszyną stanów 
        //nie jest odpowiedzialnością pojedynczych stanów. Pojedynczy stan powinien mieć co najwyżej dostęp do
        //jakiegoś wąskiego interfejsu, ale na pewno nie do całej maszyny.
        //A najlepiej tak, jak wspominałem na discordzie: generyczny public TDataType StateData {get;}
        //gdzie TDataType byłby generykiem.
        //Wtedy Twój State dziedziczący z MonoBehavior mógłby implementować IAutomachineState<TState, AutomachineEntity<TState>>
        //i uniknąłbyś masy niepotrzebnej pośredniości (State->AutomachineCore->AutomachineEntity)
        public AutomachineCore<TState> StateMachine { get; }

        public void StartState();

    }
}
