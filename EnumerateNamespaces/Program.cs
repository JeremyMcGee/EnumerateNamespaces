namespace EnumerateNamespaces
{
    using System;
    using System.Collections.Generic;

    using ManyConsole;

    public static class Program
    {
        public static int Main(params string[] args)
        {
            var commands = GetCommands();
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        private static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
