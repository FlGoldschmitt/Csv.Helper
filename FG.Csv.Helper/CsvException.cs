using System;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Represents errors that occur during the execution of the utility library.
    /// </summary>
    public class CsvException : Exception
    {
        #region constructor

        public CsvException() 
            : base()
        { }

        public CsvException(string message)
            : base(message)
        { }

        public CsvException(string message, Exception innerException)
            : base(message, innerException)
        { }

        #endregion constructor

        #region public static methods

        /// <summary>
        /// Exception thrown when the object model and the csv file do not match.
        /// </summary>
        public static CsvException InvalidParameters()
        {
            return new CsvException(
                "The number of properties in the model and values in the file do not match. " +
                "Check your model, csv file and the 'Separator' parameter."
            );
        }

        /// <summary>
        /// Exception thrown when the index value of the CsvHelperAttribute is duplicated in the model.
        /// </summary>
        public static CsvException InvalidIndex(int index, string model)
        {
            return new CsvException($"The index {index} is present several times in your object model {model}.");
        }

        /// <summary>
        /// Exception thrown when the property with the CsvHelperAttribute is not found.
        /// </summary>
        public static CsvException PropertyNotFound(int index, string model)
        {
            return new CsvException($"No property found for the index {index} on the model {model}.");
        }

        #endregion public static methods
    }
}