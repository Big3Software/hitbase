using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Big3.Hitbase.Miscellaneous
{
    public class GetDLLInfo
    {
        public enum MachineType : ushort
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
            IMAGE_FILE_MACHINE_AM33 = 0x1d3,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664,
            IMAGE_FILE_MACHINE_ARM = 0x1c0,
            IMAGE_FILE_MACHINE_EBC = 0xebc,
            IMAGE_FILE_MACHINE_I386 = 0x14c,
            IMAGE_FILE_MACHINE_IA64 = 0x200,
            IMAGE_FILE_MACHINE_M32R = 0x9041,
            IMAGE_FILE_MACHINE_MIPS16 = 0x266,
            IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
            IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
            IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
            IMAGE_FILE_MACHINE_R4000 = 0x166,
            IMAGE_FILE_MACHINE_SH3 = 0x1a2,
            IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
            IMAGE_FILE_MACHINE_SH4 = 0x1a6,
            IMAGE_FILE_MACHINE_SH5 = 0x1a8,
            IMAGE_FILE_MACHINE_THUMB = 0x1c2,
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169,
        }
        private static MachineType GetDllMachineType(string dllPath)
        {
            //offset to PE header is always at 0x3C
            //PE header starts with "PE\0\0" =  0x50 0x45 0x00 0x00
            //followed by 2-byte machine type field (see document above for enum)

            MachineType machineType = MachineType.IMAGE_FILE_MACHINE_UNKNOWN;

            try
            {
                using (FileStream fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(0x3c, SeekOrigin.Begin);
                        Int32 peOffset = br.ReadInt32();
                        fs.Seek(peOffset, SeekOrigin.Begin);
                        UInt32 peHead = br.ReadUInt32();
                        if (peHead != 0x00004550) // "PE\0\0", little-endian
                            throw new Exception("Can't find PE header");
                        machineType = (MachineType)br.ReadUInt16();
                    }
                }
            }
            catch (Exception Ex)
            {
                // Todo???
            }

            return machineType;
        }

        /// <summary>
        /// Rückgabe Prozessorarchitektur - zur Zeit nur 64, 32 oder 0 für unbekannt
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static int GetUnmanagedDllType(string dllPath)
        {
            switch (GetDllMachineType(dllPath))
            {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    return 64;
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    return 32;
                default:
                    return 0;
            }
        }
    }
}
