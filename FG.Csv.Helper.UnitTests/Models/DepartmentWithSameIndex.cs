namespace FG.Csv.Helper.UnitTests.Models
{
    /// <summary>
    /// Example of an object with one less index attribute.
    /// </summary>
    public class DepartmentWithSameIndex : CsvModel
    {
        // Attribute without the "Name" property. The file header will take the name of the property.
        [CsvHelper(Index = 0)]
        public string DepartmentCode { get; set; }

        // Attribute without the "Name" property. The file header will take the name of the property.
        [CsvHelper(Index = 1)]
        public string DepartmentName { get; set; }

        [CsvHelper(Index = 1, Name = "Region code")]
        public string RegionCode { get; set; }

        [CsvHelper(Index = 3, Name = "Region name")]
        public string RegionName { get; set; }

        public static DepartmentWithSameIndex Create(string depCode, string depName, string regCode, string regName)
        {
            return new DepartmentWithSameIndex()
            {
                DepartmentCode = depCode,
                DepartmentName = depName,
                RegionCode = regCode,
                RegionName = regName
            };
        }
    }
}