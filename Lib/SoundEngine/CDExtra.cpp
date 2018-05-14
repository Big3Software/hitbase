/////////////////////////////////////////////////////////////////////////////
// CCDExtra Klasse

#include "stdafx.h"
#include "cdextra.h"

CCDExtra::CCDExtra()
{
}


BOOL CCDExtra::ReadInformation(const CString& drive)
{
   CString filename;
   char lc[3];
   unsigned char Dummy[2];
   CFile fp;
   char InfoPacketID, InfoPacketDataLength;
   UINT nBytesRead;
   char InfoPacketData[256];
   unsigned short NumberOfInfoPackets;
   short CurrentTrack;
   int i;

   ZeroMemory(&lc, sizeof(lc));
   m_bSampler = FALSE;

//   filename.Format("c:\\info.cdp");    //!!!!!!!!!!!!!!!!!!!!!! TEST
   filename.Format(L"%s\\cdplus\\info.cdp", drive);
   if (!fp.Open(filename, CFile::modeRead))
      return FALSE;
   fp.Seek(48, CFile::begin);
   fp.Read(&lc, 2);
   fp.Close();

//   filename.Format("c:\\sub_info.%s", lc);      //!!!!!!!!!!!!!!!!!!!!!!!!!!! TEST
   filename.Format(L"%s\\cdplus\\sub_info.%s", drive, lc);
   if (!fp.Open(filename, CFile::modeRead))
      return FALSE;
   fp.Seek(44, CFile::begin);
   fp.Read(&Dummy, 2);
   NumberOfInfoPackets = (short)Dummy[0] * 256 + (short)Dummy[1];
   
   while (NumberOfInfoPackets--)
   {
      nBytesRead = fp.Read(&InfoPacketID, 1);
      nBytesRead = fp.Read(&InfoPacketDataLength, 1);
      ZeroMemory(&InfoPacketData, sizeof(InfoPacketData));
      nBytesRead = fp.Read(&InfoPacketData, InfoPacketDataLength);

	  // Auf gerade Bytes auffüllen
      fp.Seek(((fp.Seek(0, CFile::current) - 1) / 2 + 1) * 2, CFile::begin);

      switch (InfoPacketID)
      {
      case 0x01:              // Track identifier
         CurrentTrack = atoi(InfoPacketData);
         break;
      case 0x02:              // Album-Titel
         m_AlbumTitel.SetAtGrow(0, CString(InfoPacketData));
         break;
      case 0x03:
         m_UPC.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x04:
         m_ISBN.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x05:
         m_Copyright.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x06:              // Track-Titel
         if (CurrentTrack > 0)
            m_TrackTitel.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x07:
         m_Notes.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x08:              // Hauptinterpret
         m_HauptInterpret.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x09:
         m_ZweiterInterpret.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x0A:
         m_Komponist.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x0B:
         m_KomponistOriginal.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x0C:
         m_Schaffensdatum.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x0D:
         m_Veroeffentlichungsdatum.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x0E:
         m_Herausgeber.SetAtGrow(CurrentTrack, CString(InfoPacketData));
         break;
      case 0x30:
         break;
      case 0x41:
         m_Kategorie.SetAtGrow(0, InfoPacketData[0]);
         break;
      case 0x42:
         m_Bpm.SetAtGrow(CurrentTrack, InfoPacketData[0]);
         break;
      case 0x43:
         m_Tonart.SetAtGrow(CurrentTrack, InfoPacketData[0]);
         break;
// JUS 990321: Diese Meldung jetzt nur noch im Debug-Mode bringen!!
#ifdef DEBUG
      default:
         ULONGLONG offset = fp.Seek(0, CFile::current);
         ErrorBox(L"Unknown InfoPacketID 0x%02x found in %s, offset: %I64d", (int)InfoPacketID, filename, offset);
#endif
      }
   }
   
   // Wenn alle Interpreten gleich sind, dann normale CD, sonst Sampler
   if (m_HauptInterpret.GetCount() > 1)
   {
	   for (i=1;i<=CurrentTrack;i++)
	   {
		   if (m_HauptInterpret[1] != m_HauptInterpret[i])
		   {
			   m_bSampler = TRUE;
			   break;
		   }
	   }

	   if (!m_bSampler)
		   m_HauptInterpret[0] = m_HauptInterpret[1];
   }

   fp.Close();

   return TRUE;
}

