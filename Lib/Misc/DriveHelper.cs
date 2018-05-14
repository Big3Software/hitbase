using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Diagnostics;

namespace Big3.Hitbase.Miscellaneous
{
    public class DriveHelper
    {
        public class DiskDrive
        {
            public string MediaType;
            public string DriveLetter;
            public string InterfaceType;
            public string VolumeName;
            public ulong Size;
            public ulong Freespace;
        }

        public static List<DiskDrive> GetAvailableDisks()
        {
            List<DiskDrive> DiskDrives = new List<DiskDrive>();

            // browse all USB WMI physical disks
            foreach (ManagementObject drive in
             new ManagementObjectSearcher(
              "select DeviceID, MediaType,InterfaceType from Win32_DiskDrive").Get())
            {
                // associate physical disks with partitions
                ManagementObjectCollection partitionCollection = new ManagementObjectSearcher(String.Format(
                 "associators of {{Win32_DiskDrive.DeviceID='{0}'}} " +
                   "where AssocClass = Win32_DiskDriveToDiskPartition",
                 drive["DeviceID"])).Get();

                foreach (ManagementObject partition in partitionCollection)
                {
                    if (partition != null)
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        ManagementObjectCollection logicalCollection = new ManagementObjectSearcher(String.Format(
                         "associators of {{Win32_DiskPartition.DeviceID='{0}'}} " +
                          "where AssocClass= Win32_LogicalDiskToPartition",
                         partition["DeviceID"])).Get();

                        foreach (ManagementObject logical in logicalCollection)
                        {
                            if (logical != null)
                            {
                                // finally find the logical disk entry
                                ManagementObjectCollection.ManagementObjectEnumerator volumeEnumerator = new ManagementObjectSearcher(String.Format(
                                 "select * from Win32_LogicalDisk " +
                                  "where Name='{0}'",
                                 logical["Name"])).Get().GetEnumerator();

                                volumeEnumerator.MoveNext();

                                ManagementObject volume = (ManagementObject)volumeEnumerator.Current;

                                DiskDrive disk = new DiskDrive();

                                disk.MediaType = drive["MediaType"].ToString();
                                disk.DriveLetter = volume["DeviceID"].ToString();
                                disk.Freespace = (ulong)volume["Freespace"];
                                disk.Size = (ulong)volume["Size"];
                                disk.InterfaceType = drive["InterfaceType"].ToString();
                                disk.VolumeName = volume["VolumeName"].ToString();
                                DiskDrives.Add(disk);

                            }
                        }
                    }
                }
            }

            return DiskDrives;
        }
    }
}



