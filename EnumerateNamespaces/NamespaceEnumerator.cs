namespace EnumerateNamespaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class NamespaceEnumerator
    {
        public static IEnumerable<string> GetNamespaces(string assemblyName)
        {
            try
            {
                string basePath = Path.GetDirectoryName(assemblyName);
                var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyName);
                
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    LoadDependentAssembly(basePath, referencedAssembly);
                }
                
                var types = assembly.DefinedTypes;
                return
                    (from Type type in types select type.Namespace)
                    .Distinct()
                    .Where(ns => !string.IsNullOrWhiteSpace(ns));
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine("Loader exceptions:");
                foreach (var internalException in ex.LoaderExceptions)
                {
                    Console.WriteLine(internalException.ToString());
                }

                throw;
            }
        }

        private static void LoadDependentAssembly(string basePath, AssemblyName assemblyName)
        {
            try
            {
                Assembly.ReflectionOnlyLoad(assemblyName.FullName);
            }
            catch (FileNotFoundException)
            {
                string dllPath = Path.Combine(basePath, assemblyName.Name + ".dll");
                Assembly.ReflectionOnlyLoadFrom(dllPath);    
            }
        }
    }
}