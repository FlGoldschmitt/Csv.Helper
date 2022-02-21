using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using FG.Csv.Helper.UnitTests.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace FG.Csv.Helper.UnitTests
{
    [TestClass]
    public class CsvReaderUnitTests
    {
        #region CsvReader class

        [TestMethod]
        public void GetRecords_ShouldReadFileWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };
            var results = Array.Empty<string>();

            using (var csv = new CsvReader(configuration, PathManager.GetSampleFilePath("[Reader]Sample5_WithHeaders.csv")))
                results = csv.GetRecords();

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(results.Length, 101);
        }

        [TestMethod]
        public void GetRecords_ShouldReadFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };
            var results = Array.Empty<string>();

            using (var csv = new CsvReader(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(results.Length, 101);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRecordsWithNotGoodExtension_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvReader(new CsvConfiguration(), PathManager.GetSampleFilePath("NotGoodExtension.xslx"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRecordsWithEmptyFilePath_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvReader(new CsvConfiguration(), string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetRecordsWithNullFilePath_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvReader(new CsvConfiguration(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetRecordsWithNotExistsFilePath_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvReader(new CsvConfiguration(), PathManager.GetSampleFilePath("NotExistsFile.csv"));
        }

        #endregion

        #region CsvReader<T> class

        [TestMethod]
        public void GenericGetRecords_ShouldReadFileWithHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = true };

            IEnumerable<Department> results;
            using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample5_WithHeaders.csv")))
                results = csv.GetRecords();

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(results.Count(), 101);
        }

        [TestMethod]
        public void GenericGetRecords_ShouldReadFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration() { HasHeaders = false };

            IEnumerable<Department> results;
            using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(results.Count(), 101);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericGetRecordsWithoutGoodSeparator_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = '?' };

            IEnumerable<Department> results;
            using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericGetRecordsWithoutOnePropertyInModel_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = ';' };

            IEnumerable<DepartmentWithoutOneProperty> results;
            using (var csv = new CsvReader<DepartmentWithoutOneProperty>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericGetRecordsWithoutOneIndexInModel_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = ';' };

            IEnumerable<DepartmentWithoutOneIndex> results;
            using (var csv = new CsvReader<DepartmentWithoutOneIndex>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();
        }

        [TestMethod]
        [ExpectedException(typeof(CsvException))]
        public void GenericGetRecordsWithSameIndexInModel_ShouldThrownException()
        {
            var configuration = new CsvConfiguration() { Separator = ';' };

            IEnumerable<DepartmentWithSameIndex> results;
            using (var csv = new CsvReader<DepartmentWithSameIndex>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetRecords();
        }

        [TestMethod]
        public void GenericGetUnorderedRecords_ShouldReadFile()
        {
            // We test with several threads at the same time to check that we get the right number of lines at the end 
            var configuration = new CsvConfiguration() { DegreeOfParallelism = 5 };

            IEnumerable<Department> results = null;
            using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
                results = csv.GetUnorderedRecords();

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(results.Count(), 101);
        }

        [TestMethod]
        public void GenericPerformActionAsParallel_ShouldReadAndProcessFile()
        {
            var configuration = new CsvConfiguration() { DegreeOfParallelism = 5 };

            var records = new ConcurrentStack<Department>();
            using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
            {
                csv.PerformActionAsParallel(department =>
                {
                    records.Push(department);
                });
            }

            // The number of lines in the file without headers is equal to 101
            Assert.AreEqual(records.Count(), 101);
        }

        #endregion
    }
}