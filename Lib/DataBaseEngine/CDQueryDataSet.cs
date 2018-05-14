using Big3.Hitbase.Miscellaneous;
using System;
using System.Data;
using Big3.Hitbase.SharedResources;
namespace Big3.Hitbase.DataBaseEngine
{
    partial class CDQueryDataSet
    {
        partial class PersonGroupDataTable
        {
            public void a()
            {
                //this.Get
            }
        }

        partial class CDDataTable
        {
            public DataColumn GetDataColumnByField(Field field)
            {
                switch (field)
                {
                    case Field.Identity: return IdentityColumn;
                    case Field.Title: return TitleColumn;
                    case Field.Sampler: return IsSamplerColumn;
                    case Field.TotalLength: return LengthColumn;
                    case Field.NumberOfTracks: return NumberOfTracksColumn;
                    case Field.CDSet: return SetIDColumn;
                    case Field.DiscNumberInCDSet: return SetNumberColumn;
                    case Field.Category: return CategoryIDColumn;
                    case Field.Medium: return MediumIDColumn;
                    case Field.Comment: return CommentColumn;
                    case Field.Copyright: return CopyrightColumn;
                    case Field.Label: return LabelColumn;
                    case Field.YearRecorded: return YearRecordedColumn;
                    case Field.Date: return DateColumn;
                    case Field.Codes: return CodesColumn;
                    case Field.ArchiveNumber: return ArchiveNumberColumn;
                    case Field.Rating: return RatingColumn;
                    case Field.Language: return LanguageColumn;
                    case Field.Location: return LocationColumn;
                    case Field.UPC: return UPCColumn;
                    case Field.Price: return PriceColumn;
                    case Field.CDCoverFront: return FrontCoverColumn;
                    case Field.CDCoverBack: return BackCoverColumn;
                    case Field.CDCoverLabel: return CDLabelCoverColumn;
                    case Field.OriginalCD: return IsOriginalColumn;
                    case Field.Homepage: return URLColumn;
                    case Field.User1: return User1Column;
                    case Field.User2: return User2Column;
                    case Field.User3: return User3Column;
                    case Field.User4: return User4Column;
                    case Field.User5: return User5Column;
                    case Field.AlbumType: return this.TypeColumn;
                    case Field.Created: return this.CreatedColumn;
                    case Field.LastModified: return this.LastModifiedColumn;
                    default: throw new Exception("GetDataColumnByField: Unknown field: " + field.ToString());
                }
            }
        }

        partial class TrackDataTable
        {
            /// <summary>
            /// Liefert den Datentyp des angegebenen Feldes zurück
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            public Type GetTypeByField(Field field)
            {
                DataColumn col = GetDataColumnByField(field);

                return col.DataType;
            }

            /// <summary>
            /// Liefert die Spalte der Tabelle Lied zurück (nur die reinen Lied-Felder ohne Relations-IDs)
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            public System.Data.DataColumn GetDataColumnByField(Field field)
            {
                switch (field)
                {
                    case Field.TrackTitle: return TitleColumn;
                    case Field.TrackLength: return LengthColumn;
                    case Field.TrackNumber: return TrackNumberColumn;
                    case Field.TrackBpm: return BpmColumn;
                    case Field.TrackCodes: return CodesColumn;
                    case Field.TrackCategory: return CategoryIDColumn;
                    case Field.TrackComment: return CommentColumn;
                    case Field.TrackLyrics: return LyricsColumn;
                    case Field.TrackSoundFile: return SoundFileColumn;
                    case Field.TrackYearRecorded: return YearRecordedColumn;
                    case Field.TrackRating: return RatingColumn;
                    case Field.TrackChecksum: return ChecksumColumn;
                    case Field.TrackLanguage: return LanguageColumn;
                    case Field.TrackSoundFileLastModified: return SoundFileLastModifiedColumn;
                    case Field.TrackPlayCount: return PlayCountColumn;

                    case Field.TrackUser1: return User1Column;
                    case Field.TrackUser2: return User2Column;
                    case Field.TrackUser3: return User3Column;
                    case Field.TrackUser4: return User4Column;
                    case Field.TrackUser5: return User5Column;

                    default: return null;
                }
            }
        }

        partial class CDRow
        {
            public string GetStringByField(DataBase db, Field field)
            {
                object val = GetValueByField(field);

                return GetStringByField(db, field, val);
            }

