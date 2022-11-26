using System;

namespace Automachine.Scripts.Attributes
{
    public class StateEntityAttribute : Attribute
    {
        public Type BaseClassType { get; private set; }
        public StateEntityAttribute(Type type)
        {
            BaseClassType = type;
        }
    }
}
