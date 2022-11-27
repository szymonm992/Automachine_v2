using Automachine.Scripts.Attributes;

[AutomachineStates]
public enum GameState
{
    [StateEntity(typeof(Lobby))]
    Lobby,
    [StateEntity(typeof(InGame))]
    InGame,
}

