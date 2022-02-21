using System.IO;

namespace FG.Csv.Helper.UnitTests
{
    /// <summary>
    /// Class for managing paths in the project.
    /// </summary>
    public static class PathManager
    {
        /// <summary>
        /// Gets the root path of the program execution.
        /// </summary>
        public static string GetRootPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Gets the path to the folder containing the example files.
        /// </summary>
        public static string GetSampleFilesFolderPath()
        {
            string directory = Path.Combine(GetRootPath(), "Files");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        /// <summary>
        /// Gets the path to the example csv file.
        /// </summary>
        public static string GetSampleFilePath(string filename)
        {
            return Path.Combine(GetSampleFilesFolderPath(), filename);
        }

        /// <summary>
        /// Gets the path to the folder containing the results files.
        /// </summary>
        public static string GetResultsFolderPath()
        {
            string directory = Path.Combine(GetSampleFilesFolderPath(), "Results");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        /// <summary>
        /// Simulates a non existing path.
        /// </summary>
        public static string GetNotExistsFolderPath()
        {
            return Path.Combine(GetSampleFilesFolderPath(), "NotExists");
        }

        /// <summary>
        /// Gets the path to the example csv file.
        /// </summary>
        public static string GetResultFilePath(string filename)
        {
            return Path.Combine(GetResultsFolderPath(), filename);
        }
    }
}