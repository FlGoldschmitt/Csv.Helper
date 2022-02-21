using System;
using System.IO;
using System.Linq;

using FG.Csv.Helper.Extensions;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Allows to split a csv file into several fair files.
    /// </summary>
    public class CsvSplitter : IDisposable
    {
        #region private fields

        private string _inputFilePath;

        private string _outputFolderPath;

        private CsvConfiguration _configuration;

        #endregion private fields

        #region properties

        /// <summary>
        /// Location of the file to be split.
        /// </summary>
        protected string InputFilePath => _inputFilePath;

        /// <summary>
        /// Output folder that will contain the split files.
        /// </summary>
        protected string OutputFolderPath => _outputFolderPath;

        /// <summary>
        /// Csv parameters.
        /// </summary>
        protected CsvConfiguration Configuration => _configuration;

        #endregion properties

        #region constructor

        /// <summary>
        /// Create a CsvSplitter instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="inputFilePath">Location of the file to be split.</param>
        /// <param name="outputFolderPath">Directory that will contain the result of the splitting.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvSplitter(CsvConfiguration configuration, string inputFilePath, string outputFolderPath)
        {
            // An invalid object is not allowed to be created.
            inputFilePath.CheckingFilePath(true);
            outputFolderPath.CheckingFolderPath();

            _configuration = configuration;
            _inputFilePath = inputFilePath;
            _outputFolderPath = outputFolderPath;
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Split the csv file into several files.
        /// </summary>
        /// <param name="nbFiles">Number of output files.</param>
        /// <param name="fileNames">Name of output files (with "_[number]" suffix).</param>
        public void SplitRecords(int nbFiles, string fileNames)
        {
            using (var reader = new CsvReader(_configuration, _inputFilePath))
            {
                var lines = reader.GetRecords();
                int count = (int)Math.Round((double)lines.Count() / nbFiles);

                for (int i = 0; i < nbFiles; i++)
                {
                    using (var writer = new CsvWriter(_configuration, Path.Combine(_outputFolderPath, $"{fileNames}_{i + 1}.csv")))
                    {
                        if (_configuration.HasHeaders)
                            writer.SetHeaders(reader.Headers);

                        int take = (i == nbFiles - 1 ? lines.Count() - (count * i) : count);
                        if (take != 0)
                            writer.WriteRecords(lines.Skip(count * i).Take(take));
                    }
                }
            }
        }

        #endregion constructor

        #region implementation IDisposable

        public void Dispose() { }

        #endregion implementation IDisposable
    }

    /// <summary>
    /// Allows to split a csv file into several fair files using a generic type parameter inheriting from CsvModel.
    /// </summary>
    /// <typeparam name="T">Generic type inheriting from CsvModel.</typeparam>
    public class CsvSplitter<T> : CsvSplitter where T : CsvModel
    {
        #region constructor

        /// <summary>
        /// Create a CsvSplitter instance.
        /// </summary>
        /// <param name="configuration">Csv parameters.</param>
        /// <param name="inputFilePath">Location of the file to be split.</param>
        /// <param name="outputFolderPath">Directory that will contain the result of the splitting.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public CsvSplitter(CsvConfiguration configuration, string inputFilePath, string outputFolderPath)
            : base(configuration, inputFilePath, outputFolderPath)
        { }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Split the csv file into several files (reading and writing will be done from the given generic type).
        /// </summary>
        /// <param name="nbFiles">Number of output files.</param>
        /// <param name="fileNames">Name of output files (with "_[number]" suffix).</param>
        /// <exception cref="CsvException"></exception>
        public new void SplitRecords(int nbFiles, string fileNames)
        {
            using (var reader = new CsvReader<T>(Configuration, InputFilePath))
            {
                var lines = reader.GetRecords();
                int count = (int)Math.Round((double)lines.Count() / nbFiles);

                for (int i = 0; i < nbFiles; i++)
                {
                    using (var writer = new CsvWriter<T>(Configuration, Path.Combine(OutputFolderPath, $"{fileNames}_{i + 1}.csv")))
                    {
                        if (Configuration.HasHeaders)
                            writer.SetHeaders();

                        int take = (i == nbFiles - 1 ? lines.Count() - (count * i) : count);
                        if (take != 0)
                            writer.WriteRecords(lines.Skip(count * i).Take(take));
                    }
                }
            }
        }

        #endregion constructor
    }
}