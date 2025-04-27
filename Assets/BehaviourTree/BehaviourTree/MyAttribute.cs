using System;

namespace BehaviourTree
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OpenViewAttribute : Attribute
    {
        public string ButtonName = "打开视图";
    }
}