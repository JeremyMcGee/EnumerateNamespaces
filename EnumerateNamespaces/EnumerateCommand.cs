namespace EnumerateNamespaces
{
    using System;

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
            foreach (var namespaceName in NamespaceEnumerator.GetNamespaces(assemblyName))
            {
                Console.WriteLine(namespaceName);
            }

            return 0;
        }
    }
}
