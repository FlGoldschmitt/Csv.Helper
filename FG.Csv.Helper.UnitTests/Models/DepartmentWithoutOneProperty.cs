namespace FG.Csv.Helper.UnitTests.Models
{
    /// <summary>
    /// Example of an object with one less property compared to the csv file.
    /// </summary>
    public class DepartmentWithoutOneProperty : CsvModel
    {
        // Attribute without the "Name" property. The file header will take the name of the property.
        [CsvHelper(Index = 0)]
        public string DepartmentCode { get; set; }

        // Attribute without the "Name" property. The file header will take the name of the property.
        [CsvHelper(Index = 1)]
        public string DepartmentName { get; set; }

        [CsvHelper(Index = 2, Name = "Region code")]
        public string RegionCode { get; set; }

        public static DepartmentWithoutOneProperty Create(string depCode, string depName, string regCode)
        {
            return new DepartmentWithoutOneProperty()
            {
                DepartmentCode = depCode,
                DepartmentName = depName,
                RegionCode = regCode
            };
        }
    }
}