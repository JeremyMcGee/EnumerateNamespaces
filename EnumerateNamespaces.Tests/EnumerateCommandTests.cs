namespace EnumerateNamespaces.Tests
{
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class EnumerateCommandTests
    {
        [Test]
        public void CanEnumerateNamespacesInThisAssembly()
        {
            EnumerateCommand command = new EnumerateCommand();

            var result = command.GetNamespaces("EnumerateNamespaces.Tests.dll");

            Assert.That(result.Contains("EnumerateNamespaces.Tests"));
            Assert.That(result.Contains("SecondNamespace"));
        }

        [Test]
        public void DeduplicatesNamespaces()
        {
            EnumerateCommand command = new EnumerateCommand();

            var result = command.GetNamespaces("EnumerateNamespaces.Tests.dll");

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}

namespace SecondNamespace
{
    public class SecondClass
    {
    }

    public class AnotherClass
    {
    }
}
