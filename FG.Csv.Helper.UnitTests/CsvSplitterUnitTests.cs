using System;
using System.IO;
using System.Linq;

using FG.Csv.Helper.UnitTests.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG.Csv.Helper.UnitTests
{
    [TestClass]
    public class CsvSplitterUnitTests
    {
        #region CsvSplitter class

        [TestMethod]
        public void SplitRecords_ShouldCreateFilesWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter(configuration, inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]ResultWithHeaders");

            Assert.AreEqual(10, Directory.EnumerateFiles(PathManager.GetResultsFolderPath(), "[Splitter]ResultWithHeaders_*.csv").Count());
        }

        [TestMethod]
        public void SplitRecords_ShouldCreateFilesWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample8_WithoutHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter(configuration, inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]ResultWithoutHeaders");

            Assert.AreEqual(10, Directory.EnumerateFiles(PathManager.GetResultsFolderPath(), "[Splitter]ResultWithoutHeaders_*.csv").Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SplitRecordsWithNotGoodExtension_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.xlsx");
            new CsvSplitter(new CsvConfiguration(), inputFilePath, PathManager.GetResultsFolderPath());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SplitRecordsWithEmptyFilePath_ShouldThrownException()
        {
            new CsvSplitter(new CsvConfiguration(), " ", PathManager.GetResultsFolderPath());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SplitRecordsWithNullFilePath_ShouldThrownException()
        {
            new CsvSplitter(new CsvConfiguration(), null, PathManager.GetResultsFolderPath());
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SplitRecordsWithNotExistsFilePath_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("NotExistsFile.csv");
            new CsvSplitter(new CsvConfiguration(), inputFilePath, PathManager.GetResultsFolderPath());
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void SplitRecordsWithNotExistsDirectory_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample8_WithoutHeaders.csv");
            new CsvSplitter(new CsvConfiguration(), inputFilePath, PathManager.GetNotExistsFolderPath());
        }

        #endregion

        #region CsvSplitter<T> class

        [TestMethod]
        public void GenericSplitRecords_ShouldCreateFilesWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<Department>(configuration, inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");

            Assert.AreEqual(10, Directory.EnumerateFiles(PathManager.GetResultsFolderPath(), "[Splitter]GenericResultWithHeaders_*.csv").Count());
        }

        [TestMethod]
        public void GenericSplitRecords_ShouldCreateFilesWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample8_WithoutHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<Department>(configuration, inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithoutHeaders");

            Assert.AreEqual(10, Directory.EnumerateFiles(PathManager.GetResultsFolderPath(), "[Splitter]GenericResultWithoutHeaders_*.csv").Count());
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericSplitRecordsWithoutGoodSeparator_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = '?' };
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<Department>(configuration, inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericSplitRecordsWithoutOnePropertyInModel_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<DepartmentWithoutOneProperty>(new CsvConfiguration(), inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericSplitRecordsWithoutOneIndexInModel_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<DepartmentWithoutOneIndex>(new CsvConfiguration(), inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericSplitRecordsWithSameIndexInModel_ShouldThrownException()
        {
            var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
            var outputFolderPath = PathManager.GetResultsFolderPath();

            using (var csv = new CsvSplitter<DepartmentWithSameIndex>(new CsvConfiguration(), inputFilePath, outputFolderPath))
                csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");
        }

        #endregion
    }
}