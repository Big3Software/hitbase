// Mp3Engine.cpp: implementation of the CMp3Engine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "Mp3Engine.h"
#include "Mp3Player.h"
#include "VirtualCD.h"
#include "../hitmisc/HitbaseWinAppBase.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

using namespace System;
using namespace System::Windows::Forms;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMp3Engine::CMp3Engine()
{
	m_pPlayer = NULL;
	m_iBitsPerSample = 0;
}

CMp3Engine::~CMp3Engine()
{
	if (m_pPlayer)
	{
		delete m_pPlayer;
		m_pPlayer = NULL;
	}
}

CMp3Engine* CMp3Engine::CreateFromFile(const CString &sFilename)
{
	CMp3Engine* pMp3Engine = new CMp3Engine;

	pMp3Engine->m_sFilename = sFilename;

	if (pMp3Engine->Open())
		return pMp3Engine;

	delete pMp3Engine;

	return NULL;
}

BOOL CMp3Engine::Open()
{
	USES_CONVERSION;
	if (CMisc::GetFileNameExtension(m_sFilename).MakeLower() == ".wma" ||
		CMisc::GetFileNameExtension(m_sFilename).MakeLower() == ".asf" ||
		CMisc::GetFileNameExtension(m_sFilename).MakeLower() == ".wav")
	{
		CSoundEngine::GlobalInit();

		m_dwLength = 0;

		FMOD::Sound* stream;
		FMOD_RESULT res = CSoundEngine::m_pFMODSystem->createStream(W2A(m_sFilename), FMOD_OPENONLY, 0, &stream);

		if (res != FMOD_OK)
		{
			return FALSE;
		}

		res = stream->getLength((UINT*)&m_dwLength, FMOD_TIMEUNIT_MS);

		GetTags(stream);

		stream->release();

		return TRUE;
	}

	if (!GetMP3Header())
		return FALSE;

	if (!GetMP3Information())
		return FALSE;

	return TRUE;
}

BOOL CMp3Engine::Close()
{
	if (!m_pPlayer)
		return TRUE;

	BOOL bRet = m_pPlayer->Close();

	delete m_pPlayer;
	m_pPlayer = NULL;

	return bRet;
}

