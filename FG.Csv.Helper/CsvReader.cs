using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using FG.Csv.Helper.Extensions;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Allows to read a csv file.
    /// </summary>
    public class CsvReader : IDisposable
    {
        #region private fields

        private string _filePath;

        private CsvConfiguration _configuration;

        private string _headers;

        #endregion private fields

        #region properties

        /// <summary>
        /// Location of the csv file to be read.
        /// </summary>
        protected string FilePath => _filePath;

        /// <summary>
        /// Csv parameters.
        /// </summary>
        protected CsvConfiguration Configuration => _configuration;

        /// <summary>
        /// File headers.
        /// </summary>
        public string Headers => _headers;

        #endregion properties

        #region constructor

        /// <summary>
        /// Create a CsvReader instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="filePath">Location of the csv file to be read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public CsvReader(CsvConfiguration configuration, string filePath)
        {
            // An invalid object is not allowed to be created.
            filePath.CheckingFilePath(true);

            _configuration = configuration;
            _filePath = filePath;
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Retrieves all records from the file as a string array where each item represents a line.
        /// </summary>
        /// <returns>Returns an array of string representing each line of the file.</returns>
        public string[] GetRecords()
        {
            var result = File.ReadAllLines(_filePath, _configuration.Encoding);

            if (_configuration.HasHeaders)
            {
                // Save the headers
                _headers = result.Take(1).SingleOrDefault();
                
                // The first line is excluded if the file has headers.
                result = result.Skip(1).ToArray();
            }

            return result;
        }

        /// <summary>
        /// Performs an action on all records in the file in parallel.
        /// </summary>
        /// <param name="action">Action to be performed for each record.</param>
        public void PerformActionAsParallel(Action<string[]> action)
        {
            GetRecords()
                .Select(l => l.Split(_configuration.Separator))
                .AsParallel()
                .WithDegreeOfParallelism(_configuration.DegreeOfParallelism)
                .ForAll(action);
        }

        #region implementation IDisposable

        public void Dispose() { }

        #endregion implementation IDisposable

        #endregion public methods
    }

    /// <summary>
    /// Allows to read a csv file using a generic type parameter inheriting from CsvModel.
    /// </summary>
    /// <typeparam name="T">Generic type inheriting from CsvModel.</typeparam>
    public class CsvReader<T> : CsvReader where T : CsvModel
    {
        #region constructor

        /// <summary>
        /// Create a CsvReader instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="filePath">Location of the csv file to be read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public CsvReader(CsvConfiguration configuration, string filePath)
            : base(configuration, filePath)
        { }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Retrieves all records from the file as a list where each item is an instance of the given generic type.
        /// </summary>
        /// <returns>Returns a collection of objects of the given generic type representing each line of the file.</returns>
        /// <exception cref="CsvException"></exception>
        public new IEnumerable<T> GetRecords()
        {
            var records = new List<T>();
            foreach (var line in base.GetRecords())
                records.Add((T)((T)Activator.CreateInstance(typeof(T))).Fill(line.Split(Configuration.Separator)));

            return records;
        }

        /// <summary>
        /// Retrieves all records out of order from the file as a list where each item is an instance of the given generic type.
        /// (Unordered records but the method is faster (depending on the DegreeOfParallelism parameter))
        /// </summary>
        /// <returns>Returns a unordered collection of objects of the given generic type representing each line of the file</returns>
        /// <exception cref="CsvException"></exception>
        public IEnumerable<T> GetUnorderedRecords()
        {
            var records = new ConcurrentStack<T>();

            base.PerformActionAsParallel(l => records.Push((T)((T)Activator.CreateInstance(typeof(T))).Fill(l)));

            return records;
        }

        /// <summary>
        /// Performs an action on all records in the file in parallel.
        /// </summary>
        /// <param name="action">Action to be performed for each record.</param>
        /// <exception cref="CsvException"></exception>
        public void PerformActionAsParallel(Action<T> action)
        {
            PerformActionAsParallel(GetUnorderedRecords(), action);
        }

        /// <summary>
        /// Performs an action on each record in the list in parallel.
        /// </summary>
        /// <param name="records">List of records to be processed.</param>
        /// <param name="action">Action to be performed for each record.</param>
        public void PerformActionAsParallel(IEnumerable<T> records, Action<T> action)
        {
            records
                .AsParallel()
                .WithDegreeOfParallelism(Configuration.DegreeOfParallelism)
                .ForAll(action);
        }

        #endregion public methods
    }
}