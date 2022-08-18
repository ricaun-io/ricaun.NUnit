using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.NUnit.Models
{
    /// <summary>
    /// TestTypeModel
    /// </summary>
    public class TestTypeModel : TestModel
    {
        /// <summary>
        /// Tests
        /// </summary>
        public List<TestModel> Tests { get; set; } = new();

        /// <summary>
        /// Test Count
        /// </summary>
        public int TestCount => Tests.Count;

        /// <summary>
        /// SuccessHate
        /// </summary>
        public double SuccessHate => GetSuccessHate();
        private double GetSuccessHate()
        {
            var count = Tests.Count;
            var success = Success ? 1.0 : 0;
            return count == 0 ? success : Math.Round(Tests.Where(e => e.Success).Count() / (double)count, 2);
        }
    }
}
