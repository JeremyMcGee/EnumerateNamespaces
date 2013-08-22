namespace EnumerateNamespaces
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ManyConsole;

    public class NunitCommand : ConsoleCommand
    {
        private string nunitAssembly;

        public NunitCommand()
        {
            SetupArguments();
        }

        private void SetupArguments()
        {
            base.SkipsCommandSummaryBeforeRunning();

            IsCommand("run", "Run NUnit tests in the given assembly, parallelizing across namespaces.");

            HasRequiredOption<string>(
                "n|nunitAssemblyName=", "The name of the NUnit test assembly to run.", v => nunitAssembly = v);
        }

        public override int Run(string[] remainingArguments)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            GetTempLocations(nunitAssembly)
                //.Take(10)
                .AsParallel()
                //.WithDegreeOfParallelism(6)
                .ForAll(
                tempLocation =>
                {
                    ExecuteNUnit(tempLocation.NUnitAssemblyLocation, tempLocation.NamespaceName);
                    tempLocation.Remove();
                });

            Console.WriteLine("Done in {0}.", stopwatch.Elapsed);
            return 0;
        }

        private IEnumerable<TempLocation> GetTempLocations(string nunitAssembly)
        {
            foreach (var namespaceName in NamespaceEnumerator.GetNamespaces(nunitAssembly))
            {
                yield return TempLocation.Create(nunitAssembly, namespaceName);
            }
        }

        private void ExecuteNUnit(string nunitAssembly, string namespaceName)
        {
            int finalProcessState = 0;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = "nunit-console.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = string.Format(
                "{0} /run={1} /noshadow /result={1}.xml",
                nunitAssembly,
                namespaceName);

            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
                finalProcessState = exeProcess.ExitCode;
            }

            Console.WriteLine("{0} {1}",
                namespaceName,
                (finalProcessState == 0) ? "OK" : "ERROR!");
        }

        private class TempLocation
        {
            private string path;

            public string NUnitAssemblyLocation { get; private set; }

            public string NamespaceName { get; private set; }

            public static TempLocation Create(string nunitAssembly, string namespaceName)
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);
                string nunitAssemblyLocation = Path.GetDirectoryName(nunitAssembly);
                string nunitAssemblyFilename = Path.GetFileName(nunitAssembly);

                foreach (var fileToCopy in Directory.EnumerateFiles(nunitAssemblyLocation))
                {
                    string destination = Path.Combine(tempFolder, Path.GetFileName(fileToCopy));
                    File.Copy(fileToCopy, destination);
                }

                return new TempLocation
                {
                    path = tempFolder,
                    NUnitAssemblyLocation = Path.Combine(tempFolder, nunitAssemblyFilename),
                    NamespaceName = namespaceName
                };
            }

            public void Remove()
            {
                Thread.Sleep(1000);
                Directory.Delete(path, recursive: true);
            }
        }
    }
}