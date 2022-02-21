# C# Utility library for csv files

This library allows you to read, write, split or merge your csv files.  
It's a basic implementation that allows these different actions to be carried out quickly and easily.  

## How to install the NuGet package

### Package Manager

```
    Install-Package FG.Csv.Helper -Version 1.0.0
```
### .NET CLI

```
    dotnet add package FG.Csv.Helper --version 1.0.0
```
### PackageReference

```
    <PackageReference Include="FG.Csv.Helper" Version="1.0.0" />
```

## How to use

### Configuration

Each functionality (read, write, split and merge) has its own class (CsvReader, CsvWriter, CsvMerger and CsvSplitter).  
These classes require an instance of CsvConfiguration as a constructor parameter.  

*CsvConfiguration:* 
```c#
    /// <summary>
    /// Contains the settings for the Csv Helper tool.
    /// </summary>
    public class CsvConfiguration
    {
        /// <summary>
        /// Separator of the csv file (default: ;).
        /// </summary>
        public char Separator { get; set; } = ';';

        /// <summary>
        /// Number of threads used in parallel when reading/writing (default: 1).
        /// </summary>
        public int DegreeOfParallelism { get; set; } = 1;

        /// <summary>
        /// Presence or not of a file header (default: false). Not used for the writer (use SetHeaders method).
        /// </summary>
        public bool HasHeaders { get; set; } = false;

        /// <summary>
        /// Encoding type of the csv file (default: Encoding.Default).
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;
    }
```
### Using a csv model

The object model can be used for all the library's functionalities. It will allow data to be manipulated in object form and not in raw form.

Your object model must respect a few rules:
- Inherit the CsvModel class;  
- Each property must define the CsvHelper attribute, and specify a value for the 'Index' property. This determines the position of this property in the csv file. The second property 'Name' of this attribute is optional. It will be used in writing the headers of the csv file, otherwise the name of the property will be used.

*Example of a model inheriting from CsvModel:*
```c#
  public class Department : CsvModel
  {
      // Attribute without the "Name" property. The file header will take the name of the property.
      [CsvHelper(Index = 0)]
      public string DepartmentCode { get; set; }

      // Attribute without the "Name" property. The file header will take the name of the property.
      [CsvHelper(Index = 1)]
      public string DepartmentName { get; set; }

      [CsvHelper(Index = 2, Name = "Region code")]
      public string RegionCode { get; set; }

      [CsvHelper(Index = 3, Name = "Region name")]
      public string RegionName { get; set; }
  }
```

### Read a csv file  

Two classes may be used to read a csv file:  

- The CsvReader class allows to get the content of the csv file as a string array where each item represents a line.  

```c#
  // Example: Reading a file containing headers and the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  var results = Array.Empty<string>();

  using (var csv = new CsvReader(configuration, PathManager.GetSampleFilePath("[Reader]Sample5_WithHeaders.csv")))
      results = csv.GetRecords();
```

- The CsvReader&lt;T&gt; class is used to get the content of the csv file as an IEnumerable where each item is an instance of the given generic type argument.  
  
```c#
  // Example: Reading a file containing headers and the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  IEnumerable<Department> results;
  
  using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample5_WithHeaders.csv")))
      results = csv.GetRecords();
```

The two CsvReader classes also allow to execute an action for each line of the csv file (or for a defined list) in parallel.

```c#
  // Example: Execution of an action on each line of the csv file with 5 threads in parallel.
  var configuration = new CsvConfiguration() { DegreeOfParallelism = 5 };
  var records = new ConcurrentStack<Department>();
  
  using (var csv = new CsvReader<Department>(configuration, PathManager.GetSampleFilePath("[Reader]Sample6_WithoutHeaders.csv")))
  {
      csv.PerformActionAsParallel(department =>
      {
          // REPLACE THIS LINE WITH YOUR CODE.
          // In our case, we do the same action as the read functionality. 
          // Each result is added to a ConcurrentStack (as several threads can add a value at the same time).
          records.Push(department);
      });
  }
```

