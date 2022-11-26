using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class CharacterManager : AutomachineEntity<CharacterState>
{
    public int xdd = 0;
    public int fdsfsdf = 0;

    public override void Initialize()
    {
        base.Initialize();
        if (stateMachine.IsReady)
        {

        }
    }


    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Y))
        {
            xdd++;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            fdsfsdf--;
        }
    }


}
