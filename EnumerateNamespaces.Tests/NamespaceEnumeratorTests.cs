namespace EnumerateNamespaces.Tests
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class NamespaceEnumeratorTests
    {
        [Test]
        public void CanEnumerateNamespacesInThisAssembly()
        {
            var result = NamespaceEnumerator.GetNamespaces("EnumerateNamespaces.Tests.dll");

            Assert.That(result.Contains("EnumerateNamespaces.Tests"));
            Assert.That(result.Contains("SecondNamespace"));
        }

        [Test]
        public void DeduplicatesNamespaces()
        {
            var result = NamespaceEnumerator.GetNamespaces("EnumerateNamespaces.Tests.dll");

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
