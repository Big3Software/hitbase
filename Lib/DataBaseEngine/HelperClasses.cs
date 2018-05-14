using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;

namespace Big3.Hitbase.DataBaseEngine
{
    public static class SqlCeUpgrade
    {
        public static void EnsureVersion40(this SqlCeEngine engine, string filename)
        {
            SQLCEVersion fileversion = DetermineVersion(filename);
            if (fileversion == SQLCEVersion.SQLCE20)
                throw new ApplicationException("Unable to upgrade from 2.0 to 4.0");
            if (SQLCEVersion.SQLCE40 > fileversion)
            {
                // Original-Datei sichern
                string filenameBackup = GetBackupFilename(filename);
                File.Copy(filename, filenameBackup);
                engine.Upgrade();//string.Format("Data Source=\"{0}\"", filename+".new"));
            }
        }

        public static string GetBackupFilename(string filename)
        {
            string filenameBackup = filename + ".bak";
            int count = 2;

            while (File.Exists(filenameBackup))
            {
                filenameBackup = filename + ".bak" + "." + count.ToString();
                count++;
            }

            return filenameBackup;
        }

        public enum SQLCEVersion
        {
            SQLCE20 = 0,
            SQLCE30 = 1,
            SQLCE35 = 2,
            SQLCE40 = 3
        }
        
        public static SQLCEVersion DetermineVersion(string filename)
        {
            var versionDictionary = new Dictionary<int, SQLCEVersion> 
            { 
                { 0x73616261, SQLCEVersion.SQLCE20 }, 
                { 0x002dd714, SQLCEVersion.SQLCE30},
                { 0x00357b9d, SQLCEVersion.SQLCE35},
                { 0x003d0900, SQLCEVersion.SQLCE40}

            };
            int versionLONGWORD = 0;
            
            try
            {
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Seek(16, SeekOrigin.Begin);
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        versionLONGWORD = reader.ReadInt32();
                    }
                }
            }
            catch
            {
                throw;
            }

            if (versionDictionary.ContainsKey(versionLONGWORD))
            {
                return versionDictionary[versionLONGWORD];
            }
            else
            {
                throw new ApplicationException("Unable to determine database file version: " + filename);
            }
        }
    }
}
