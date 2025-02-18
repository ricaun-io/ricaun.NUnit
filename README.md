# ricaun.NUnit

ricaun.NUnit is a package to manage the Load and Test assemblies using the [NUnit](https://www.nuget.org/packages/NUnit/) Attributes as patterns.

[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](https://github.com/ricaun-io/ricaun.NUnit)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/ricaun-io/ricaun.NUnit/actions/workflows/Build.yml/badge.svg)](https://github.com/ricaun-io/ricaun.NUnit/actions)
[![NUnit](https://img.shields.io/badge/NUnit-3.13.3-blue)](https://www.nuget.org/packages/NUnit)
[![nuget](https://img.shields.io/nuget/v/ricaun.NUnit?logo=nuget)](https://www.nuget.org/packages/ricaun.NUnit)

## Features
```C#
var location = Assembly.GetExecutingAssembly().Location;
var test = TestEngine.TestAssembly(location);
```

### Test Attributes

The tests use the [NUnit](https://www.nuget.org/packages/NUnit/) Attributes `[Test]` to execute a method test. 
* The attribute `[SetUp]` and `[TearDown]` is executed for each method with the attribute `[Test]`.
* The attribute `[OneTimeSetUp]` and `[OneTimeTearDown]` is executed one time before each method with the attribute `[Test]`.
* The attribute `[Ignore]` makes the class or method to be ignored.
* The attribute `[Order]` makes the order that the test will run in.
* The attribute `[Explicit]` works only if the `Filter` is enable.
* The attribute `[TestCase]` disable the `Optional Parameters` 
* The attribute `[TestCaseSource]` disable the `Optional Parameters` 
* The attribute `[TestFixture]` disable the `Optional Parameters` in the constructor.
* The attribute `[TestFixtureSource]` disable the `Optional Parameters` in the constructor.

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

    [Test(ExpectedResult = 1)]
    public int ResultTest()
    {
        return 1;
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void CasesTest(int i)
    {
        Assert.True(i > 0);
    }

    public static int[] CasesSource = new[] { 1, 2, 3 };
    [TestCaseSource(nameof(CasesSource))]
    public void CasesSourceTest(int i)
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
To force the `Assembly` to load references in another folder using `directoryResolver` as a directory.
```C#
var location = Assembly.GetExecutingAssembly().Location;
string[] tests = TestEngine.GetTestFullNames(location, directoryResolver);
```

The name of the test equal to: `Namespace`.`Type`.`Method`.`TestNameAlias`.

### TestFullName

Is possible to get test FullName using `TestModel`.
```C#
TestModel test;
string testFullName = test.FullName;
```

The name of the test equal to: `TypeName`.`TestName`.`TestAlias`.

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

### Result

`Result` allow receiving a test result when the `TestEngine` is running.
Use `TestEngine.Result` to apply an interface `ITestModelResult`.

``` C#
TestEngine.Result = new TestModelResult((testModel) =>
{
    Debug.WriteLine(testModel);
});
var location = Assembly.GetExecutingAssembly().Location;
var test = TestEngine.TestAssembly(location);
TestEngine.Result = null;
```

### Fail

`Fail` allow to create a test with a custom `Exception` for each `testNames`.

``` C#
var exception = new Exception();
var test = TestEngine.Fail(location, exception);
```
``` C#
var testNames = new string[] { "Test1", "Test2" };
var test = TestEngine.Fail(location, exception, testNames);
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

* [Latest release](https://github.com/ricaun-io/ricaun.NUnit/releases/latest)

## License

This project is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](https://github.com/ricaun-io/ricaun.NUnit/stargazers)!