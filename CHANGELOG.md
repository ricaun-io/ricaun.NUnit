# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.2.5] / 2023-03-27
### Fixed
- Fix `Explicit` not working, add test.

## [1.2.4] / 2023-03-24
### Features
- [x] Read `Ignore` and `Explicit` reason in the `TestModel` message.

## [1.2.3] / 2023-03-15
### Features
- [x] Force Task Tests to run in the with single-threaded apartment.
- Try Force `TestEngine` to use `RunSafe` if current thread is `STA` (Unable to use `RunSafe`, Revit outside context problem).
### Updated
- Add `TaskSTA` - run Task with single-threaded apartment.
- Add `RunSafe` in `TaskSTA`, create a safe single-threaded if current thread is `STA`.
- Update `Revit` Commands and Tests, speed test / context test.
- Update `VoidTaskResult` return `null`
- Update `TestModel` ToString single line.

## [1.2.2] / 2023-03-14
### Features
- [x] Enable Task Tests to work with the Framework
- [x] Async Test Timeout - `TestEngineFilter.CancellationTokenTimeOut`
### Updated
- Update `TestModel`
- Update `AppDomainUtils`
- Add `TaskTests`
- Add `InvokeTask`
- Add `IsReturnTypeEqualsTask`
- Add `IsParameterTypeSimilar`

## [1.2.1] / 2023-02-15
### Features
- `AssemblyResolveService` ignore assembly version
### Updated
- Update `AssemblyResolveService` with `IsNameAndCultureEquals`

## [1.2.0] / 2023-02-13
### Features
- Support `.NET Standard 2.0` and `.NET Framework 4.5`
### Updated
- Remove `Application Revit UI` from `Debug`
- Remove obsolete `TestEngine.GetTestFullName` method
### Fixed
- Fix missing Services to `internal`

## [1.1.2] / 2023-02-13
### Features
- Feature `TestEngine.GetTestFullNames` with `directoryResolver`
- Feature `AssemblyResolveService` improved
### Added
- Add new `AssemblyResolveService`
- Add `InternalsVisibleTo` in the csproj to enable Tests
### Updated
- Update all services to internal
- Update `AssemblyResolveService` to `AssemblyResolveCurrentDirectoryService`

## [1.1.1] / 2022-12-21
### Features
- Feature `TestModel` with FullName
- Feature `TestEngine.Result` to invoke `TestModel` when finish using `ITestModelResult`
### Changed
- Change `TestEngine` to partial to add `Result` feature
### Added
- Add `TestEngine.Result`
- Add `TestModelResult` and `ITestModelResult`

## [1.1.0] / 2022-12-06
### Features
- Feature `TestInstance` generete all `TestModel` if failed or ignored.
- Feature `TestEngine.GetTestFullName` to compare with `TestEngine.GetTestFullNames`.
### Fixed
- Fix Multiple AssertionResults: `TestExecutionContextUtils.Clear();`
### Updated
- Update core `TestAssemblyService` and `TestService` to add fail tests using `TestInstance` method
### Tests
- Add `TestsExplict` to test failed and ExplictAttribute

## [1.0.9] / 2022-12-06
### Features
- Feature `TestEngine.Initialize` to force load `NUnit` reference.

## [1.0.8] / 2022-12-05
### Features
- Feature `ValidateTestAssemblyNUnitVersion` - NUnit version `3.13.3.0`
### Added
- Add `ValidateTestAssemblyNUnitVersion`
- Add Command/TestUtils

## [1.0.7] / 2022-12-01
### Features
- Feature `TestEngine.GetTestFullNames` supported
- Feature `WildcardPattern` filter supported
- Feature `TestCaseAttribute` supported
### Added
- Add `Alias` in `TestModel`
- Add `ExplicitEnabled` in Filter
- Add `WildcardPattern`
- Add `HasFilterTestMethod`
- Update `GetFilterTestMethods`
### Fixed
- Fix `SetUp` & `TearDown`
### Tests
- Test Static with `SetUp` & `TearDown`

## [1.0.6] / 2022-11-30 - 2022-10-31
### Features
- Add `TestEngineFilter` simple filter by method/test name
- Add SampleTest Project
- Local Test with `SampleTest` in Revit
- Skipped Test Parameter
- Test with `ExpectedResult` feature
### Tests
- Add `ricaun.NUnit.Tests`
- Add `SampleTest.Tests`
### Added
- Add `ITest` in Build
- Add `GetMethodTestNames` - Get Test Names
- Add `IsValueExpectedResult` case expected result
- Add `Skipped` feature
- Add `IgnoreException` feature

## [1.0.5] / 2022-09-20
### Features
- Remove await / async
### Updated
- Update Version and VersionNUnit
- Ignore Task methods

## [1.0.4] / 2022-09-20
### Features
- Force to `LoadReferencedAssemblies`
- Fix `ContainNUnit`
- Remove `ValidateTestAssemblyNUnitVersion`
- Add Extension for Debug

## [1.0.3] / 2022-09-17
### Features
- TestEngine.ContainNUnit()
- class CurrentDirectory
- class AssemblyResolveService

## [1.0.2] / 2022-09-12
### Changed
- Change Test Name to AssemblyName minus Version

## [1.0.1] / 2022-08-19
### Fixed
- Fix Console in Instance/Dispose Type

## [1.0.0] / 2022-08-18
### Features
- Test Assembly with diferent NUnit Version with LoadFile
- Test Methods with Attribute Name
- Test if Assembly contain the same NUnit Version
- Test Method with custom parameters
- Test Instance with custom parameters
- Test with Console Output

[vNext]: ../../compare/1.0.0...HEAD
[1.2.5]: ../../compare/1.2.4...1.2.5
[1.2.4]: ../../compare/1.2.3...1.2.4
[1.2.3]: ../../compare/1.2.2...1.2.3
[1.2.2]: ../../compare/1.2.1...1.2.2
[1.2.1]: ../../compare/1.2.0...1.2.1
[1.2.0]: ../../compare/1.1.2...1.2.0
[1.1.2]: ../../compare/1.1.1...1.1.2
[1.1.1]: ../../compare/1.1.0...1.1.1
[1.1.0]: ../../compare/1.0.9...1.1.0
[1.0.9]: ../../compare/1.0.8...1.0.9
[1.0.8]: ../../compare/1.0.7...1.0.8
[1.0.7]: ../../compare/1.0.6...1.0.7
[1.0.6]: ../../compare/1.0.5...1.0.6
[1.0.5]: ../../compare/1.0.4...1.0.5
[1.0.4]: ../../compare/1.0.3...1.0.4
[1.0.3]: ../../compare/1.0.2...1.0.3
[1.0.2]: ../../compare/1.0.1...1.0.2
[1.0.1]: ../../compare/1.0.0...1.0.1
[1.0.0]: ../../compare/1.0.0