            public static string GetStringByField(DataBase db, Field field, object val)
            {
                if (field == Field.TotalLength)
                    return Misc.GetShortTimeString((int)val);

                if (field == Field.Price)
                {
                    return Misc.FormatCurrencyValue((int)val);
                }

                if (field == Field.ArtistCDType || field == Field.ArtistTrackType ||
                    field == Field.ComposerCDType || field == Field.ComposerTrackType)
                    return DataBase.GetNameOfPersonGroupType(val == null ? PersonGroupType.Unknown : (PersonGroupType)val);

                if (field == Field.ArtistCDSex || field == Field.ArtistTrackSex ||
                    field == Field.ComposerCDSex || field == Field.ComposerTrackSex)
                    return DataBase.GetNameOfPersonGroupSex(val == null ? SexType.Unknown : (SexType)val);

                if (field == Field.Date)
                    return db.FormatDate((string)val);

                if (field == Field.YearRecorded && (int)val == 0)
                    return "";

                if (val is bool)
                {
                    if ((bool)val == true)
                        return StringTable.Yes;
                    else
                        return StringTable.No;
                }

                if (field == Field.User1 || field == Field.User2 || field == Field.User3 ||
                    field == Field.User4 || field == Field.User5)
                    return db.GetDisplayStringByUserField(val as string, field); 
                
                if (val != null)
                    return val.ToString();
                else
                    return "";
            }

            public object GetValueByField(Field field)
            {
                switch (field)
                {
                    case Field.Identity:
                        return Identity;
                    case Field.Title:
                        return Title;
                    case Field.Sampler:
                        return IsSampler;
                    case Field.TotalLength:
                        return Length; 
                    case Field.NumberOfTracks:
                        return NumberOfTracks;
                    case Field.CDSet:
                        return (SetRow == null) ? null : SetRow.Name;
                    case Field.DiscNumberInCDSet:
                        return SetNumber;
                    case Field.Category:
                        return (CategoryRow == null) ? null : CategoryRow.Name;
                    case Field.Medium:
                        return (MediumRow == null) ? null : MediumRow.Name;
                    case Field.Comment:
                        return Comment;
                    case Field.Copyright:
                        return Copyright;
                    case Field.Label:
                        return Label;
                    case Field.YearRecorded:
                        return IsYearRecordedNull() ? 0 : YearRecorded;
                    case Field.Date:
                        return Date;
                    case Field.Codes:
                        return Codes;
                    case Field.ArchiveNumber:
                        return ArchiveNumber;
                    case Field.Rating:
                        return IsRatingNull() ? 0 : Rating;
                    case Field.Language:
                        return Language;
                    case Field.Location:
                        return Location;
                    case Field.UPC:
                        return UPC;
                    case Field.Price:
                        return IsPriceNull() ? 0 : Price;
                    case Field.CDCoverFront:
                        return FrontCover;
                    case Field.CDCoverBack:
                        return BackCover;
                    case Field.CDCoverLabel:
                        return CDLabelCover;
                    case Field.OriginalCD:
                        return IsOriginal;
                    case Field.Homepage:
                        return URL;
                    case Field.AlbumType:
                        return IsTypeNull() ? 0 : Type;
                    case Field.User1:
                        return User1;
                    case Field.User2:
                        return User2;
                    case Field.User3:
                        return User3;
                    case Field.User4:
                        return User4;
                    case Field.User5:
                        return User5;

                    case Field.ArtistCDName:
                        return (PersonGroupRowByArtist == null) ? null : PersonGroupRowByArtist.Name;
                    case Field.ArtistCDSaveAs:
                        return (PersonGroupRowByArtist == null) ? null : PersonGroupRowByArtist.SaveAs;
                    case Field.ArtistCDType:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsTypeNull() ? PersonGroupType.Unknown : (object)PersonGroupRowByArtist.Type;
                    case Field.ArtistCDSex:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsSexNull() ? SexType.Unknown : (object)PersonGroupRowByArtist.Sex;
                    case Field.ArtistCDCountry:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsCountryNull() ? null : PersonGroupRowByArtist.Country;
                    case Field.ArtistCDHomepage:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsURLNull() ? null : PersonGroupRowByArtist.URL;
                    case Field.ArtistCDDateOfBirth:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsBirthDayNull() ? null : (object)PersonGroupRowByArtist.BirthDay;
                    case Field.ArtistCDDateOfDeath:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsDayOfDeathNull() ? null : (object)PersonGroupRowByArtist.DayOfDeath;
                    case Field.ArtistCDComment:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsCommentNull() ? null : PersonGroupRowByArtist.Comment;
                    case Field.ArtistCDImageFilename:
                        return (PersonGroupRowByArtist == null) || PersonGroupRowByArtist.IsImageFilenameNull() ? null : PersonGroupRowByArtist.ImageFilename;

                    case Field.ComposerCDName:
                        return (this.PersonGroupRowByComposer == null) ? null : PersonGroupRowByComposer.Name;
                    case Field.ComposerCDSaveAs:
                        return (PersonGroupRowByComposer == null) ? null : PersonGroupRowByComposer.SaveAs;
                    case Field.ComposerCDType:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsTypeNull() ? PersonGroupType.Unknown : (object)PersonGroupRowByComposer.Type;
                    case Field.ComposerCDSex:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsSexNull() ? SexType.Unknown : (object)PersonGroupRowByComposer.Sex;
                    case Field.ComposerCDCountry:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsCountryNull() ? null : PersonGroupRowByComposer.Country;
                    case Field.ComposerCDHomepage:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsURLNull() ? null : PersonGroupRowByComposer.URL;
                    case Field.ComposerCDDateOfBirth:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsBirthDayNull() ? null : (object)PersonGroupRowByComposer.BirthDay;
                    case Field.ComposerCDDateOfDeath:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsDayOfDeathNull() ? null : (object)PersonGroupRowByComposer.DayOfDeath;
                    case Field.ComposerCDComment:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsCommentNull() ? null : PersonGroupRowByComposer.Comment;
                    case Field.ComposerCDImageFilename:
                        return (PersonGroupRowByComposer == null) || PersonGroupRowByComposer.IsImageFilenameNull() ? null : PersonGroupRowByComposer.ImageFilename;

                    default:
                        return null;
                }
            }
        }

