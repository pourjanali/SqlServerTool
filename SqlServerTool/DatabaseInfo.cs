using System;
using System.IO;

namespace SqlServerTool
{
    /// <summary>
    /// Represents a database with its name and file paths.
    /// </summary>
    public class DatabaseInfo
    {
        public string Name { get; set; } = "N/A";
        public string MdfPath { get; set; } = "N/A";
        public string LdfPath { get; set; } = "N/A";

        // #32: Updated properties to hold drive letter and full disk model name.
        public string MdfDrive => !string.IsNullOrEmpty(MdfPath) && Path.IsPathRooted(MdfPath) ? Path.GetPathRoot(MdfPath) : "";
        public string LdfDrive => !string.IsNullOrEmpty(LdfPath) && Path.IsPathRooted(LdfPath) ? Path.GetPathRoot(LdfPath) : "";
        public string MdfDriveModel { get; set; } = "";
        public string LdfDriveModel { get; set; } = "";

        // This allows the object's name to be displayed correctly in the ListBox.
        public override string ToString()
        {
            return this.Name;
        }
    }
}