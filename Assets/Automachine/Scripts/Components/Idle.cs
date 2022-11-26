using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Idle : State<CharacterState>
{

    public Idle(CharacterState state) : base(state)
    {

    }

    public override void Start()
    {
        base.Start();
        Debug.Log("idle started");
    }

    public override void Tick()
    {
        base.Tick();
    }

}

