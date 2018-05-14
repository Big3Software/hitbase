using namespace System;
using namespace System::Collections::Generic;

namespace HitbaseDataBase
{
public ref class CDUtility
{
public:
/*	static void CopyToNewFormat(CCD& oldCDData, Big3::Hitbase::DataBaseEngine::CD^ newCDData)
	{
		newCDData->ID = oldCDData.m_dwID;
		newCDData->Identity = gcnew String(oldCDData.GetIdentity()->m_sIdentity);
		newCDData->NumberOfTracks = oldCDData.m_wNumberOfTracks;
		newCDData->Sampler = oldCDData.m_bSampler ? true : false;
		newCDData->Title = gcnew String(oldCDData.m_sTitle);
		newCDData->Comment = gcnew String(oldCDData.m_sComment);
		newCDData->Codes = gcnew String(oldCDData.m_sCodes);
		newCDData->ArchiveNumber = gcnew String(oldCDData.m_sArchiveNumber);
		newCDData->Date = gcnew String(oldCDData.GetDataBase()->DateShort2Long(oldCDData.m_sDate));
		newCDData->Category = gcnew String(oldCDData.GetCategory()->m_sCategory);
		newCDData->Medium = gcnew String(oldCDData.GetMedium()->m_sMedium);
		newCDData->CDCoverFrontFilename = gcnew String(oldCDData.m_sBitmapPath);
		newCDData->UserField1 = gcnew String(oldCDData.m_sUser[0]);
		newCDData->UserField2 = gcnew String(oldCDData.m_sUser[1]);
		newCDData->UserField3 = gcnew String(oldCDData.m_sUser[2]);
		newCDData->UserField4 = gcnew String(oldCDData.m_sUser[3]);
		newCDData->UserField5 = gcnew String(oldCDData.m_sUser[4]);
		newCDData->Copyright = gcnew String(oldCDData.m_sCopyright);
		newCDData->Label = gcnew String(oldCDData.m_sLabel);
		newCDData->Language = gcnew String(oldCDData.m_sLanguage);
		newCDData->Location = gcnew String(oldCDData.m_sLocation);
		newCDData->Original = oldCDData.m_bOriginal ? true : false;
		newCDData->Rating = oldCDData.m_lRating;
		newCDData->UPC = gcnew String(oldCDData.m_sUPC);
		newCDData->URL = gcnew String(oldCDData.m_sURL);
		newCDData->YearRecorded = oldCDData.m_lYearRecorded;
		newCDData->Price = oldCDData.m_lPrice;
		newCDData->CDCoverLabelFilename = gcnew String(oldCDData.m_sCDLabelBitmap);
		newCDData->CDCoverBackFilename = gcnew String(oldCDData.m_sBackCoverFilename);

		newCDData->Artist = gcnew String(oldCDData.GetArtist()->m_sArtist);
		newCDData->Composer = gcnew String(oldCDData.GetComposer()->m_sArtist);

		for (int i=0;i<oldCDData.m_wNumberOfTracks;i++)
		{
			if (newCDData->Sampler)
				newCDData->Tracks[i]->Artist = gcnew String(oldCDData.GetTrack(i)->GetArtist()->m_sArtist);
			else
				newCDData->Tracks[i]->Artist = gcnew String(oldCDData.GetArtist()->m_sArtist);
			newCDData->Tracks[i]->Title = gcnew String(oldCDData.GetTrack(i)->m_sTitle);
			newCDData->Tracks[i]->Length = oldCDData.GetTrack(i)->m_dwLength;
			newCDData->Tracks[i]->BitRate = oldCDData.GetTrack(i)->m_lBitRate;
			newCDData->Tracks[i]->Bpm = oldCDData.GetTrack(i)->m_wBpm;
			newCDData->Tracks[i]->Category = gcnew String(oldCDData.GetTrack(i)->GetCategory()->m_sCategory);
			newCDData->Tracks[i]->Channels = oldCDData.GetTrack(i)->m_lChannels;
			newCDData->Tracks[i]->CheckSum = gcnew String(oldCDData.GetTrack(i)->m_sCheckSum);
			newCDData->Tracks[i]->Codes = gcnew String(oldCDData.GetTrack(i)->m_sCodes);
			newCDData->Tracks[i]->Comment = gcnew String(oldCDData.GetTrack(i)->m_sComment);
			newCDData->Tracks[i]->Composer = gcnew String(oldCDData.GetTrack(i)->GetComposer()->m_sArtist);
			newCDData->Tracks[i]->Format = oldCDData.GetTrack(i)->m_lFormat;
			newCDData->Tracks[i]->Language = gcnew String(oldCDData.GetTrack(i)->m_sLanguage);
			newCDData->Tracks[i]->Lyrics = gcnew String(oldCDData.GetTrack(i)->m_sLyrics);
			newCDData->Tracks[i]->Rating = oldCDData.GetTrack(i)->m_lRating;
			newCDData->Tracks[i]->SampleRate = oldCDData.GetTrack(i)->m_lSampleRate;
			newCDData->Tracks[i]->TrackNumber = oldCDData.GetTrack(i)->m_wTrack;
			newCDData->Tracks[i]->UserField1 = gcnew String(oldCDData.GetTrack(i)->m_sUser[0]);
			newCDData->Tracks[i]->UserField2 = gcnew String(oldCDData.GetTrack(i)->m_sUser[1]);
			newCDData->Tracks[i]->UserField3 = gcnew String(oldCDData.GetTrack(i)->m_sUser[2]);
			newCDData->Tracks[i]->UserField4 = gcnew String(oldCDData.GetTrack(i)->m_sUser[3]);
			newCDData->Tracks[i]->UserField5 = gcnew String(oldCDData.GetTrack(i)->m_sUser[4]);
			newCDData->Tracks[i]->Soundfile = gcnew String(oldCDData.GetTrack(i)->m_sWavePath);
			newCDData->Tracks[i]->YearRecorded = oldCDData.GetTrack(i)->m_lYearRecorded;
		}

		newCDData->Participants = gcnew ParticipantList();
		for (int i=0;i<oldCDData.GetParticipants()->GetCount();i++)
		{
			Participant^ newParticipant = gcnew Participant();
			newParticipant->Name = oldCDData.GetDataBase()->theDataBase->GetArtistById(oldCDData.GetParticipants()->GetAt(i).m_dwIDArtist)->szArtistName;
			newParticipant->Role = oldCDData.GetDataBase()->theDataBase->GetRoleById(oldCDData.GetParticipants()->GetAt(i).m_dwIDRole)->R_Role;

			newParticipant->TrackNumber = oldCDData.GetParticipants()->GetAt(i).m_dwTrackNumber;
			newParticipant->Comment = gcnew String(oldCDData.GetParticipants()->GetAt(i).m_sComment);
			newCDData->Participants->Add(newParticipant);
		}
	}

	static void CopyFromNewFormat(CCD& oldCDData, Big3::Hitbase::DataBaseEngine::CD^ newCDData)
	{
		BOOL oldCDSampler = oldCDData.m_bSampler;

		oldCDData.GetArtist()->m_sArtist = CString(newCDData->Artist);
		oldCDData.GetComposer()->m_sArtist = CString(newCDData->Composer);
		oldCDData.m_sTitle = CString(newCDData->Title);
		oldCDData.m_wNumberOfTracks = newCDData->NumberOfTracks;

		oldCDData.m_bSampler = newCDData->Sampler;

		CString str = CString(newCDData->Date);
		oldCDData.m_sDate = oldCDData.GetDataBase()->DateLong2Short(str);

		oldCDData.GetCategory()->m_sCategory = CString(newCDData->Category);
		oldCDData.GetMedium()->m_sMedium = CString(newCDData->Medium);

		oldCDData.m_sArchiveNumber = CString(newCDData->ArchiveNumber);
		oldCDData.m_sCodes = CString(newCDData->Codes);
		oldCDData.m_lYearRecorded = newCDData->YearRecorded;
		oldCDData.m_lPrice = newCDData->Price;

		oldCDData.m_lRating = newCDData->Rating;
		oldCDData.m_sCopyright = CString(newCDData->Copyright);
		oldCDData.m_sComment = CString(newCDData->Comment);
		oldCDData.m_sLabel = CString(newCDData->Label);

		oldCDData.m_sURL = CString(newCDData->URL);
		oldCDData.m_sUPC = CString(newCDData->UPC);
		oldCDData.m_bOriginal = newCDData->Original;

		oldCDData.m_sLanguage = CString(newCDData->Language);
		oldCDData.m_sLocation = CString(newCDData->Location);

		oldCDData.m_sBitmapPath = CString(newCDData->CDCoverFrontFilename);
		oldCDData.m_sBackCoverFilename = CString(newCDData->CDCoverBackFilename);
		oldCDData.m_sCDLabelBitmap = CString(newCDData->CDCoverLabelFilename);

		oldCDData.m_sUser[0] = CString(newCDData->UserField1);
		oldCDData.m_sUser[1] = CString(newCDData->UserField2);
		oldCDData.m_sUser[2] = CString(newCDData->UserField3);
		oldCDData.m_sUser[3] = CString(newCDData->UserField4);
		oldCDData.m_sUser[4] = CString(newCDData->UserField5);

		for (int i=0;i<newCDData->NumberOfTracks;i++)
		{
			if (oldCDData.m_bSampler && oldCDSampler)
				oldCDData.GetTrack(i)->GetArtist()->m_sArtist = CString(newCDData->Tracks[i]->Artist);
			oldCDData.GetTrack(i)->m_sTitle = CString(newCDData->Tracks[i]->Title);
			oldCDData.GetTrack(i)->m_dwLength = newCDData->Tracks[i]->Length;
			oldCDData.GetTrack(i)->m_lBitRate = newCDData->Tracks[i]->BitRate;
			oldCDData.GetTrack(i)->m_wBpm = newCDData->Tracks[i]->Bpm;
			oldCDData.GetTrack(i)->GetCategory()->m_sCategory = CString(newCDData->Tracks[i]->Category);
			oldCDData.GetTrack(i)->m_lChannels = newCDData->Tracks[i]->Channels;
			oldCDData.GetTrack(i)->m_sCheckSum = CString(newCDData->Tracks[i]->CheckSum);
			oldCDData.GetTrack(i)->m_sCodes = CString(newCDData->Tracks[i]->Codes);
			oldCDData.GetTrack(i)->m_sComment = CString(newCDData->Tracks[i]->Comment);
			oldCDData.GetTrack(i)->GetComposer()->m_sArtist = CString(newCDData->Tracks[i]->Composer);
			oldCDData.GetTrack(i)->m_lFormat = newCDData->Tracks[i]->Format;
			oldCDData.GetTrack(i)->m_sLanguage = CString(newCDData->Tracks[i]->Language);
			oldCDData.GetTrack(i)->m_sLyrics = CString(newCDData->Tracks[i]->Lyrics);
			oldCDData.GetTrack(i)->m_lRating = newCDData->Tracks[i]->Rating;
			oldCDData.GetTrack(i)->m_lSampleRate = newCDData->Tracks[i]->SampleRate;
			oldCDData.GetTrack(i)->m_wTrack = newCDData->Tracks[i]->TrackNumber;
			oldCDData.GetTrack(i)->m_sUser[0] = CString(newCDData->Tracks[i]->UserField1);
			oldCDData.GetTrack(i)->m_sUser[1] = CString(newCDData->Tracks[i]->UserField2);
			oldCDData.GetTrack(i)->m_sUser[2] = CString(newCDData->Tracks[i]->UserField3);
			oldCDData.GetTrack(i)->m_sUser[3] = CString(newCDData->Tracks[i]->UserField4);
			oldCDData.GetTrack(i)->m_sUser[4] = CString(newCDData->Tracks[i]->UserField5);
			oldCDData.GetTrack(i)->m_sWavePath = CString(newCDData->Tracks[i]->Soundfile);
			oldCDData.GetTrack(i)->m_lYearRecorded = newCDData->Tracks[i]->YearRecorded;
		}

		oldCDData.GetParticipants()->RemoveAll();
		if (newCDData->Participants != nullptr)
		{
			for (int i=0;i<newCDData->Participants->Count;i++)
			{
				CParticipant participant(oldCDData.GetDataBase());
				
				participant.m_dwIDArtist = oldCDData.GetDataBase()->theDataBase->GetArtistByName(newCDData->Participants[i]->Name, true)->IDArtist;
				participant.m_dwIDRole = oldCDData.GetDataBase()->theDataBase->GetRoleByName(newCDData->Participants[i]->Role)->R_ID;

				participant.m_dwTrackNumber = newCDData->Participants[i]->TrackNumber;
				participant.m_sComment = CString(newCDData->Participants[i]->Comment);

				oldCDData.GetParticipants()->Add(participant);
			}
		}
	}*/
};
}




