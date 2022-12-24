using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;
using Automachine.Scripts.Signals;

public class GameStateManager : AutomachineEntity<GameState>
{

    public override void OnStateMachineInitialized(OnStateMachineInitialized<GameState> OnStateMachineInitialized)
    {
        base.OnStateMachineInitialized(OnStateMachineInitialized);
        if (!stateMachine.HasTransition(GameState.Lobby, GameState.InGame))
        {
            stateMachine.AddAnyStateTransition(GameState.InGame, () => stateMachine != null);
        }
    }

    //REVIEW: niepotrzebny Update. Jeśli ma coś zobrazować, to potrzebny jest przynajmniej jakiś komentarz
    protected override void Update()
    {
        base.Update();
    }


}

