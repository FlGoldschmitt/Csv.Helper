using System;
using System.IO;
using System.Collections.Generic;

using FG.Csv.Helper.Extensions;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Allows to merge multiple csv files into a single file.
    /// </summary>
    public class CsvMerger : IDisposable
    {
        #region private fields

        private IEnumerable<string> _inputFilesPath;

        private string _outputFilePath;

        private CsvConfiguration _configuration;

        #endregion private fields

        #region properties

        /// <summary>
        /// Location of the files to be merged.
        /// </summary>
        protected IEnumerable<string> InputFilesPath => _inputFilesPath;

        /// <summary>
        /// Output file that will contain the result of the merge.
        /// </summary>
        protected string OutputFilePath => _outputFilePath;

        /// <summary>
        /// Csv parameters.
        /// </summary>
        protected CsvConfiguration Configuration => _configuration;

        #endregion properties

        #region constructor

        /// <summary>
        /// Create a CsvMerger instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="inputFilesPath">Location of the files to be merged.</param>
        /// <param name="outputFilePath">Output file containing the result of the merger.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvMerger(CsvConfiguration configuration, IEnumerable<string> inputFilesPath, string outputFilePath)
        {
            // An invalid object is not allowed to be created.
            if (inputFilesPath == null) throw new ArgumentNullException(nameof(inputFilesPath), "Value cannot be null.");

            foreach (var filePath in inputFilesPath)
                filePath.CheckingFilePath(true);

            outputFilePath.CheckingFilePath(false, true);

            _configuration = configuration;
            _inputFilesPath = inputFilesPath;
            _outputFilePath = outputFilePath;
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Merges multiple csv files into one.
        /// </summary>
        public void MergeRecords()
        {
            string headers = null;
            List<string> records = new List<string>();

            // Read and retrieve the contents of all input files.
            foreach (string file in _inputFilesPath)
                using (var reader = new CsvReader(_configuration, file))
                {
                    records.AddRange(reader.GetRecords());
                    
                    if (_configuration.HasHeaders)
                        headers = reader.Headers;
                }

            // Writing the results to the output file.
            using (var writer = new CsvWriter(_configuration, _outputFilePath))
            {
                if (_configuration.HasHeaders)
                    writer.SetHeaders(headers);

                writer.WriteRecords(records);
            }
        }

        #endregion public methods

        #region implementation IDisposable

        public void Dispose() { }

        #endregion implementation IDisposable
    }

    /// <summary>
    /// Allows to merge multiple csv files into a single file using a generic type parameter inheriting from CsvModel.
    /// </summary>
    /// <typeparam name="T">Generic type inheriting from CsvModel.</typeparam>
    public class CsvMerger<T> : CsvMerger where T : CsvModel
    {
        #region constructor

        /// <summary>
        /// Create a CsvMerger instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="inputFilesPath">Location of the files to be merged.</param>
        /// <param name="outputFilePath">Output file containing the result of the merger.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvMerger(CsvConfiguration configuration, IEnumerable<string> inputFilesPath, string outputFilePath)
            : base(configuration, inputFilesPath, outputFilePath)
        { }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Merges multiple csv files into one (reading and writing will be done from the given generic type).
        /// </summary>
        /// <exception cref="CsvException"></exception>
        public new void MergeRecords()
        {
            // Read and retrieve the contents of all input files.
            List<T> records = new List<T>();
            foreach (string file in InputFilesPath)
                using (var reader = new CsvReader<T>(Configuration, file))
                    records.AddRange(reader.GetRecords());

            // Writing the results to the output file.
            using (var writer = new CsvWriter<T>(Configuration, OutputFilePath))
            {
                if (Configuration.HasHeaders)
                    writer.SetHeaders();

                writer.WriteRecords(records);
            }
        }

        #endregion public methods
    }
}