BOOL CMp3Engine::Play(DWORD dwMSStart, DWORD dwMSEnd)
{
	if (!m_pPlayer)
	{
		m_pPlayer = new CMp3Player;

		m_pPlayer->m_pMp3Engine = this;

		if (!m_pPlayer->Open(m_sFilename))
			return FALSE;
	
		SetAutoVolumeAdjust(((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bAutoVolumeAdjust);
	}

	if (IsPlaying())
	{
		Seek(dwMSStart);
		return TRUE;
	}
	else
	{
		if (m_pPlayer->Play(dwMSStart))
		{
			return TRUE;
		}
		else
			return FALSE;
	}
}

BOOL CMp3Engine::Stop()
{
	if (!m_pPlayer)
		return FALSE;

	BOOL bRet = m_pPlayer->Stop();

	return bRet;
}

BOOL CMp3Engine::Pause(BOOL bPause /* = TRUE */)
{
	if (m_pPlayer->Pause(bPause ? true : false))
		return TRUE;
	else
		return FALSE;
}

BOOL CMp3Engine::Seek(DWORD dwPosition)
{ 
	if (!m_pPlayer)
		return FALSE;

	m_pPlayer->Seek(dwPosition);
	
	return TRUE;
}

DWORD CMp3Engine::GetPlayPositionMS()
{
	if (!m_pPlayer)
		return 0;

	if (!IsPlaying())
		return 0;

	return m_pPlayer->GetCurrentPosition();
}

BOOL CMp3Engine::IsPlaying()
{
	if (!m_pPlayer)
		return FALSE;

	return m_pPlayer->IsPlaying();
}

BOOL CMp3Engine::IsPlayCompleted()
{
	if (!m_pPlayer)
		return TRUE;

	return m_pPlayer->m_bPlayCompleted;
}

BOOL CMp3Engine::SetSpeed(double dblSpeed)
{
	if (!m_pPlayer)
		return FALSE;

	return m_pPlayer->SetSpeed(dblSpeed);
}

BOOL CMp3Engine::SetVolume(double dblVolume)
{
	if (!m_pPlayer)
		return FALSE;

	return m_pPlayer->SetVolume(dblVolume);
}

void CMp3Engine::SetAutoVolumeAdjust(BOOL bAutoVolumeAdjust) 
{
	if (!m_pPlayer)
		return ;

	m_pPlayer->SetAutoVolumeAdjust(bAutoVolumeAdjust);

	CSoundEngineItem::SetAutoVolumeAdjust(bAutoVolumeAdjust);
}

// Liest die MP3-Informationen aus der MP3-Datei.
BOOL CMp3Engine::GetMP3Information()
{
	USES_CONVERSION;

	if (CMisc::GetFileNameExtension(m_sFilename).MakeLower() != ".wma" &&
		CMisc::GetFileNameExtension(m_sFilename).MakeLower() != ".asf")
	{
		try
		{
			ID3::ID3Info^ id3Info = gcnew ID3::ID3Info(gcnew System::String(m_sFilename), true);

			int id3Version = Big3::Hitbase::Configuration::Settings::Current->UseID3Version;

			if (id3Info->ID3v1Info->HaveTag)
			{
				m_sArtist = id3Info->ID3v1Info->Artist->Trim();
				m_sTitle = id3Info->ID3v1Info->Title->Trim();
				m_sComment = id3Info->ID3v1Info->Comment->Trim();
				m_sAlbum = id3Info->ID3v1Info->Album->Trim();
				m_sYear = id3Info->ID3v1Info->Year->Trim();
				m_sGenre = MapID3GenreToHitbaseGenre(id3Info->ID3v1Info->Genre);
				m_iTrack = id3Info->ID3v1Info->TrackNumber;
			}

			if (id3Info->ID3v2Info != nullptr && id3Info->ID3v2Info->HaveTag)
			{
				m_sArtist = id3Info->ID3v2Info->GetTextFrame("TPE1")->Trim();
				m_sTitle = id3Info->ID3v2Info->GetTextFrame("TIT2")->Trim();
				m_sComment = id3Info->ID3v2Info->GetTextFrame("COMM")->Trim();
				m_sAlbum = id3Info->ID3v2Info->GetTextFrame("TALB")->Trim();
				m_sYear = id3Info->ID3v2Info->GetTextFrame("TYER")->Trim();
				CString trackNumber = id3Info->ID3v2Info->GetTextFrame("TRCK");
				m_iTrack = _wtoi(trackNumber);
				m_sGenre = id3Info->ID3v2Info->GetTextFrame("TCON")->Trim();
				m_sComposer = id3Info->ID3v2Info->GetTextFrame("TCOM")->Trim();
			}

			// Auf die Feld-Länge verkleinern
			m_sTitle = m_sTitle.Left(100);
			m_sArtist = m_sArtist.Left(100);
			m_sAlbum = m_sComment.Left(100);

	/*		CString str;
			ReadID3Tag(myTag1, myTag2, ID3FID_YEAR, str);
			m_sYear = str;*/
		}
		catch (Exception^ e)
		{
			// Zunächst mal einfach Fehler ignorieren
		}
	}

	return TRUE;
}
 
CString CMp3Engine::MapID3GenreToHitbaseGenre(unsigned char id3Genre)
{
	const wchar_t* genres[] = { 
/* 00 */ L"Blues",
/* 01 */ L"Classic Rock",
/* 02 */ L"Country",
/* 03 */ L"Dance",
/* 04 */ L"Disco",
/* 05 */ L"Funk",
/* 06 */ L"Grunge",
/* 07 */ L"Hip-Hop",
/* 08 */ L"Jazz",
/* 09 */ L"Metal",
/* 10 */ L"New Age",
/* 11 */ L"Oldies",
/* 12 */ L"Other",
/* 13 */ L"Pop",
/* 14 */ L"R&B",
/* 15 */ L"Rap",
/* 16 */ L"Reggae",
/* 17 */ L"Rock",
/* 18 */ L"Techno",
/* 19 */ L"Industrial",
/* 20 */ L"Alternative",
/* 21 */ L"Ska",
/* 22 */ L"Death Metal",
/* 23 */ L"Pranks",
/* 24 */ L"Soundtrack",
/* 25 */ L"Euro-Techno",
/* 26 */ L"Ambient",
/* 27 */ L"Trip-Hop",
/* 28 */ L"Vocal",
/* 29 */ L"Jazz&Funk",
/* 30 */ L"Fusion",
/* 31 */ L"Trance",
/* 32 */ L"Classical",
/* 33 */ L"Instrumental",
/* 34 */ L"Acid",
/* 35 */ L"House",
/* 36 */ L"Game",
/* 37 */ L"Sound Clip",
/* 38 */ L"Gospel",
/* 39 */ L"Noise",
/* 40 */ L"Alternative Rock",
/* 41 */ L"Bass",
/* 42 */ L"Soul",
/* 43 */ L"Punk",
/* 44 */ L"Space",
/* 45 */ L"Meditative",
/* 46 */ L"Instrumental Pop",
/* 47 */ L"Instrumental Rock",
/* 48 */ L"Ethnic",
/* 49 */ L"Gothic",
/* 50 */ L"Darkwave",
/* 51 */ L"Techno-Industrial",
/* 52 */ L"Electronic",
/* 53 */ L"Pop-Folk",
/* 54 */ L"Eurodance",
/* 55 */ L"Dream",
/* 56 */ L"Southern Rock",
/* 57 */ L"Comedy",
/* 58 */ L"Cult",
/* 59 */ L"Gangsta",
/* 60 */ L"Top 40",
/* 61 */ L"Christian Rap",
/* 62 */ L"Pop/Funk",
/* 63 */ L"Jungle",
/* 64 */ L"Native US",
/* 65 */ L"Cabaret",
/* 66 */ L"New Wave",
/* 67 */ L"Psychedelic",
/* 68 */ L"Rave",
/* 69 */ L"Showtunes",
/* 70 */ L"Trailer",
/* 71 */ L"Lo-Fi",
/* 72 */ L"Tribal",
/* 73 */ L"Acid Punk",
/* 74 */ L"Acid Jazz",
/* 75 */ L"Polka",
/* 76 */ L"Retro",
/* 77 */ L"Musical",
/* 78 */ L"Rock & Roll",
/* 79 */ L"Hard Rock",
/* 80 */ L"Folk",
/* 81 */ L"Folk-Rock",
/* 82 */ L"National Folk",
/* 83 */ L"Swing",
/* 84 */ L"Fast Fusion",
/* 85 */ L"Bebop",
/* 86 */ L"Latin",
/* 87 */ L"Revival",
/* 88 */ L"Celtic",
/* 89 */ L"Bluegrass",
/* 90 */ L"Avantgarde",
/* 91 */ L"Gothic Rock",
/* 92 */ L"Progressive Rock",
/* 93 */ L"Psychedelic Rock",
/* 94 */ L"Symphonic Rock",
/* 95 */ L"Slow Rock",
/* 96 */ L"Big Band",
/* 97 */ L"Chorus",
/* 98 */ L"Easy Listening",
/* 99 */ L"Acoustic",
/* 100 */ L"Humour",
/* 101 */ L"Speech",
/* 102 */ L"Chanson",
/* 103 */ L"Opera",
/* 104 */ L"Chamber Music",
/* 105 */ L"Sonata",
/* 106 */ L"Symphony",
/* 107 */ L"Booty Bass",
/* 108 */ L"Primus",
/* 109 */ L"Porn Groove",
/* 110 */ L"Satire",
/* 111 */ L"Slow Jam",
/* 112 */ L"Club",
/* 113 */ L"Tango",
/* 114 */ L"Samba (Musik)",
/* 115 */ L"Folklore",
/* 116 */ L"Ballad",
/* 117 */ L"Power Ballad",
/* 118 */ L"Rhytmic Soul",
/* 119 */ L"Freestyle",
/* 120 */ L"Duet",
/* 121 */ L"Punk Rock",
/* 122 */ L"Drum Solo",
/* 123 */ L"Acapella",
/* 124 */ L"Euro-House",
/* 125 */ L"Dance Hall"
	};

	if (id3Genre > 125)
		return L"";

	return genres[(int)id3Genre];
}

/*void CMp3Engine::ReadID3Tag(ID3_Tag& myTag1, ID3_Tag& myTag2, ID3_FrameID ID3Field, CString& sValue)
{
	ID3_Frame* myFrame = myTag2.Find(ID3Field);
    if (NULL != myFrame)
    {
		char *sText = ID3_GetString(myFrame, ID3FN_TEXT);
		sValue = sText;
//		delete sText;
    }

	if (sValue.IsEmpty())
	{
		ID3_Frame* myFrame = myTag1.Find(ID3Field);
		if (NULL != myFrame)
		{
			char *sText = ID3_GetString(myFrame, ID3FN_TEXT);
			sValue = sText;
//			delete [] sText;
		}
	}
}*/

int s_iaBitrates[16][5] = {
	{ 0,	0,		0,		0,		0	}, 
	{ 32,	32,		32,		32,		8	}, 
	{ 64,	48,		40,		48,		16  },
	{ 96,	56,		48,		56,		24  },
	{ 128,	64,		56,		64,		32  },
	{ 160,	80,		64,		80,		40  },
	{ 192,	96,		80,		96,		48  },
	{ 224,	112,	96,		112,	56  },
	{ 256,	128,	112,	128,	64  },
	{ 288,	160,	128,	144,	80  },
	{ 320,	192,	160,	160,	96  },
	{ 352,	224,	192,	176,	112 },
	{ 384,	256,	224,	192,	128 },
	{ 416,	320,	256,	224,	144 },
	{ 448,	384,	320,	256,	160 },
	{ -1,	-1,		-1,		-1,		-1  }
	};

// Mit dieser Funktion wird die Länge der MP3-Datei abgeschätzt!
// Wir kein gültiger Header gefunden wird von einer Auflösung von 128 kbit/sec ausgegangen!
// Die tatsächliche Länge wird dann beim Abspielen ermittelt!
BOOL CMp3Engine::GetMP3Header()
{
	USES_CONVERSION;
	BOOL bResult = TRUE;

#if 0
	CFile file;

	m_dwLength = 0;

	if (!file.Open(m_sFilename, CFile::modeRead|CFile::shareDenyNone))
		return FALSE;

	DWORD dwMP3Header;
	file.Read(&dwMP3Header, sizeof(dwMP3Header));

	if (dwMP3Header == 0x46464952)      // "RIFF" rückwärts
	{
		// Es handelt sich um eine RIFF-Datei. Nun den DATA-Block suchen.
		DWORD dwValue;
		file.Seek(0x10, CFile::begin);
		file.Read(&dwValue, sizeof(dwValue));
		file.Seek(0x28+dwValue, CFile::begin);
		file.Read(&dwMP3Header, sizeof(dwMP3Header));
	}

	// JUS 000105: Prüfen, ob ein ID3-Header am Anfang kommt!
	char *pHeader = (char *)(&dwMP3Header);
	if (pHeader[0] == 'I' && pHeader[1] == 'D' && pHeader[2] == '3')
	{									// "ID3" steht am Anfang von ID3v2-Tags
		//The first part of the ID3v2 tag is the 10 byte tag header, laid out
		//as follows:

		//ID3v2/file identifier      "ID3"
		//ID3v2 version              $04 00
		//ID3v2 flags                %abcd0000
		//ID3v2 size             4 * %0xxxxxxx

		// Den ID3-Header nun ueberspringen. An Stelle 6-9 steht die Länge (synchsafe integer)

#define ID3V2_HEADER_SIZE 10
		unsigned char sBuf[ID3V2_HEADER_SIZE];
		file.Seek(0, CFile::begin);
		file.Read(sBuf, sizeof(sBuf));

		int iFirst = sBuf[9],
			iSecond = sBuf[8],
			iThird = sBuf[7],
			iForth = sBuf[6];

		int iValue = iFirst + (iSecond << 7) + (iThird << 14) + (iForth << 21);
		file.Seek(iValue+ID3V2_HEADER_SIZE, CFile::begin);
		file.Read(&dwMP3Header, sizeof(dwMP3Header));
	}

	// Ich muss jetzt noch die Bits innerhalb der einzelnen Bytes umdrehen!
	DWORD dwIntelMP3Header = 0;
	for (int i=0;i<32;i++)
	{
		if (dwMP3Header & (1 << (((i/8+1)*8-1-i)+i/8*8)))
			dwIntelMP3Header += (1 << i);
	}

	memcpy(&m_MP3Header, &dwIntelMP3Header, sizeof(m_MP3Header));

	// Frame-Sync nicht gefunden..... noch suchen gehen!
	if (m_MP3Header.iFrameSync != 2047)
	{
		BOOL bFrameSyncFound = FALSE;

		file.SeekToBegin();
		const int BUFFER_SIZE = 2048;
		unsigned char szBuff[BUFFER_SIZE];
		while (!bFrameSyncFound)
		{
			int iBytesRead;
			iBytesRead = file.Read(szBuff, BUFFER_SIZE);

			for (int i=0;i<iBytesRead;i++)
			{
				if (szBuff[i] == 0xff)
				{
					if (i < BUFFER_SIZE-1)
					{
						if ((szBuff[i+1] & 0xF0) == 0xF0)
						{
							file.Seek(i, CFile::begin);
							file.Read(&dwMP3Header, sizeof(dwMP3Header));
							// Ich muss jetzt noch die Bits innerhalb der einzelnen Bytes umdrehen!
							DWORD dwIntelMP3Header = 0;
							for (int i=0;i<32;i++)
							{
								if (dwMP3Header & (1 << (((i/8+1)*8-1-i)+i/8*8)))
									dwIntelMP3Header += (1 << i);
							}

							memcpy(&m_MP3Header, &dwIntelMP3Header, sizeof(m_MP3Header));

							bFrameSyncFound = TRUE;
							break;
						}
					}
				}
			}

			if (iBytesRead != BUFFER_SIZE)
				break;
		}
	}

	int iCol = -1;

	//bits V1,L1	V1,L2	V1,L3	V2,L1	V2, L2 & L3 
	//0000 free		free	free	free	free 
	//0001 32		32		32		32		8 
	//0010 64		48		40		48		16 
	//0011 96		56		48		56		24 
	//0100 128		64		56		64		32 
	//0101 160		80		64		80		40 
	//0110 192		96		80		96		48 
	//0111 224		112		96		112		56 
	//1000 256		128		112		128		64 
	//1001 288		160		128		144		80 
	//1010 320		192		160		160		96 
	//1011 352		224		192		176		112 
	//1100 384		256		224		192		128 
	//1101 416		320		256		224		144 
	//1110 448		384		320		256		160 
	//1111 bad		bad		bad		bad		bad 
	//V1 - MPEG Version 1
	//V2 - MPEG Version 2 and Version 2.5
	//L1 - Layer I
	//L2 - Layer II
	//L3 - Layer III

	if (m_MP3Header.iMPEGAudioVersionID == 3 && m_MP3Header.iLayerDescription == 3)
		iCol = 0;
	if (m_MP3Header.iMPEGAudioVersionID == 3 && m_MP3Header.iLayerDescription == 1)
		iCol = 1;
	if (m_MP3Header.iMPEGAudioVersionID == 3 && m_MP3Header.iLayerDescription == 2)
		iCol = 2;
	if (m_MP3Header.iMPEGAudioVersionID != 3 && m_MP3Header.iLayerDescription == 3)
		iCol = 3;
	if (m_MP3Header.iMPEGAudioVersionID != 3 && m_MP3Header.iLayerDescription != 3)
		iCol = 4;

	// Die Länge der MP3-Datei überschlagen. Zuerst Standardwerte annehmen.
	DWORD dwFileLength = (DWORD)file.GetLength();

	if (iCol < 0 || dwFileLength == 0)
	{
		file.Close();
		return FALSE;
	}

	// Die vier Bits des BitratenIndex noch umdrehen (lo-endian)
	int iBitRateIndex=0;
	for (int i=0;i<4;i++)
	{
		if (m_MP3Header.iBitrateIndex & (8 >> i))
			iBitRateIndex += (1 << i);
	}

	int iBitRate = s_iaBitrates[iBitRateIndex][iCol]*1000;

	if (iBitRate <= 0)
		iBitRate = 128000;      // Standard

	if (m_MP3Header.iFrameSync == 2047) // Alle Bits an heißt: Hier beginnt ein MP3-Frame
	{
		DWORD dwPadding = 0;
		DWORD dwSampleRate = 44100;          //!!!!!!!!!!!!!!!
		DWORD dwFrameLengthInBytes = 144 * iBitRate / dwSampleRate + dwPadding;

		DWORD dwFrameCount = dwFileLength / dwFrameLengthInBytes;

		m_iBitRate = iBitRate;
		if (m_MP3Header.iChannelMode == 3)
			m_iChannels = 1;
		else
			m_iChannels = 2;
		m_iSampleRate = dwSampleRate;
	}
	else
		bResult = FALSE;

	DWORD dwEstimatedDuration = dwFileLength / (iBitRate / 8) * 1000;

	m_dwLength = dwEstimatedDuration;

	file.Close(); 
#endif

//	if (!bResult)
	{
		CSoundEngine::GlobalInit();

		m_dwLength = 0;

		FMOD::Sound* stream;
		FMOD_RESULT res = CSoundEngine::m_pFMODSystem->createStream(W2A(m_sFilename), FMOD_OPENONLY, 0, &stream);

		if (res != FMOD_OK)
		{
			return false;
		}

		res = stream->getLength((UINT*)&m_dwLength, FMOD_TIMEUNIT_MS);

		FMOD_SOUND_TYPE soundType;
		FMOD_SOUND_FORMAT soundFormat;
		int channels;
		int bits;
		stream->getFormat(&soundType, &soundFormat, &channels, &bits);
		if (channels == 1)
			m_iChannels = 1;
		else
			m_iChannels = 2;

		// MP3-Tags lesen
		GetTags(stream);

		stream->release();

		bResult = true;
	}

	return bResult;
}

BOOL CMp3Engine::WriteMP3Information(const CString& sFilename, const CSoundEngineItemData& MP3Info)
{
	try
	{
		if (CMisc::GetFileNameExtension(sFilename).MakeLower() != L".wma" &&
			CMisc::GetFileNameExtension(sFilename).MakeLower() != L".asf")
		{
			if (CMisc::IsReadOnly(sFilename))
				return FALSE;

			ID3::ID3Info^ id3Info = gcnew ID3::ID3Info(gcnew System::String(sFilename), true);

			int id3Version = Big3::Hitbase::Configuration::Settings::Current->UseID3Version;

			if (id3Version == 0 || id3Version == 2)
			{
				// HACK JUS 08.04.2007: Damit ID3v2 Tags auch geschrieben werden, wenn kein ID3v2 Header vorhanden ist!
				// Ab und zu mal schauen, ob es vielleicht eine neue Version der Lib gibt
				// http://www.codeproject.com/cs/library/Do_Anything_With_ID3.asp
				id3Info->ID3v1Info->HaveTag = true;		

				id3Info->ID3v1Info->Artist = gcnew System::String(MP3Info.m_sArtist);
				id3Info->ID3v1Info->Title = gcnew System::String(MP3Info.m_sTitle);
				id3Info->ID3v1Info->Comment = gcnew System::String(MP3Info.m_sComment);
				id3Info->ID3v1Info->TrackNumber = MP3Info.m_iTrack;
				id3Info->ID3v1Info->Album = gcnew System::String(MP3Info.m_sAlbum);
				id3Info->ID3v1Info->Year = gcnew System::String(MP3Info.m_sYear.Left(4));
			}

			if (id3Version == 1 || id3Version == 2)
			{
				// HACK JUS 08.04.2007: Damit ID3v2 Tags auch geschrieben werden, wenn kein ID3v2 Header vorhanden ist!
				// Ab und zu mal schauen, ob es vielleicht eine neue Version der Lib gibt
				// http://www.codeproject.com/cs/library/Do_Anything_With_ID3.asp
				id3Info->ID3v2Info->HaveTag = true;		

				id3Info->ID3v2Info->SetMinorVersion(3);
				id3Info->ID3v2Info->SetTextFrame("TPE1", gcnew System::String(MP3Info.m_sArtist));
				id3Info->ID3v2Info->SetTextFrame("TIT2", gcnew System::String(MP3Info.m_sTitle));
				id3Info->ID3v2Info->SetTextWithLanguageFrame("COMM", gcnew System::String(MP3Info.m_sComment));
				id3Info->ID3v2Info->SetTextFrame("TALB", gcnew System::String(MP3Info.m_sAlbum));
				id3Info->ID3v2Info->SetTextFrame("TRCK", MP3Info.m_iTrack.ToString());
				id3Info->ID3v2Info->SetTextFrame("TYER", gcnew System::String(MP3Info.m_sYear));
				
			}

			switch (id3Version)
			{
			case 0:
				id3Info->ID3v1Info->Save();
				break;
			case 1:
				id3Info->ID3v2Info->Save();
				break;
			case 2:
				id3Info->ID3v1Info->Save();
				id3Info->ID3v2Info->Save();
				break;
			}
		}
	}
	catch (System::Exception^ e)
	{
		MessageBox::Show(e->ToString(), Application::ProductName, MessageBoxButtons::OK, MessageBoxIcon::Information);
	}

	return TRUE;

/*	USES_CONVERSION;
	if (CMisc::GetFileNameExtension(sFilename).MakeLower() != ".wma" &&
		CMisc::GetFileNameExtension(sFilename).MakeLower() != ".asf")
	{
		ID3_Tag myTag;

		myTag.Link(W2A(sFilename));

		ID3_AddTrack(&myTag, MP3Info.m_iTrack, MP3Info.m_iNumberOfTracks, true);
		ID3_AddArtist(&myTag, W2A(MP3Info.m_sArtist), true);
		ID3_AddTitle(&myTag, W2A(MP3Info.m_sTitle), true);
		ID3_AddComment(&myTag, W2A(MP3Info.m_sComment), "", true);
		ID3_AddAlbum(&myTag, W2A(MP3Info.m_sAlbum), true);
		ID3_AddYear(&myTag, W2A(MP3Info.m_sYear.Left(4)), true);

		luint nTags;
		switch (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iVirtualCDID3Version)
		{
		case 0:
			nTags = myTag.Update(ID3TT_ID3V1);
			break;
		case 1:
			nTags = myTag.Update(ID3TT_ID3V2);
			break;
		case 2:
			nTags = myTag.Update(ID3TT_ID3V1);
			nTags = myTag.Update(ID3TT_ID3V2);
			break;
		}

		return nTags > 0;
	}

	return TRUE;*/
}

// Schreibt die MP3-Informationen in die MP3-Datei.
BOOL CMp3Engine::SaveToFile()
{
	CSoundEngineItemData data;

	data.m_sArtist = m_sArtist;
	data.m_sTitle = m_sTitle;
	data.m_sComment = m_sComment;
	data.m_sAlbum = m_sAlbum;
	data.m_iTrack = m_iTrack;
	data.m_sYear = m_sYear;

	return CMp3Engine::WriteMP3Information(m_sFilename, data);

/*	try
	{
		if (CMisc::GetFileNameExtension(m_sFilename).MakeLower() != L".wma" &&
			CMisc::GetFileNameExtension(m_sFilename).MakeLower() != L".asf")
		{
			if (CMisc::IsReadOnly(m_sFilename))
				return FALSE;

			ID3::ID3Info^ id3Info = gcnew ID3::ID3Info(gcnew System::String(m_sFilename), true);

			int id3Version = ((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iVirtualCDID3Version;

			if (id3Version == 0 || id3Version == 2)
			{
				id3Info->ID3v1Info->Artist = gcnew System::String(m_sArtist);
				id3Info->ID3v1Info->Title = gcnew System::String(m_sTitle);
				id3Info->ID3v1Info->Comment = gcnew System::String(m_sComment);
			}

			if (id3Version == 1 || id3Version == 2)
			{
				// HACK JUS 08.04.2007: Damit ID3v2 Tags auch geschrieben werden, wenn kein ID3v2 Header vorhanden ist!
				// Ab und zu mal schauen, ob es vielleicht eine neue Version der Lib gibt
				// http://www.codeproject.com/cs/library/Do_Anything_With_ID3.asp
				id3Info->ID3v2Info->HaveTag = true;		

				id3Info->ID3v2Info->SetMinorVersion(3);
				id3Info->ID3v2Info->SetTextFrame("TPE1", gcnew System::String(m_sArtist));
				id3Info->ID3v2Info->SetTextFrame("TIT2", gcnew System::String(m_sTitle));
				id3Info->ID3v2Info->SetTextWithLanguageFrame("COMM", gcnew System::String(m_sComment));
			}

			switch (id3Version)
			{
			case 0:
				id3Info->ID3v1Info->Save();
				break;
			case 1:
				id3Info->ID3v2Info->Save();
				break;
			case 2:
				id3Info->ID3v1Info->Save();
				id3Info->ID3v2Info->Save();
				break;
			}
		}
	}
	catch (System::Exception^ e)
	{
		MessageBox::Show(e->ToString(), Application::ProductName, MessageBoxButtons::OK, MessageBoxIcon::Information);
	}


		ID3_Tag myTag;

		myTag.Link(W2A(m_sFilename));

		ID3_AddArtist(&myTag, W2A(m_sArtist), true);
		ID3_AddTitle(&myTag, W2A(m_sTitle), true);
		ID3_AddComment(&myTag, W2A(m_sComment), "", true);

		luint nTags;
		switch (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iVirtualCDID3Version)
		{
		case 0:
			nTags = myTag.Update(ID3TT_ID3V1);
			break;
		case 1:
			nTags = myTag.Update(ID3TT_ID3V2);
			break;
		case 2:
			nTags = myTag.Update(ID3TT_ID3V1);
			nTags = myTag.Update(ID3TT_ID3V2);
			break;
		}

		return nTags > 0;*/

	return TRUE;
}

void CMp3Engine::GetTags(FMOD::Sound* stream)
{
	if (stream != NULL) 
	{ 
		int numTags, numTagsUpdated; 
		// Make sure there are tags 
		stream->getNumTags(&numTags, &numTagsUpdated); 
		if (numTags != 0) 
		{ 
			m_sArtist = GetTag(stream, L"ARTIST", L"TPE1");
			if (m_sArtist.IsEmpty())		// Wenn Artist leer ist, "AUTHOR"-Tag lesen (WMA)
				m_sArtist = GetTag(stream, L"AUTHOR", L"");

			m_sTitle = GetTag(stream, L"TITLE", L"TIT2");
			m_sAlbum = GetTag(stream, L"ALBUM", L"TALB");
			m_sYear = GetTag(stream, L"YEAR", L"TYER");
			m_sComment = GetTag(stream, L"COMMENT", L"??");
			m_iTrack = _wtoi(GetTag(stream, L"TRACK", L"TRCK"));
			m_sGenre = GetTag(stream, L"GENRE", L"TCON");
			m_sCopyright = GetTag(stream, L"", L"TCOP");
			m_sComposer = GetTag(stream, L"", L"TCOM");

			// Find the artists tag 
/*			FMOD_TAG tag;
			FMOD_RESULT res;

			for (int i=0;i<numTags;i++)
				stream->getTag(NULL, i, &tag);*/
		} 
	} 
}

// Liefert den Inhalt des Tags zurück. Benutzt die Einstellung von Hitbase, ob V1 oder V2 geliefert wird.
CString CMp3Engine::GetTag(FMOD::Sound* stream, const CString& tagV1, const CString& tagV2)
{
	USES_CONVERSION;
	FMOD_RESULT res;
	CString value = L"";
	FMOD_TAG tag;

	if (Big3::Hitbase::Configuration::Settings::Current->UseID3Version != 0 && tagV2.GetLength() > 0)
	{
		// ID3v2 lesen
		res = stream->getTag(W2A(tagV2), 0, &tag);
		
		if (res == FMOD_OK)
		{
			if (tag.datatype == FMOD_TAGDATATYPE_STRING_UTF16)
			{
				try
				{
					//Unicode-Startzeichen suchen
					char* p = (char*)tag.data;
					if (*p == '\xff' && *(p+1) == '\xfe')
						value = (wchar_t*)((char*)tag.data+2); 
					else
						value = (wchar_t*)tag.data; 
				}
				catch(...)
				{
					// Ungültige ID3-tags
					value = L"";
				}
			}
			else
				value = (char*)tag.data; 
		}
	}

	if ((Big3::Hitbase::Configuration::Settings::Current->UseID3Version == 0 ||
		Big3::Hitbase::Configuration::Settings::Current->UseID3Version == 2 && value == "") &&
		tagV1.GetLength() > 0)
	{
		res = stream->getTag(W2A(tagV1), 0, &tag);
		if (res == FMOD_OK)
			value = (char*)tag.data; 
	}

	return value;
}