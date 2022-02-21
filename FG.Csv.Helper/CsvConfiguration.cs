using System.Text;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Contains the settings for the csv Helper tool.
    /// </summary>
    public class CsvConfiguration
    {
        /// <summary>
        /// Separator of the csv file (default: ;).
        /// </summary>
        public char Separator { get; set; } = ';';

        /// <summary>
        /// Number of threads used in parallel when reading/writing (default: 1).
        /// </summary>
        public int DegreeOfParallelism { get; set; } = 1;

        /// <summary>
        /// Presence or not of a file header (default: false). Not used for the writer (use SetHeaders method).
        /// </summary>
        public bool HasHeaders { get; set; } = false;

        /// <summary>
        /// Encoding type of the csv file (default: Encoding.Default).
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;
    }
}