namespace EnumerateNamespaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using ManyConsole;

    public class EnumerateCommand : ConsoleCommand
    {
        private string assemblyName;

        public EnumerateCommand()
        {
            SetupArguments();
        }

        private void SetupArguments()
        {
            base.SkipsCommandSummaryBeforeRunning();

            IsCommand("list", "List namespaces in the given assembly.");

            HasRequiredOption<string>(
                "a|assemblyName=", "The name of the assembly to enumerate.", v => assemblyName = v);
        }

        public override int Run(string[] remainingArguments)
        {
            foreach (var namespaceName in GetNamespaces(assemblyName))
            {
                Console.WriteLine(namespaceName);
            }

            return 0;
        }

        public IEnumerable<string> GetNamespaces(string assemblyName)
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
                return (from Type type in types select type.Namespace).Distinct();

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

        private void LoadDependentAssembly(string basePath, AssemblyName assemblyName)
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
