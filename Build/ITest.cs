﻿using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Components;
using ricaun.Nuke.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

public interface ITest : ICompile, IHazContent
{
    [Parameter]
    bool TestResults => TryGetValue<bool?>(() => TestResults) ?? true;

    [Parameter]
    bool TestBuildStopWhenFailed => TryGetValue<bool?>(() => TestBuildStopWhenFailed) ?? true;

    [Parameter]
    string TestProjectName => TryGetValue<string>(() => TestProjectName) ?? "*.Tests";

    Target Test => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var testFailed = false;
            var testProjects = Solution.GetProjects(TestProjectName);

            foreach (var testProject in testProjects)
            {
                var testResultsDirectory = GetContentDirectory(testProject);

                var configurations = testProject.GetReleases();
                foreach (var configuration in configurations)
                {
                    try
                    {
                        Serilog.Log.Logger.Information($"Build: {testProject.Name} {configuration}");
                        testProject.Build(configuration);
                        Serilog.Log.Logger.Information($"Test: {testProject.Name} {configuration}");

                        DotNetTasks.DotNetTest(_ => _
                            .SetProjectFile(testProject)
                            .SetConfiguration(configuration)
                            .EnableNoBuild()
                            .When(TestResults, _ => _
                                .SetLoggers("trx")
                                .SetResultsDirectory(testResultsDirectory)));
                    }
                    catch (System.Exception ex)
                    {
                        Serilog.Log.Logger.Information($"Exception: {ex}");
                    }

                    if (TestResults)
                        testFailed |= ReportTestCount(testProject, configuration);
                }
            }

            if (testFailed & TestBuildStopWhenFailed)
            {
                throw new Exception($"Test Failed = {testFailed}");
            }

        });

    /// <summary>
    /// ReportTestCount
    /// </summary>
    /// <param name="testProject"></param>
    /// <param name="configuration"></param>
    /// <returns>Return if fail some test</returns>
    bool ReportTestCount(Project testProject, string configuration = "")
    {
        var testResultsDirectory = GetContentDirectory(testProject);

        IEnumerable<string> GetOutcomes(AbsolutePath file)
            => XmlTasks.XmlPeek(
                file,
                "/xn:TestRun/xn:Results/xn:UnitTestResult/@outcome",
                ("xn", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"));

        var resultFiles = PathConstruction.GlobFiles(testResultsDirectory, "*.trx");
        var outcomes = resultFiles.SelectMany(GetOutcomes).ToList();
        var passedTests = outcomes.Count(x => x == "Passed");
        var failedTests = outcomes.Count(x => x == "Failed");
        var skippedTests = outcomes.Count(x => x == "NotExecuted");

        Serilog.Log.Logger.Warning($"Project: {testProject.Name} ({configuration}) \t Passed: {passedTests} \t Failed: {failedTests} \t Skipped: {skippedTests}");

        return failedTests > 0;
    }

}