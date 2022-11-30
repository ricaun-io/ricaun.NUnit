using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.NUnit.Models
{
    /// <summary>
    /// TestAssemblyModel
    /// </summary>
    public class TestAssemblyModel : TestModel
    {
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// SuccessHate
        /// </summary>
        public double SuccessHate => GetSuccessHate();

        /// <summary>
        /// Tests
        /// </summary>
        public List<TestTypeModel> Tests { get; set; } = new();

        /// <summary>
        /// Test Count
        /// </summary>
        public int TestCount => Tests.SelectMany(e => e.Tests).Count();

        private double GetSuccessHate()
        {
            var tests = Tests.SelectMany(e => e.Tests);
            var count = tests.Count();
            var success = Success ? 1.0 : 0;
            return count == 0 ? success : Math.Round(tests.Where(e => e.Success).Count() / (double)count, 2);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} {TestCount} {Success} {Math.Round(SuccessHate * 100, 2)}% {Time}ms";
        }

        /// <summary>
        /// Show in the Console
        /// </summary>
        public string AsString()
        {
            var text = $"{this}\n";
            foreach (var test in this.Tests)
            {
                text += $"\t{test}\n";
                foreach (var t in test.Tests)
                {
                    text += $"\t\t{t}\n";
                }
            }
            return text;
        }
    }
}
