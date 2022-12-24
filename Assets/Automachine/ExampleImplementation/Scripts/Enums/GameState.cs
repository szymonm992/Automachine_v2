//REVIEW: nie podoba mi się, że istnieje namespace Scripts, który niczego nie wnosi.
//Powinno być po prostu Automachine.Attributes
using Automachine.Scripts.Attributes;

[AutomachineStates]
public enum GameState
{
    [StateEntity(typeof(Lobby))]
    Lobby,
    [StateEntity(typeof(InGame))]
    InGame,
}

