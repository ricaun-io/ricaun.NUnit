using NUnit.Framework;
using ricaun.NUnit.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: AssemblyMetadata("Custom", "Custom")]

namespace ricaun.NUnit.Tests
{
    public class ReferenceLoaderUtils_Tests
    {
        [Test]
        public void ReferenceLoaderUtils_Tests_GetReferences()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var names = ReferenceLoaderUtils.GetReferencedAssemblies(assemblyFile);
            Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("nunit.framework")));
            Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("ricaun.NUnit")));
        }

        [Test]
        public void ReferenceLoaderUtils_Tests_GetReferencesRepeat()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            for (int i = 0; i < 5; i++)
            {
                var names = ReferenceLoaderUtils.GetReferencedAssemblies(assemblyFile);
                Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("nunit.framework")));
                Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("ricaun.NUnit")));
            }
            Assert.Zero(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().Length);
        }

        [Test]
        public void ReferenceLoaderUtils_Tests_GetReferencesRepeat_Assemblies()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var startWith = AppDomain.CurrentDomain.GetAssemblies().Length;
            for (int i = 0; i < 5; i++)
            {
                var names = ReferenceLoaderUtils.GetReferencedAssemblies(assemblyFile);
                Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("nunit.framework")));
                Assert.IsNotNull(names.FirstOrDefault(e => e.Name.StartsWith("ricaun.NUnit")));
            }
            var endWith = AppDomain.CurrentDomain.GetAssemblies().Length;
            Assert.Zero(endWith - startWith);
        }

        [TestCase("ricaun.NUnit.Tests")]
        [TestCase("SampleMetadata")] // Test custom attribute in SampleMetadataAttribute
        [TestCase("SampleMetadataAttribute")]
        public void ReferenceLoaderUtils_Tests_GetMetadataAttributes(string filePath)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var assemblyFile = Directory.GetFiles(location, $"{filePath}*.dll", SearchOption.AllDirectories).FirstOrDefault();
            Console.WriteLine(Path.GetFileName(assemblyFile));

            var startWith = AppDomain.CurrentDomain.GetAssemblies().Length;
            var assemblyMetadataAttributes = ReferenceLoaderUtils.GetAssemblyMetadataAttributes(assemblyFile);
            foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
            {
                Console.WriteLine($"{assemblyMetadataAttribute.Key}: {assemblyMetadataAttribute.Value}");
            }
            Assert.NotZero(assemblyMetadataAttributes.Length);

            var endWith = AppDomain.CurrentDomain.GetAssemblies().Length;
            Assert.Zero(endWith - startWith);
        }
    }

}