### Write a csv file  
  
Two classes may be used to write a csv file:  

- The CsvWriter class allows you to write the csv file line by line from an IEnumerable&lt;string&gt;.  

```c#
  // Example: Writing a file with the separator ','.
  var configuration = new CsvConfiguration() { Separator = ',' };
  IEnumerable<string> records = new List<string>() { [CONTENT] };
  using (var csv = new CsvWriter(configuration, PathManager.GetResultFilePath("[Writer]ResultWithHeaders.csv")))
  {
      // Add headers 
      csv.SetHeaders("Department code,Department name,Region code,Region name");
      csv.WriteRecords(records);
  }
```

- The CsvWriter&lt;T&gt; class allows to write a csv file using a generic type parameter inheriting from CsvModel.  
  It writes all the lines of the file from a list where each element is an instance of the given generic type.  

```c#
// Example: Writing a file with the separator ','.
var configuration = new CsvConfiguration() { Separator = ',' };
IEnumerable<Department> departments = new List<Department>() { [CONTENT] };
using (var csv = new CsvWriter<Department>(configuration, PathManager.GetResultFilePath("[Writer]GenericResultWithHeaders.csv")))
{
    // Add headers based on the properties of the model
    csv.SetHeaders();
    csv.WriteRecords(departments);
}
```

### Split a csv file

Two classes may be used to split a csv file:  

- The CsvSplitter class is used to split the file.  
  If the "HasHeaders" property is set to "true", the headers will be retrieved from the original file and copied to each split file.  
  The SplitRecords method takes as first parameter the number of output files, and the name of these files (with "_[number]" suffix).

```c#
  // Example: Splitting a file with the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
  var outputFolderPath = PathManager.GetResultsFolderPath();

  using (var csv = new CsvSplitter(configuration, inputFilePath, outputFolderPath))
      csv.SplitRecords(10, "[Splitter]ResultWithHeaders");
```

- The CsvSplitter&lt;T&gt; class is used to split the file using a generic type parameter inheriting from CsvModel, which will allow, among other things, retrieve the headers defined in the associated model.  

```c#
  // Example: Splitting a file with the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  var inputFilePath = PathManager.GetSampleFilePath("[Splitter]Sample7_WithHeaders.csv");
  var outputFolderPath = PathManager.GetResultsFolderPath();

  using (var csv = new CsvSplitter<Department>(configuration, inputFilePath, outputFolderPath))
      csv.SplitRecords(10, "[Splitter]GenericResultWithHeaders");
```

### Merge a csv files

Two classes may be used to merge a csv files:  

- The CsvMerger class is used to merge several files into one.  

```c#
  // Example: Merging files with the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  var inputFilesPath = new List<string>() { 
      PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
      PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
  };
  var outputFilePath = PathManager.GetResultFilePath("[Merger]ResultWithHeaders.csv");

  using (var csv = new CsvMerger(configuration, inputFilesPath, outputFilePath))
    csv.MergeRecords();
```

- The CsvMerger&lt;T&gt; class is used to merge the files using a generic type parameter inheriting from CsvModel, which will allow, among other things, retrieve the headers defined in the associated model.    

```c#
  // Example: Merging files with the separator ';'.
  var configuration = new CsvConfiguration() { HasHeaders = true, Separator = ';' };
  var inputFilesPath = new List<string>() { 
      PathManager.GetSampleFilePath("[Merger]Sample1_WithHeaders.csv"),
      PathManager.GetSampleFilePath("[Merger]Sample2_WithHeaders.csv")
  };
  var outputFilePath = PathManager.GetResultFilePath("[Merger]GenericResultWithHeaders.csv");

  using (var csv = new CsvMerger<Department>(configuration, inputFilesPath, outputFilePath))
    csv.MergeRecords();
```
