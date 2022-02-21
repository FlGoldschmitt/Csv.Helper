using System;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Represents the custom attribute of the csv utility.
    /// </summary>
    public class CsvHelperAttribute : Attribute
    {
        /// <summary>
        /// Defines the position of a property in the csv file.
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// Defines the name of a property in the csv file when writing headers.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}