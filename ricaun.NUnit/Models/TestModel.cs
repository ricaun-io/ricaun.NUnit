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
        /// Test Alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Test FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Test Success?
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Test Skipped
        /// </summary>
        public bool Skipped { get; set; } = false;

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
            var result = Skipped ? "Skipped" : Success ? "Passed" : "Failed";
            var name = string.IsNullOrEmpty(Alias) ? Name : Alias != Name ? $"{Alias}[{Name}]" : Alias;
            var message = !Message.Contains("Exception") ? Message : Message.Substring(0, Message.IndexOf("Exception") + "Exception".Length);
            return $"{name}\t{result}\t{message}".Replace("\n", " ").Replace("\r", " ");
        }
    }
}
