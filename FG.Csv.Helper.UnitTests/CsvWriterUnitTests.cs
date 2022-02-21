using System;
using System.IO;
using System.Collections.Generic;

using FG.Csv.Helper.UnitTests.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG.Csv.Helper.UnitTests
{
    [TestClass]
    public class CsvWriterUnitTests
    {
        IEnumerable<string> _records = new List<string>()
        {
            "01;Ain;84;Auvergne-Rhône-Alpes",
            "02;Aisne;32;Hauts-de-France",
            "03;Allier;84;Auvergne-Rhône-Alpes",
            "04;Alpes-de-Haute-Provence;93;Provence-Alpes-Côte d'Azur",
            "05;Hautes-Alpes;93;Provence-Alpes-Côte d'Azur",
            "06;Alpes-Maritimes;93;Provence-Alpes-Côte d'Azur",
            "07;Ardèche;84;Auvergne-Rhône-Alpes",
            "08;Ardennes;44;Grand Est",
            "09;Ariège;76;Occitanie",
            "10;Aube;44;Grand Est"
        };

        IEnumerable<Department> _departments = new List<Department>()
        {
            Department.Create("01","Ain","84","Auvergne-Rhône-Alpes"),
            Department.Create("02","Aisne","32","Hauts-de-France"),
            Department.Create("03","Allier","84","Auvergne-Rhône-Alpes"),
            Department.Create("04","Alpes-de-Haute-Provence","93","Provence-Alpes-Côte d'Azur"),
            Department.Create("05","Hautes-Alpes","93","Provence-Alpes-Côte d'Azur"),
            Department.Create("06","Alpes-Maritimes","93","Provence-Alpes-Côte d'Azur"),
            Department.Create("07","Ardèche","84","Auvergne-Rhône-Alpes"),
            Department.Create("08","Ardennes","44","Grand Est"),
            Department.Create("09","Ariège","76","Occitanie"),
            Department.Create("10","Aube","44","Grand Est")
        };

        #region CsvWriter class

        [TestMethod]
        public void WriteRecords_ShouldCreateFileWithHeaders()
        {
            var configuration = new CsvConfiguration();
            string filePath = PathManager.GetResultFilePath("[Writer]ResultWithHeaders.csv");

            using (var csv = new CsvWriter(configuration, filePath))
            {
                csv.SetHeaders("Department code;Department name;Region code;Region name");
                csv.WriteRecords(_records);
            }

            // Check the creation of the file and its content
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(11, File.ReadAllLines(filePath).Length);
        }

        [TestMethod]
        public void WriteRecords_ShouldCreateFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration();
            string filePath = PathManager.GetResultFilePath("[Writer]ResultWithoutHeaders.csv");

            using (var csv = new CsvWriter(configuration, filePath))
                csv.WriteRecords(_records);

            // Check the creation of the file and its content
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(10, File.ReadAllLines(filePath).Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteRecordsWithNotGoodExtension_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvWriter(new CsvConfiguration(), PathManager.GetResultFilePath("NotGoodExtension.xslx"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteRecordsWithEmptyFilePath_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvWriter(new CsvConfiguration(), string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteRecordsWithNullFilePath_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvWriter(new CsvConfiguration(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void WriteRecordsWithNotExistsDirectory_ShouldThrownException()
        {
            // An invalid object is not allowed to be created.
            new CsvWriter(new CsvConfiguration(), Path.Combine(PathManager.GetNotExistsFolderPath(), "[Writer]ResultWithoutHeaders.csv"));
        }

        #endregion

        #region CsvWriter<T> class

        [TestMethod]
        public void GenericWriteRecords_ShouldCreateFileWithHeaders()
        {
            var configuration = new CsvConfiguration();
            string filePath = PathManager.GetResultFilePath("[Writer]GenericResultWithHeaders.csv");

            using (var csv = new CsvWriter<Department>(configuration, filePath))
            {
                csv.SetHeaders();
                csv.WriteRecords(_departments);
            }

            // Check the creation of the file and its content
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(11, File.ReadAllLines(filePath).Length);
        }

        [TestMethod]
        public void GenericWriteRecords_ShouldCreateFileWithoutHeaders()
        {
            var configuration = new CsvConfiguration();
            string filePath = PathManager.GetResultFilePath("[Writer]GenericResultWithoutHeaders.csv");

            using (var csv = new CsvWriter<Department>(configuration, filePath))
                csv.WriteRecords(_departments);

            // Check the creation of the file and its content
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(10, File.ReadAllLines(filePath).Length);
        }

        #endregion
    }
}