namespace ricaun.NUnit.Models
{
    /// <summary>
    /// Test Model
    /// </summary>
    public class TestModel
    {
        /// <summary>
        /// Test Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Test Success?
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Message Output
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Console Output
        /// </summary>
        public string Console { get; set; } = "";

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}\t{Success}";
        }
    }
}
