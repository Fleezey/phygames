using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Runtime
{
    internal static class UXMLRuntimeFactories
    {
        private static readonly bool k_Registered;

        #if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        internal static void RegisterUserFactories()
        {
            HashSet<string> userAssemblies = new HashSet<string>(InternalBridge.GetAllUserAssemblies());
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!(userAssemblies.Contains(assembly.GetName().Name + ".dll")))
                    continue;
                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (typeof(IUxmlFactory).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        var factory = (IUxmlFactory)Activator.CreateInstance(type);
                        InternalBridge.RegisterFactory(factory);
                    }
                }
            }
        }
        #endif
    }

}
