# ricaun.NUnit

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
* The attribute `[Ignore]`, `[TestCase]`, and `[Explicit]` makes the class or method to be ignored.

```C#
public class TestSampleClass
{
    [SetUp]
    public void RunBeforeTest()
    {
        Console.WriteLine("Execute RunBeforeTest");
    }

    [TearDown]
    public void RunAfterTest()
    {
        Console.WriteLine("Execute RunAfterTest");
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
}
```

## Release

* [Latest release](../../releases/latest)

## License

This project is [licensed](LICENSE) under the [MIT Licence](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!