        partial class TrackRow
        {
            public string GetStringByField(DataBase db, Field field)
            {
                object val = GetValueByField(db, field);

                return GetStringByValue(db, field, val);
            }

            public static string GetStringByValue(DataBase db, Field field, object val)
            {
                if (field == Field.TrackLength || field == Field.TotalLength)
                    return Misc.GetShortTimeString((int)val);

                if (field == Field.Price)
                {
                    return Misc.FormatCurrencyValue((int)val);
                }

                if (field == Field.ArtistCDType || field == Field.ArtistTrackType ||
                    field == Field.ComposerCDType || field == Field.ComposerTrackType)
                    return DataBase.GetNameOfPersonGroupType(val == null ? PersonGroupType.Unknown : (PersonGroupType)val);

                if (field == Field.ArtistCDSex || field == Field.ArtistTrackSex ||
                    field == Field.ComposerCDSex || field == Field.ComposerTrackSex)
                    return DataBase.GetNameOfPersonGroupSex(val == null ? SexType.Unknown : (SexType)val);

                if (field == Field.Date)
                    return db.FormatDate((string)val);

                if (field == Field.YearRecorded && (int)val == 0)
                    return "";

                if (val is bool)
                {
                    if ((bool)val == true)
                        return StringTable.Yes;
                    else
                        return StringTable.No;
                }

                if (val != null)
                    return val.ToString();
                else
                    return "";
            }

