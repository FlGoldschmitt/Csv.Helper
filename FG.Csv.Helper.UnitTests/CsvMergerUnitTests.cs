using System;
using System.IO;
using System.Collections.Generic;

using FG.Csv.Helper.UnitTests.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG.Csv.Helper.UnitTests
{
    [TestClass]
    public class CsvMergerUnitTests
    {
        #region CsvMerger class

        [TestMethod]
        public void MergeRecords_ShouldCreateFileWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };
            var inputFilesPath = new List<string>() { 
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv");

            using (var csv = new CsvMerger(configuration, inputFilesPath, outputFilePath))
                csv.MergeRecords();

            Assert.AreEqual(102, File.ReadAllLines(outputFilePath).Length);
        }

        [TestMethod]
        public void MergeRecords_ShouldCreateFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample3_WithoutHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample4_WithoutHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithoutHeaders.csv");

            using (var csv = new CsvMerger(configuration, inputFilesPath, outputFilePath))
                csv.MergeRecords();

            Assert.AreEqual(101, File.ReadAllLines(outputFilePath).Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MergeRecordsWithNotGoodExtension_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.xlsx"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv");

            new CsvMerger(new CsvConfiguration(), inputFilesPath, outputFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MergeRecordsWithEmptyFilePath_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                " ",
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv");

            new CsvMerger(new CsvConfiguration(), inputFilesPath, outputFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MergeRecordsWithNullFilePath_ShouldThrownException()
        {
            new CsvMerger(new CsvConfiguration(), null, PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv"));
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MergeRecordsWithNotExistsFilePath_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("NotExistsFile.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv");

            new CsvMerger(new CsvConfiguration(), inputFilesPath, outputFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void MergeRecordsWithNotExistsDirectory_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = Path.Combine(PathManager.GetNotExistsFolderPath(), "[Merger]ResultWithHeaders.csv");

            new CsvMerger(new CsvConfiguration(), inputFilesPath, outputFilePath);
        }

        #endregion

        #region CsvMerger<T> class

        [TestMethod]
        public void GenericMergeRecords_ShouldCreateFileWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

            using (var csv = new CsvMerger<Department>(configuration, inputFilesPath, outputFilePath))
                csv.MergeRecords();

            Assert.AreEqual(102, File.ReadAllLines(outputFilePath).Length);
        }

        [TestMethod]
        public void GenericMergeRecords_ShouldCreateFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample3_WithoutHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample4_WithoutHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithoutHeaders.csv");

            using (var csv = new CsvMerger<Department>(configuration, inputFilesPath, outputFilePath))
                csv.MergeRecords();

            Assert.AreEqual(101, File.ReadAllLines(outputFilePath).Length);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericMergeRecordsWithoutGoodSeparator_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = '?' };
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

            using (var csv = new CsvMerger<Department>(configuration, inputFilesPath, outputFilePath))
                csv.MergeRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericMergeRecordsWithoutOnePropertyInModel_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

            using (var csv = new CsvMerger<DepartmentWithoutOneProperty>(new CsvConfiguration(), inputFilesPath, outputFilePath))
                csv.MergeRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericMergeRecordsWithoutOneIndexInModel_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

            using (var csv = new CsvMerger<DepartmentWithoutOneIndex>(new CsvConfiguration(), inputFilesPath, outputFilePath))
                csv.MergeRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericMergeRecordsWithSameIndexInModel_ShouldThrownException()
        {
            var inputFilesPath = new List<string>() {
                PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
                PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
            };
            var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

            using (var csv = new CsvMerger<DepartmentWithSameIndex>(new CsvConfiguration(), inputFilesPath, outputFilePath))
                csv.MergeRecords();
        }

        #endregion
    }
}