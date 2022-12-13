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
using Automachine.Scripts.Signals;

public class StateInstaller : MonoInstaller
{

    [SerializeField] private AutomachineDebugSettings debugSettings = new AutomachineDebugSettings();

    public override void InstallBindings()
    {
        InstallStateMachine();
    }

    private void InstallStateMachine()
    {
        if(!Container.HasBinding(typeof(SignalBus)))
        {
            SignalBusInstaller.Install(Container);
        }
        
        Container.BindInstance(debugSettings).AsCached().NonLazy();

        var enumsOnGameObject = SearchForEnumWithAttributeOnGameObject<AutomachineStatesAttribute>();

        foreach (Type type in enumsOnGameObject)
        {
            object[] args = { type };
            InvokeGenericMethod("InstallStates", type, args);
            InvokeGenericMethod("CreateAndDeclareSignals", type, null);
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
            if (debugSettings.logFoundMatchingEnums)
            {
                AutomachineLogger.Log("Found entity with enum type of: <color=white>" + currentType.Name + "</color>" +
                    " for entity on: <color=white>" + gameObject.name+ "</color>. Creating a state machine...");
            }
            Container.BindInstance(currentType).WhenInjectedInto<TransitionsManager<TState>>();
        }

        Container.BindInterfacesAndSelfTo<TransitionsManager<TState>>().FromNew().AsCached().NonLazy();

        Array fields = currentType.GetEnumValues();

        foreach (var currentField in fields)
        {
            TState state = ConvertObjectTo<TState>(currentField);
            FieldInfo field = currentType.GetField(state.ToString());

            if (field.IsDefined(typeof(StateEntityAttribute), false))
            {
                if (debugSettings.logCreatingStates)
                {
                    AutomachineLogger.Log("Creating state: <color=white>" + state + "</color>");
                }

                if (field.IsDefined(typeof(DefaultStateAttribute), false))
                {
                    Container.BindInstance(state).WithId("AutomachineDefaultState").WhenInjectedInto(typeof(AutomachineCore<TState>));
                    if (debugSettings.logBindingDefaultStates)
                    {
                        AutomachineLogger.Log("Binding default state: <color=white>" + state + "</color>");
                    }
                }

                Type baseClassType = field.GetCustomAttribute<StateEntityAttribute>().BaseClassType;
                Container.BindInterfacesAndSelfTo(baseClassType).FromNewComponentOnNewGameObject()
                    .WithGameObjectName("Automachine state: "+baseClassType.Name).AsCached().NonLazy();
                Container.BindInstance(state).WhenInjectedInto(baseClassType);
            }
        }
        Container.BindInterfacesAndSelfTo<StateManager<TState>>().FromNew().AsCached();
        Container.BindInterfacesAndSelfTo<AutomachineCore<TState>>().FromNew().AsCached();
        Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentInHierarchy().AsCached();
    }

    public void CreateAndDeclareSignals<TState>() where TState : Enum
    {
        var automachineSignalTypes = SearchForClassWithAttribute<AutomachineSignalAttribute>();
        
        if(automachineSignalTypes.Any())
        {
            foreach (var signal in automachineSignalTypes)
            {
                var signalType = signal.MakeGenericType(typeof(TState));
                Container.DeclareSignal(signalType).OptionalSubscriber();

                if (debugSettings.logFoundSignals)
                {
                    AutomachineLogger.Log("Declaring signals for enum type of: <color=white>" + signalType.Name + "</color> " +
                        "for entity on "+gameObject.name);
                }
            }

            Container.BindSignal<OnStateMachineInitialized<TState>>()
                   .ToMethod<AutomachineEntity<TState>>(entity => entity.OnStateMachineInitialized).FromResolve();
        }
    }

    public void ValidateGameObjectHasEntity<TState>(Type type, List<Type> selectedEnumTypes) where TState : Enum
    {
        if (gameObject.GetComponent<AutomachineEntity<TState>>() != null)
        {
            selectedEnumTypes.Add(type);
        }
    }

    private IEnumerable<Type> SearchForClassWithAttribute<T>() where T : Attribute
    {
        List<Type> selectedSignalTypes = new();
        foreach (Type enumType in Assembly.GetExecutingAssembly().GetTypes()
                  .Where(x => x.IsClass && x.GetCustomAttribute<T>() != null))
        {
            selectedSignalTypes.Add(enumType);
        }
        return selectedSignalTypes;
    }

    private IEnumerable<Type> SearchForEnumWithAttributeOnGameObject<T>() where T : Attribute
    {
        List<Type> selectedEnumTypes = new();
        foreach (Type enumType in Assembly.GetExecutingAssembly().GetTypes()
                  .Where(x => x.IsSubclassOf(typeof(Enum)) &&
                  x.GetCustomAttribute<T>() != null))
        {
            var methodInfoValidation = typeof(StateInstaller).GetMethod("ValidateGameObjectHasEntity");
            var entityValidator = methodInfoValidation.MakeGenericMethod(enumType);
            object[] args = { enumType, selectedEnumTypes };

            entityValidator.Invoke(this, args);
        }
        return selectedEnumTypes;
    }

    private T ConvertObjectTo<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }

    private void InvokeGenericMethod(string name, Type type, object[] args)
    {
        var method = typeof(StateInstaller).GetMethod(name);
        var methodLauncher = method.MakeGenericMethod(type);
        methodLauncher.Invoke(this, args);
    }
}
