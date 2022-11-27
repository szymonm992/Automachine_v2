using Automachine.Scripts.Attributes;


[AutomachineStates]
public enum CharacterState
{
    [DefaultState, StateEntity(typeof(Idle))]
    Idle,
    [StateEntity(typeof(Walking))]
    Walking,
    [StateEntity(typeof(Dead))]
    Dead,

}

