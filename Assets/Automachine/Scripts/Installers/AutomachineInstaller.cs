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
        if (currentType == null)
        {
            AutomachineLogger.LogError("Provided type is null! Canceling state machine creation");
            return;
        }
        else
        {
            AutomachineLogger.Log("Found enum of type <color=white>" + currentType.Name + "</color> that matches Automachine criteria. Creating a state machine...");
            //Container.BindInstance(currentType).WhenInjectedInto(typeof(TransitionsManager<TState>));
        }


        Array fields = currentType.GetEnumValues();

        foreach (var currentField in fields)
        {
            TState state = ConvertObjectTo<TState>(currentField);
            FieldInfo field = currentType.GetField(state.ToString());

            if (field.IsDefined(typeof(StateEntityAttribute), false))
            {
               AutomachineLogger.Log("Creating state: <color=white>" + state + "</color>");

                if (field.IsDefined(typeof(DefaultStateAttribute), false))
                {
                    Container.BindInstance(state).WithId("AutomachineDefaultState").WhenInjectedInto(typeof(Automachine<TState>));
                   AutomachineLogger.Log("Binding default state <color=white>" + state + "</color>");
                }
                Type baseClassType = field.GetCustomAttribute<StateEntityAttribute>().BaseClassType;
            }

        }

        Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentsInHierarchy().AsCached();
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