            public object GetValueByField(DataBase db, Field field)
            {
                switch (field)
                {
                    case Field.TrackTitle:
                        return Title;
                    case Field.TrackLength:
                        return Length;
                    case Field.TrackNumber:
                        return TrackNumber;
                    case Field.TrackBpm:
                        if (!IsBpmNull())
                            return Bpm;
                        else
                            return 0;
                    case Field.TrackCodes:
                        return Codes;
                    case Field.TrackCategory:
                        if (!IsCategoryIDNull())
                        {
                            Category category = db.AllCategories.GetById(CategoryID);
                            if (category != null)
                                return category.Name;
                            else
                                return null;
                        }
                        else
                        {
                            return null;
                        }
                    case Field.TrackComment:
                        return Comment;
                    case Field.TrackLyrics:
                        return Lyrics;
                    case Field.TrackSoundFile:
                        return SoundFile;
                    case Field.TrackYearRecorded:
                        return IsYearRecordedNull() ? 0 : YearRecorded;
                    case Field.TrackRating:
                        return IsRatingNull() ? 0 : Rating;
                    case Field.TrackChecksum:
                        return Checksum;
                    case Field.TrackLanguage:
                        return Language;
                    case Field.TrackSoundFileLastModified:
                        return IsSoundFileLastModifiedNull() ? new DateTime(1900,1,1) : SoundFileLastModified;
                    case Field.TrackPlayCount:
                        return IsPlayCountNull() ? 0 : PlayCount;
                    case Field.TrackUser1:
                        return User1;
                    case Field.TrackUser2:
                        return User2;
                    case Field.TrackUser3:
                        return User3;
                    case Field.TrackUser4:
                        return User4;
                    case Field.TrackUser5:
                        return User5;
                    
                    case Field.ArtistTrackName:
                        return (PersonGroupRowByArtistTrack == null) ? null : PersonGroupRowByArtistTrack.Name;
                    case Field.ArtistTrackSaveAs:
                        return (PersonGroupRowByArtistTrack == null) ? null : PersonGroupRowByArtistTrack.SaveAs;
                    case Field.ArtistTrackType:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsTypeNull() ? PersonGroupType.Unknown : (object)PersonGroupRowByArtistTrack.Type;
                    case Field.ArtistTrackSex:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsSexNull() ? SexType.Unknown : (object)PersonGroupRowByArtistTrack.Sex;
                    case Field.ArtistTrackCountry:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsCountryNull() ? null : PersonGroupRowByArtistTrack.Country;
                    case Field.ArtistTrackHomepage:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsURLNull() ? null : PersonGroupRowByArtistTrack.URL;
                    case Field.ArtistTrackDateOfBirth:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsBirthDayNull() ? null : (object)PersonGroupRowByArtistTrack.BirthDay;
                    case Field.ArtistTrackDateOfDeath:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsDayOfDeathNull() ? null : (object)PersonGroupRowByArtistTrack.DayOfDeath;
                    case Field.ArtistTrackComment:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsCommentNull() ? null : PersonGroupRowByArtistTrack.Comment;
                    case Field.ArtistTrackImageFilename:
                        return (PersonGroupRowByArtistTrack == null) || PersonGroupRowByArtistTrack.IsImageFilenameNull() ? null : PersonGroupRowByArtistTrack.ImageFilename;

                    case Field.ComposerTrackName:
                        return (this.PersonGroupRowByComposerTrack == null) ? null : PersonGroupRowByComposerTrack.Name;
                    case Field.ComposerTrackSaveAs:
                        return (PersonGroupRowByComposerTrack == null) ? null : PersonGroupRowByComposerTrack.SaveAs;
                    case Field.ComposerTrackType:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsTypeNull() ? PersonGroupType.Unknown : (object)PersonGroupRowByComposerTrack.Type;
                    case Field.ComposerTrackSex:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsSexNull() ? SexType.Unknown : (object)PersonGroupRowByComposerTrack.Sex;
                    case Field.ComposerTrackCountry:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsCountryNull() ? null : PersonGroupRowByComposerTrack.Country;
                    case Field.ComposerTrackHomepage:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsURLNull() ? null : PersonGroupRowByComposerTrack.URL;
                    case Field.ComposerTrackDateOfBirth:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsBirthDayNull() ? null : (object)PersonGroupRowByComposerTrack.BirthDay;
                    case Field.ComposerTrackDateOfDeath:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsDayOfDeathNull() ? null : (object)PersonGroupRowByComposerTrack.DayOfDeath;
                    case Field.ComposerTrackComment:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsCommentNull() ? null : PersonGroupRowByComposerTrack.Comment;
                    case Field.ComposerTrackImageFilename:
                        return (PersonGroupRowByComposerTrack == null) || PersonGroupRowByComposerTrack.IsImageFilenameNull() ? null : PersonGroupRowByComposerTrack.ImageFilename;

                    default:
                        return CDRow.GetValueByField(field);
                }
            }
        }
    }
}

namespace Big3.Hitbase.DataBaseEngine.CDQueryDataSetTableAdapters
{
    public partial class CDTableAdapter
    {
        public CDTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

    public partial class TrackTableAdapter
    {
        public TrackTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

    public partial class PersonGroupTableAdapter
    {
        public PersonGroupTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

    public partial class CategoryTableAdapter
    {
        public CategoryTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

    public partial class MediumTableAdapter
    {
        public MediumTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

    public partial class SetTableAdapter
    {
        public SetTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }

}