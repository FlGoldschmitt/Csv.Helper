using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using FG.Csv.Helper.Extensions;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Allows to write a csv file.
    /// </summary>
    public class CsvWriter : IDisposable
    {
        #region private fields

        private StreamWriter _writer;
        private CsvConfiguration _configuration;

        #endregion private fields

        #region properties

        protected StreamWriter Writer => _writer;
        
        protected CsvConfiguration Configuration => _configuration;

        #endregion properties

        #region constructor

        /// <summary>
        /// Create a CsvWriter instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="filePath">Location of the csv file to be write.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvWriter(CsvConfiguration configuration, string filePath)
        {
            // An invalid object is not allowed to be created.
            filePath.CheckingFilePath(false, true);

            _configuration = configuration;
            _writer = new StreamWriter(filePath);
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Writes all the lines in the file from a string list where each element represents a line.
        /// </summary>
        /// <param name="records">List of lines to be written (value1[SEPARATOR]value2[SEPARATOR]...).</param>
        public void WriteRecords(IEnumerable<string> records)
        {
            WriteRecord(records.First(), false);
            foreach (var record in records.Skip(1))
                WriteRecord(record);
        }

        /// <summary>
        /// Writes a line to the file.
        /// </summary>
        /// <param name="record">Line to be written.</param>
        /// <param name="addline">
        /// Forces a line break before writing a newline (default:true).
        /// Prevents a line break at the end of the csv file.
        /// </param>
        public void WriteRecord(string record, bool addline = true)
        {
            StringBuilder sb = new StringBuilder();

            if (addline)
                sb.AppendLine();

            bool isFirst = true;
            foreach (var property in record.Split(Configuration.Separator))
            {
                sb.Append($"{(isFirst ? string.Empty : Configuration.Separator.ToString())}{property}");
                isFirst = false;
            }

            Writer.Write(sb.ToString());
        }

        /// <summary>
        /// Writes the headers of the file.
        /// </summary>
        /// <param name="headers">Contains the header line.</param>
        public void SetHeaders(string headers)
        {
            Writer.WriteLine(headers);
        }

        #endregion public methods

        #region implementation IDisposable

        public void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }

        #endregion implementation IDisposable
    }

    /// <summary>
    /// Allows to write a csv file using a generic type parameter inheriting from CsvModel.
    /// </summary>
    /// <typeparam name="T">Generic type inheriting from CsvModel.</typeparam>
    public class CsvWriter<T> : CsvWriter where T : CsvModel
    {
        #region constructor

        /// <summary>
        /// Create a CsvWriter instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="filePath">Location of the csv file to be write.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvWriter(CsvConfiguration configuration, string filepath)
            : base(configuration, filepath)
        { }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Writes all the lines of the file from a list where each item is an instance of the given generic type.
        /// </summary>
        /// <param name="records">List of objects to be written (property1[SEPARATOR]property2[SEPARATOR]...).</param>
        public void WriteRecords(IEnumerable<T> records)
        {
            WriteRecord(records.First(), false);
            foreach (var record in records.Skip(1))
                WriteRecord(record);
        }

        /// <summary>
        /// Writes a line into the file from an instance of the given generic type.
        /// </summary>
        /// <param name="record">Line to be written.</param>
        /// <param name="addline">
        /// Forces a line break before writing a newline (default:true).
        /// Prevents a line break at the end of the csv file.
        /// </param>
        public void WriteRecord(T record, bool addline = true)
        {
            StringBuilder sb = new StringBuilder();

            if (addline)
                sb.AppendLine();

            bool isFirst = true;
            foreach (var property in record.GetOrderedProperties())
            {
                sb.Append($"{(isFirst ? string.Empty : Configuration.Separator.ToString())}{property.GetValue(record)}");
                isFirst = false;
            }

            Writer.Write(sb.ToString());
        }

        /// <summary>
        /// Writes the file headers from the CsvHelper attributes present on the model.
        /// </summary>
        public void SetHeaders()
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;
            foreach (var property in ((T)Activator.CreateInstance(typeof(T))).GetOrderedProperties())
            {
                // Retrieve the "Name" attribute on the property.
                string propName = ((CsvHelperAttribute)property.GetCustomAttributes(typeof(CsvHelperAttribute), true).First()).Name;

                // If the attribute does not exist, we take the name of the property.
                if (string.IsNullOrEmpty(propName))
                    propName = property.Name;

                sb.Append($"{(isFirst ? string.Empty : Configuration.Separator.ToString())}{propName}");
                isFirst = false;
            }

            base.SetHeaders(sb.ToString());
        }

        #endregion public methods
    }
}
