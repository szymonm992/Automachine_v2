using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Automachine.Scripts.Attributes;
using Automachine.Scripts;
using Automachine.Scripts.Components;
using Automachine.Scripts.Models;

public class AutomachineInstaller : MonoInstaller
{
    private readonly List<Type> selectedEnumTypes = new List<Type>();

    public override void InstallBindings()
    {
        InstallStateMachine();
    }

    private void InstallStateMachine()
    {
        SearchForEnumWithAttribute<AutomachineStatesAttribute>();

        if (selectedEnumTypes.Count > 0)
        {
            foreach (Type type in selectedEnumTypes)
            {
                var methodInfoGameStates = typeof(AutomachineInstaller).GetMethod("InstallStates");//needs a public void

                var gameStatesLauncher = methodInfoGameStates.MakeGenericMethod(type);

                object[] args = { type };
                gameStatesLauncher.Invoke(this, args);
            }
        }
    }

    public void InstallStates<TState>(Type currentType) where TState : Enum
    {
        
    }

    private void SearchForEnumWithAttribute<T>() where T : Attribute
    {
        foreach (Type enumType in Assembly.GetExecutingAssembly().GetTypes()
                  .Where(x => x.IsSubclassOf(typeof(Enum)) &&
                  x.GetCustomAttribute<T>() != null))
        {
            selectedEnumTypes.Add(enumType);
        }
    }

    private T ConvertObjectTo<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }
}
