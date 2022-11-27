using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class GameStateManager : AutomachineEntity<GameState>
{

    public override void Initialize()
    {
        base.Initialize();
        if (stateMachine.IsReady)
        {
            if(!stateMachine.HasTransition(GameState.Lobby, GameState.InGame))
            {
                stateMachine.AddAnyStateTransition(GameState.InGame, () => stateMachine != null);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }


}

