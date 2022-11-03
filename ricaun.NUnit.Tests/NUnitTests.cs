﻿using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Tests
{
    public class NUnitTests
    {
        private const string fileName = "SampleTest.Tests.dll";
        private string pathFile;

        public NUnitTests()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pathFile = Path.Combine(location, fileName);
        }

        [Test]
        public void TestAssemblyService()
        {
            Console.WriteLine(fileName);
            var service = new Services.TestAssemblyService(pathFile);
            foreach (var method in service.GetTestTypeMethods())
            {
                Console.WriteLine($"Test Method: {service.GetMethodFullName(method)}");
                try
                {
                    using (var instence = new Services.InstanceDisposable(method.DeclaringType))
                    {
                        try
                        {
                            var testAttribute = service.GetAttribute<TestAttribute>(method);
                            var result = testAttribute.ExpectedResult ?? null;
                            var value = instence.Invoke(method);

                            var equals = (value is not null) ? value.Equals(result) : result is null;
                            Console.WriteLine($"\t {method.Name} {equals} \t {value} {value is null} {result}");
                        }
                        catch (Exception ex)
                        {
                            var exInner = ex.InnerException is null ? ex : ex.InnerException;
                            switch (exInner)
                            {
                                case SuccessException success:
                                    break;
                                case IgnoreException ignore:
                                    break;
                                case AssertionException assertion:
                                    break;
                                default:
                                    throw exInner;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().GetBaseException());
                    //Console.WriteLine(ex.TargetSite);
                    //Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        [Test]
        public void TestAssembly()
        {
            Console.WriteLine(fileName);
            var json = TestEngine.TestAssembly(pathFile);
            Console.WriteLine(json);

            foreach (var test in json.Tests)
            {
                foreach (var t in test.Tests)
                {
                    Console.WriteLine($"\t{t}");
                }
            }

            Assert.IsTrue(json.TestCount > 0, $"{fileName} with no Tests.");
        }
    }
}