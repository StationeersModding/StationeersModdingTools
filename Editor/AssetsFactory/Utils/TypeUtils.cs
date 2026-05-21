using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    /// <summary>
    /// Check if a namespace.type class exists
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Finds existing component in any namespace
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool ComponentExists(
            string namespaceName,
            string className)
        {
            string fullName = $"{namespaceName}.{className}";

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Any(t =>
                    t != null &&
                    t.FullName == fullName &&
                    typeof(UnityEngine.Component).IsAssignableFrom(t));
        }
        
        /// <summary>
        /// Finds existing component in the specific namespace
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool NamespaceComponentExists(string namespaceName, string className)
        {
            string fullTypeName = string.IsNullOrEmpty(namespaceName)
                ? className
                : $"{namespaceName}.{className}";

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypesSafe)
                .Any(type =>
                    type != null &&
                    type.FullName == fullTypeName &&
                    typeof(UnityEngine.Component).IsAssignableFrom(type));
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(type => type != null);
            }
        }
    }
}
