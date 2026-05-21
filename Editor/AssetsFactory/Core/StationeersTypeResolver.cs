using System;
using System.Reflection;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class StationeersTypeResolver
    {
        public static Type ResolveComponentType(
            string namespaceName,
            string className,
            string preferredAssemblyName)
        {
            string fullName = string.IsNullOrEmpty(namespaceName)
                ? className
                : namespaceName + "." + className;

            if (!string.IsNullOrEmpty(preferredAssemblyName))
            {
                Type type = Type.GetType(fullName + ", " + preferredAssemblyName);

                if (IsValidComponentType(type))
                {
                    return type;
                }
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);

                if (IsValidComponentType(type))
                {
                    return type;
                }
            }

            return null;
        }

        public static bool IsValidComponentType(Type type)
        {
            return type != null
                   && !type.IsAbstract
                   && typeof(Component).IsAssignableFrom(type);
        }
    }
}