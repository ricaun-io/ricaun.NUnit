# ricaun.NUnit

ricaun.NUnit is a package to manage the Load and Test assemblies using the NUnit Attributes as patterns.

[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](../..)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](../../actions/workflows/Build.yml/badge.svg)](../../actions)

## Features
```C#
var location = Assembly.GetExecutingAssembly().Location;
var test = TestEngine.TestAssembly(location);
```

### Test Attributes

The tests use the NUnit Attributes `[Test]` to execute a method test. 
* The attribute `[SetUp]` and `[TearDown]` is executed for each method with the attribute `[Test]`.
* The attribute `[OneTimeSetUp]` and `[OneTimeTearDown]` is executed one time before each method with the attribute `[Test]`.
* The attribute `[Ignore]` makes the class or method to be ignored.
* The attribute `[Explicit]` works only if the `Filter` is enable.
* The attribute `[TestCase]` disable the `Optional Parameters` 

```C#
public class TestSampleClass
{
    [OneTimeSetUp]
    public void OneBeforeTest()
    {
        Console.WriteLine("Execute OneBeforeTest");
    }

    [OneTimeTearDown]
    public void OneAfterTest()
    {
        Console.WriteLine("Execute OneAfterTest");
    }

    [SetUp]
    public void BeforeTest()
    {
        Console.WriteLine("Execute BeforeTest");
    }

    [TearDown]
    public void AfterTest()
    {
        Console.WriteLine("Execute AfterTest");
    }

    [Test]
    public void NormalTest()
    {
        Console.WriteLine("Execute NormalTest");
        Assert.True(true);
    }

    [Test]
    public void FailTest()
    {
        Console.WriteLine("Execute FailTest");
        Assert.True(false, "This is a custom fail message.");
    }

    [Test]
    public void PassTest()
    {
        Console.WriteLine("Execute PassTest");
        Assert.Pass("This is a custom pass message.");
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void CasesTest(int i)
    {
        Assert.True(i > 0);
    }
}
```

### TestFullNames

Is possible to get all the test names in the `Assembly`.
```C#
var location = Assembly.GetExecutingAssembly().Location;
string[] tests = TestEngine.GetTestFullNames(location);
```

The name of the test equal to: `namespace`.`type`.`method`.`testname`

### Filter

Is possible to add a custom filter to test only a specific test name, the filter uses `WildcardPattern`.

```C#
TestEngineFilter.Add("*"); // Select all tests
```

```C#
TestEngineFilter.Add("*.Test"); // Test endswith
```

```C#
TestEngineFilter.Add("*.Test1"); // Test endswith
TestEngineFilter.Add("*.Test2"); // Test endswith
```

```C#
TestEngineFilter.Add("namespace.type.method.Test1"); // Test specific name
TestEngineFilter.Add("namespace.type.method.Test2"); // Test specific name
```

If filter is enable the `[Explicit]` tests is not skipped.

```C#
TestEngineFilter.Reset(); // Reset filter
```

### Optional Parameters

Is possible to add optional parameters in the class or method tested. 
The custom arguments is selected based in the `Type` of the argument, if the `Type` is not found the test gonna execute with `null` argument.

```C#
var location = Assembly.GetExecutingAssembly().Location;
var test = TestEngine.TestAssembly(location, "This is a custom string parameter.");
```

The sample below show some implementation with a method with argument and `Constructor/IDisposable`.

```C#
public class TestParameterClass : IDisposable
{
    public TestParameterClass(string parameter)
    {
        Console.WriteLine($"Constructor TestParameterClass: {parameter}");
    }

    [Test]
    public void ParameterTest(string parameter)
    {
        Console.WriteLine($"ParameterTest: {parameter}");
        Assert.IsNotNull(parameter);
    }

    public void Dispose()
    {
        Console.WriteLine($"Dispose TestParameterClass");
    }
}
```


## Release

* [Latest release](../../releases/latest)

## License

This project is [licensed](LICENSE) under the [MIT Licence](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!