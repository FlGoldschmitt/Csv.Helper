using System;
using System.IO;

namespace FG.Csv.Helper.Extensions
{
    /// <summary>
    /// Extension class for the String type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks the validity of a csv file.
        /// </summary>
        /// <param name="filePath">Path of the file to be checked.</param>
        /// <param name="verifyFileExists">Checks if the file exists or not.</param>
        /// <param name="verifyParentFolderExists">Checks if the parent folder exists or not.</param>
        public static void CheckingFilePath(this string filePath, bool verifyFileExists = false, bool verifyParentFolderExists = false)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath), "Value cannot be null.");
            if (filePath.Length == 0 || filePath == " ") throw new ArgumentException("Empty path name is not legal.", nameof(filePath));
            if (Path.GetExtension(filePath)?.ToLower() != ".csv") throw new ArgumentException("Only .csv extensions are accepted.");

            if (verifyFileExists)
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Could not find file '{filePath}'.");

            if (verifyParentFolderExists)
                if (!Directory.GetParent(filePath).Exists)
                    throw new DirectoryNotFoundException($"Could not find parent directory {filePath}.");
        }

        /// <summary>
        /// Checks the validity of a folder.
        /// </summary>
        /// <param name="folderPath">Path of the folder to be checked.</param>
        public static void CheckingFolderPath(this string folderPath)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath), "Value cannot be null.");
            if (folderPath.Length == 0 || folderPath == " ") throw new ArgumentException("Empty path name is not legal.", nameof(folderPath));
            if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException($"Could not find parent directory {folderPath}.");
        }
    }
}