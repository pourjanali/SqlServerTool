using System;

namespace SqlServerTool // This namespace should match your project
{
    /// <summary>
    /// Represents a database with its name and file paths.
    /// </summary>
    public class DatabaseInfo
    {
        public string Name { get; set; } = "N/A";
        public string MdfPath { get; set; } = "N/A";
        public string LdfPath { get; set; } = "N/A";

        // This allows the object's name to be displayed correctly in the ListBox.
        public override string ToString()
        {
            return this.Name;
        }
    }
}
