//
//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2003 Idael Cardoso. 
//

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Big3.Hitbase.SoundEngine.CDText
{
  public class DataReadEventArgs : EventArgs
  {
    private byte[] m_Data;
    private uint m_DataSize;
    public DataReadEventArgs(byte[] data, uint size)
    {
      m_Data = data;
      m_DataSize = size;
    }
    public byte[] Data
    {
      get 
      {
        return m_Data;
      }
    }
    public uint DataSize
    {
      get
      {
        return m_DataSize;
      }
    }
  }

  public class ReadProgressEventArgs : EventArgs
  {
    private uint m_Bytes2Read;
    private uint m_BytesRead;
    private bool m_CancelRead = false;
    public ReadProgressEventArgs(uint bytes2read, uint bytesread)
    {
      m_Bytes2Read = bytes2read;
      m_BytesRead = bytesread;
    }
    public uint Bytes2Read
    {
      get
      {
        return m_Bytes2Read;
      }
    }
    public uint BytesRead
    {
      get
      {
        return m_BytesRead;
      }
    }
    public bool CancelRead
    {
      get
      {
        return m_CancelRead;
      }
      set
      {
        m_CancelRead = value;
      }
    }
  }

  internal enum DeviceChangeEventType {DeviceInserted, DeviceRemoved};
  internal class DeviceChangeEventArgs : EventArgs
  {
    private DeviceChangeEventType m_Type;
    private char m_Drive;
    public DeviceChangeEventArgs(char drive, DeviceChangeEventType type)
    {
      m_Drive = drive;
      m_Type = type;
    }
    public char Drive
    {
      get
      {
        return m_Drive;
      }
    }
    public DeviceChangeEventType ChangeType
    {
      get
      {
        return m_Type;
      }
    }
  }
  public delegate void CdDataReadEventHandler(object sender, DataReadEventArgs ea);
  public delegate void CdReadProgressEventHandler(object sender, ReadProgressEventArgs ea);
  internal delegate void DeviceChangeEventHandler(object sender, DeviceChangeEventArgs ea);

  internal enum DeviceType : uint 
  { 
    DBT_DEVTYP_OEM = 0x00000000,      // oem-defined device type
    DBT_DEVTYP_DEVNODE = 0x00000001,  // devnode number
    DBT_DEVTYP_VOLUME = 0x00000002,   // logical volume
    DBT_DEVTYP_PORT = 0x00000003,     // serial, parallel
    DBT_DEVTYP_NET = 0x00000004       // network resource
  }

  internal enum VolumeChangeFlags : ushort
  {
    DBTF_MEDIA = 0x0001,          // media comings and goings
    DBTF_NET   = 0x0002           // network volume
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct DEV_BROADCAST_HDR 
  {
    public uint dbch_size; 
    public DeviceType dbch_devicetype; 
    uint dbch_reserved; 
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct DEV_BROADCAST_VOLUME 
  {
    public uint dbcv_size; 
    public DeviceType dbcv_devicetype; 
    uint dbcv_reserved; 
    uint dbcv_unitmask; 
    public char[] Drives
    {
      get
      {
        string drvs = "";
        for (char c = 'A'; c <= 'Z'; c++)
        {
          if ( (dbcv_unitmask & (1 << (c - 'A'))) != 0 )
          {
            drvs += c;
          }
        }
        return drvs.ToCharArray();
      }
    }
    public VolumeChangeFlags dbcv_flags; 
  }
  


}