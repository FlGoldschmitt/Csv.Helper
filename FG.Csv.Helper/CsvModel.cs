using System;
using System.Linq;
using System.Reflection;

namespace FG.Csv.Helper
{
    /// <summary>
    /// Abstract model from which the model classes must derive to use the library's generic functions.
    /// </summary>
    public abstract class CsvModel
    {
        /// <summary>
        /// Fills the object from the row values of the csv file.
        /// </summary>
        /// <param name="values">List of values to be filled in the object.</param>
        /// <returns>Returns the object with its properties filled in.</returns>
        /// <exception cref="CsvException"></exception>
        public CsvModel Fill(string[] values)
        {
            var properties = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(CsvHelperAttribute)));

            if (properties.Count() != values.Length)
                throw CsvException.InvalidParameters();

            for (int i = 0; i < values.Length; i++)
            {
                PropertyInfo propInfo = null;

                try
                {
                    propInfo = properties.SingleOrDefault(prop => (
                        (CsvHelperAttribute)prop.GetCustomAttributes(typeof(CsvHelperAttribute), true).First()).Index == i
                    );
                }
                catch(InvalidOperationException)
                {
                    throw CsvException.InvalidIndex(i, GetType().Name);
                }
                
                if (propInfo == null)
                    throw CsvException.PropertyNotFound(i, GetType().Name);

                propInfo.SetValue(this, values[i]);
            }

            return this;
        }

        /// <summary>
        /// Retrieves the list of model properties in the order of the index values of the CsvHelperAttribute.
        /// </summary>
        /// <returns>List of properties in the order of the index value of the CsvHelperAttribute.</returns>
        public IOrderedEnumerable<PropertyInfo> GetOrderedProperties()
        {
            return GetType().GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(CsvHelperAttribute)))
                .OrderBy(prop => ((CsvHelperAttribute)prop.GetCustomAttributes(typeof(CsvHelperAttribute), true).First()).Index);
        }
    }
}