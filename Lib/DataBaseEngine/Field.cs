using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.Miscellaneous;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.SharedResources;
using System.Windows;
using System.ComponentModel;

namespace Big3.Hitbase.DataBaseEngine
{
    public enum Field
    {
        None = 0,
        Title,
        CDSet,
        DiscNumberInCDSet,
        Sampler,
        TotalLength,
        NumberOfTracks,
        Category,
        Medium,
        Comment,
        Copyright,
        Label,
        YearRecorded,
        Date,
        Codes,
        ArchiveNumber,
        Rating,
        Language,
        Location,
        UPC,
        Price,
        CDCoverFront,
        CDCoverBack,
        CDCoverLabel,
        OriginalCD,
        Homepage,
        Identity,
        CDID,
        AlbumType,
        Created,
        LastModified,

        User1 = 50,
        User2,
        User3,
        User4,
        User5,

        TrackTitle = 100,
        TrackLength,
        TrackNumber,
        TrackBpm,
        TrackCodes,
        TrackCategory,
        TrackComment,
        TrackLyrics,
        TrackSoundFile,
        TrackYearRecorded,
        TrackRating,
        TrackChecksum,
        TrackLanguage,
        TrackID,
        TrackSoundFileLastModified,
        TrackPlayCount,

        TrackUser1 = 150,
        TrackUser2,
        TrackUser3,
        TrackUser4,
        TrackUser5,

        ArtistCDName = 200,
        ArtistCDSaveAs,
        ArtistCDType,
        ArtistCDSex,
        ArtistCDCountry,
        ArtistCDHomepage,
        ArtistCDDateOfBirth,
        ArtistCDDateOfDeath,
        ArtistCDComment,
        ArtistCDImageFilename,

        ArtistTrackName = 250,
        ArtistTrackSaveAs,
        ArtistTrackType,
        ArtistTrackSex,
        ArtistTrackCountry,
        ArtistTrackHomepage,
        ArtistTrackDateOfBirth,
        ArtistTrackDateOfDeath,
        ArtistTrackComment,
        ArtistTrackImageFilename,

        ComposerCDName = 300,
        ComposerCDSaveAs,
        ComposerCDType,
        ComposerCDSex,
        ComposerCDCountry,
        ComposerCDHomepage,
        ComposerCDDateOfBirth,
        ComposerCDDateOfDeath,
        ComposerCDComment,
        ComposerCDImageFilename,

        ComposerTrackName = 350,
        ComposerTrackSaveAs,
        ComposerTrackType,
        ComposerTrackSex,
        ComposerTrackCountry,
        ComposerTrackHomepage,
        ComposerTrackDateOfBirth,
        ComposerTrackDateOfDeath,
        ComposerTrackComment,
        ComposerTrackImageFilename
    }

    public class FieldCollection : List<Field>
    {
        public void SortFields(DataBase db)
        {
            Sort(new Comparison<Field>(delegate(Field a, Field b)
            {
                return String.Compare(db.GetNameOfField(a), db.GetNameOfField(b));
            }));
        }
    }

    public enum FieldType
    {
        Unknown = 0,
        CD,             // Nur CD-Felder
        Track,          // Nur Track-Felder
        TrackAndCD,     // Track und CD-Felder
        TrackMain,      // Nur die Hauptfelder der Tracks (also z.B. ohne Komponist, Bild)
        All
    }

    public enum PersonType
    {
        Unknown = 0,
        Artist,
        Composer
    }

    public class FieldHelper
    {
        static public bool IsCDField(Field field)
        {
            return IsCDField(field, false);
        }

        static public bool IsCDField(Field field, bool pureCDField)
        {
            if ((int)field > 0 && (int)field < 100)
                return true;

            if (!pureCDField)
            {
                if ((int)field >= 200 && (int)field <= 249
                    || (int)field >= 300 && (int)field <= 349)
                    return true;
            }

            return false;
        }

        static public bool IsTrackField(Field field)
        {
            return IsTrackField(field, false);
        }

        static public bool IsTrackField(Field field, bool pureTrackField)
        {
            if ((int)field >= 100 && (int)field < 200)
                return true;

            if (!pureTrackField)
            {
                if ((int)field >= 250 && (int)field <= 299
                    || (int)field >= 350 && (int)field <= 399)
                    return true;
            }
            else
            {
                if (field == Field.ArtistTrackName || field == Field.ComposerTrackName)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Liefert alle verfügbaren Felder zurück.
        /// </summary>
        /// <returns></returns>
        static public FieldCollection GetAllFields()
        {
            FieldCollection fields = new FieldCollection();
            foreach (Field field in Enum.GetValues(typeof(Field)))
            {
                if (field != Field.None && field != Field.CDID && field != Field.TrackID)
                    fields.Add(field);
            }

            return fields;
        }

        /// <summary>
        /// Liefert alle CD-Felder zurück
        /// </summary>
        /// <param name="pureCDFields">true, wenn nur die reinen CD-Felder zurückgeliefert werden sollen.
        /// (also ohne die Interpreten und Komponisten-Felder)</param>
        /// <returns></returns>
        static public FieldCollection GetAllCDFields(bool pureCDFields)
        {
            FieldCollection fields = new FieldCollection();
            foreach (Field field in Enum.GetValues(typeof(Field)))
            {
                if (IsCDField(field, pureCDFields) && field != Field.CDID && field != Field.TrackID)
                    fields.Add(field);
            }

            return fields;
        }

        /// <summary>
        /// Liefert alle Track-Felder zurück.
        /// </summary>
        /// <param name="pureTrackFields">true, wenn nur die reinen Track-Felder zurückgeliefert werden sollen.
        /// (also ohne die Interpreten und Komponisten-Felder)</param>
        /// <returns></returns>
        static public FieldCollection GetAllTrackFields(bool pureTrackFields)
        {
            FieldCollection fields = new FieldCollection();
            foreach (Field field in Enum.GetValues(typeof(Field)))
            {
                if (IsTrackField(field, pureTrackFields) && field != Field.CDID && field != Field.TrackID)
                    fields.Add(field);
            }

            return fields;
        }

        static public bool IsUserField(Field field)
        {
            if (field >= Field.User1 && field <= Field.User5 || field >= Field.TrackUser1 && field <= Field.TrackUser5)
                return true;

            return false;
        }

        static public bool IsCDArtistField(Field field)
        {
            if (field >= Field.ArtistCDName && field < Field.ArtistTrackName)
                return true;

            return false;
        }

        static public bool IsCDComposerField(Field field)
        {
            if (field >= Field.ComposerCDName && field < Field.ComposerTrackName)
                return true;

            return false;
        }

        static public bool IsTrackArtistField(Field field)
        {
            if (field >= Field.ArtistTrackName && field < Field.ComposerCDName)
                return true;

            return false;
        }

        static public bool IsTrackComposerField(Field field)
        {
            if (field >= Field.ComposerTrackName)
                return true;

            return false;
        }

        static public bool IsInternalField(Field field)
        {
            if (field == Field.TrackSoundFileLastModified ||
                field == Field.TrackChecksum)
                return true;

            return false;
        }

        /// <summary>
        /// Konvertiert das alte FIELD Enum in das neue
        /// </summary>
        /// <param name="oldFieldEnum"></param>
        /// <returns></returns>
        static public Field GetNewFieldFromOldField(int oldFieldEnum)
        {
            switch (oldFieldEnum)
            {
                case 1: return Field.TotalLength;   //#define FIELD_TOTALLENGTH           1
                case 2: return Field.NumberOfTracks;//#define FIELD_NUMBEROFTRACKS        2
                case 3: return Field.CDSet;         //#define FIELD_CDSET                 3
                case 4: return Field.Sampler;       //#define FIELD_CDSAMPLER             4
                case 5: return Field.ArtistCDName;  //#define FIELD_CDNAME                5
                case 6: return Field.Title;         //#define FIELD_CDTITLE               6
                case 7: return Field.Category;      //#define FIELD_CATEGORY              7
                case 8: return Field.Date;          //#define FIELD_DATE                  8
                case 9: return Field.Codes;         //#define FIELD_CODES                 9
                case 10: return Field.Comment;      //#define FIELD_CDCOMMENT             10
                case 11: return Field.ArchiveNumber;//#define FIELD_ARCHIVNUMMER          11
                case 12: return Field.Medium;       //#define FIELD_MEDIUM                12
                case 13: return Field.YearRecorded; //#define FIELD_YEAR_RECORDED         13     // JUS 17.01.2002
                case 14: return Field.Copyright;    //#define FIELD_COPYRIGHT             14     // JUS 17.01.2002
                case 15: return Field.CDCoverFront; //#define FIELD_CDCOVER_FILENAME      15     // JUS 02.09.2002
                case 16: return Field.CDCoverBack;  //#define FIELD_CDCOVERBACK_FILENAME  16     // JUS 29.08.2003
                case 17: return Field.CDCoverLabel; //#define FIELD_CDCOVERLABEL_FILENAME 17     // JUS 29.08.2003
                case 18: return Field.OriginalCD;   //#define FIELD_ORIGINAL_CD           18     // JUS 29.08.2003
                case 19: return Field.Label;        //#define FIELD_LABEL                 19     // JUS 29.08.2003
                case 20: return Field.UPC;          //#define FIELD_UPC                   20     // JUS 29.08.2003
                case 21: return Field.Homepage;     //#define FIELD_URL                   21     // JUS 29.08.2003
                case 22: return Field.Rating;       //#define FIELD_RATING                22     // JUS 29.08.2003
                case 23: return Field.Price;        //#define FIELD_PRICE                 23     // JUS 29.08.2003
                case 24: return Field.Language;     //#define FIELD_LANGUAGE              24     // JUS 14.12.2005
                case 25: return Field.Location;     //#define FIELD_LOCATION              25     // JUS 14.12.2005

                case 50: return Field.User1;        //#define FIELD_CDUSER1          50
                case 51: return Field.User2;        //#define FIELD_CDUSER2          51
                case 52: return Field.User3;        //#define FIELD_CDUSER3          52
                case 53: return Field.User4;        //#define FIELD_CDUSER4          53
                case 54: return Field.User5;        //#define FIELD_CDUSER5          54

                case 100: return Field.ArtistTrackName; //#define FIELD_TRACK_ARTIST     100
                case 101: return Field.TrackTitle;      //#define FIELD_TRACK_TITLE      101
                case 102: return Field.TrackLength;     //#define FIELD_TRACK_LENGTH     102
                case 103: return Field.TrackNumber;     //#define FIELD_TRACK_NUMBER     103
                case 104: return Field.TrackBpm;        //#define FIELD_TRACK_BPM        104
                case 105: return Field.TrackCodes;      //#define FIELD_TRACK_CODES      105
                case 106: return Field.TrackComment;    //#define FIELD_TRACK_COMMENT    106
                case 109: return Field.TrackLyrics;     //#define FIELD_TRACK_LYRICS     109
                case 110: return Field.TrackSoundFile;  //#define FIELD_TRACK_SOUNDFILE  110
                case 112: return Field.TrackYearRecorded; //#define FIELD_TRACK_YEAR_RECORDED 112    // JUS 17.01.2002
                case 113: return Field.TrackRating;     //#define FIELD_TRACK_RATING        113    // JUS 29.08.2002
                case 114: return Field.TrackChecksum;   //#define FIELD_TRACK_CHECKSUM      114    // JUS 29.08.2002
                case 115: return Field.TrackCategory;   //#define FIELD_TRACK_CATEGORY      115    // JUS 20.12.2005
                case 116: return Field.TrackLanguage;   //#define FIELD_TRACK_LANGUAGE      116    // JUS 20.12.2005

                case 150: return Field.TrackUser1;      //#define FIELD_TRACK_USER1      150
                case 151: return Field.TrackUser2;      //#define FIELD_TRACK_USER2      151
                case 152: return Field.TrackUser3;      //#define FIELD_TRACK_USER3      152
                case 153: return Field.TrackUser4;      //#define FIELD_TRACK_USER4      153
                case 154: return Field.TrackUser5;      //#define FIELD_TRACK_USER5      154

                case 201: return Field.ArtistCDSaveAs;  //#define FIELD_ARTIST_CD_SORTKEY       201
                case 202: return Field.ArtistCDType;    //#define FIELD_ARTIST_CD_GROUPTYPE     202
                case 203: return Field.ArtistCDSex;     //#define FIELD_ARTIST_CD_SEX           203
                case 204: return Field.ArtistCDComment; //#define FIELD_ARTIST_CD_COMMENT       204
                case 205: return Field.ArtistCDHomepage;//#define FIELD_ARTIST_CD_URL           205
                case 206: return Field.ArtistCDCountry; //#define FIELD_ARTIST_CD_COUNTRY       206
                case 207: return Field.ArtistCDDateOfBirth;//#define FIELD_ARTIST_CD_BIRTHDAY      207
                case 208: return Field.ArtistCDDateOfDeath;//#define FIELD_ARTIST_CD_DAYOFDEATH    208
                case 209: return Field.ArtistCDImageFilename;//#define FIELD_ARTIST_CD_IMAGEFILENAME 209

                case 251: return Field.ArtistTrackSaveAs;   //#define FIELD_ARTIST_TRACK_SORTKEY       251
                case 252: return Field.ArtistTrackType;     //#define FIELD_ARTIST_TRACK_GROUPTYPE     252
                case 253: return Field.ArtistTrackSex;      //#define FIELD_ARTIST_TRACK_SEX           253
                case 254: return Field.ArtistTrackComment;  //#define FIELD_ARTIST_TRACK_COMMENT       254
                case 255: return Field.ArtistTrackHomepage; //#define FIELD_ARTIST_TRACK_URL           255
                case 256: return Field.ArtistTrackCountry;  //#define FIELD_ARTIST_TRACK_COUNTRY       256
                case 257: return Field.ArtistTrackDateOfBirth;//#define FIELD_ARTIST_TRACK_BIRTHDAY      257
                case 258: return Field.ArtistTrackDateOfDeath;//#define FIELD_ARTIST_TRACK_DAYOFDEATH    258
                case 259: return Field.ArtistTrackImageFilename;//#define FIELD_ARTIST_TRACK_IMAGEFILENAME 259

                case 300: return Field.ComposerCDName;      //#define FIELD_COMPOSER_CD_NAME          300
                case 301: return Field.ComposerCDSaveAs;    //#define FIELD_COMPOSER_CD_SORTKEY       301
                case 302: return Field.ComposerCDType;      //#define FIELD_COMPOSER_CD_GROUPTYPE     302
                case 303: return Field.ComposerCDSex;       //#define FIELD_COMPOSER_CD_SEX           303
                case 304: return Field.ComposerCDComment;   //#define FIELD_COMPOSER_CD_COMMENT       304
                case 305: return Field.ComposerCDHomepage;  //#define FIELD_COMPOSER_CD_URL           305
                case 306: return Field.ComposerCDCountry;   //#define FIELD_COMPOSER_CD_COUNTRY       306
                case 307: return Field.ComposerCDDateOfBirth; //#define FIELD_COMPOSER_CD_BIRTHDAY      307
                case 308: return Field.ComposerCDDateOfDeath; //#define FIELD_COMPOSER_CD_DAYOFDEATH    308
                case 309: return Field.ComposerCDImageFilename; //#define FIELD_COMPOSER_CD_IMAGEFILENAME 309

                case 350: return Field.ComposerTrackName;       //#define FIELD_COMPOSER_TRACK_NAME          350
                case 351: return Field.ComposerTrackSaveAs;     //#define FIELD_COMPOSER_TRACK_SORTKEY       351
                case 352: return Field.ComposerTrackType;       //#define FIELD_COMPOSER_TRACK_GROUPTYPE     352
                case 353: return Field.ComposerTrackSex;        //#define FIELD_COMPOSER_TRACK_SEX           353
                case 354: return Field.ComposerTrackComment;    //#define FIELD_COMPOSER_TRACK_COMMENT       354
                case 355: return Field.ComposerTrackHomepage;   //#define FIELD_COMPOSER_TRACK_URL           355
                case 356: return Field.ComposerTrackCountry;    //#define FIELD_COMPOSER_TRACK_COUNTRY       356
                case 357: return Field.ComposerTrackDateOfBirth; //#define FIELD_COMPOSER_TRACK_BIRTHDAY      357
                case 358: return Field.ComposerTrackDateOfDeath; //#define FIELD_COMPOSER_TRACK_DAYOFDEATH    358
                case 359: return Field.ComposerTrackImageFilename;//#define FIELD_COMPOSER_TRACK_IMAGEFILENAME 359

                default:
                    System.Diagnostics.Debug.Assert(false);
                    return Field.None;
            }
        }

        /// <summary>
        /// Konvertiert das alte FIELD Enum in das neue
        /// </summary>
        /// <param name="oldFieldEnum"></param>
        /// <returns></returns>
        static public int GetOldFieldFromNewField(Field newFieldEnum)
        {
            switch (newFieldEnum)
            {
                case Field.TotalLength: return 1;   //#define FIELD_TOTALLENGTH           1
                case Field.NumberOfTracks: return 2;//#define FIELD_NUMBEROFTRACKS        2
                case Field.CDSet: return 3;         //#define FIELD_CDSET                 3
                case Field.Sampler: return 4;       //#define FIELD_CDSAMPLER             4
                case Field.ArtistCDName: return 5;  //#define FIELD_CDNAME                5
                case Field.Title: return 6;         //#define FIELD_CDTITLE               6
                case Field.Category: return 7;      //#define FIELD_CATEGORY              7
                case Field.Date: return 8;          //#define FIELD_DATE                  8
                case Field.Codes: return 9;         //#define FIELD_CODES                 9
                case Field.Comment: return 10;      //#define FIELD_CDCOMMENT             10
                case Field.ArchiveNumber: return 11;//#define FIELD_ARCHIVNUMMER          11
                case Field.Medium: return 12;       //#define FIELD_MEDIUM                12
                case Field.YearRecorded: return 13; //#define FIELD_YEAR_RECORDED         13     // JUS 17.01.2002
                case Field.Copyright: return 14;    //#define FIELD_COPYRIGHT             14     // JUS 17.01.2002
                case Field.CDCoverFront: return 15; //#define FIELD_CDCOVER_FILENAME      15     // JUS 02.09.2002
                case Field.CDCoverBack: return 16;  //#define FIELD_CDCOVERBACK_FILENAME  16     // JUS 29.08.2003
                case Field.CDCoverLabel: return 17; //#define FIELD_CDCOVERLABEL_FILENAME 17     // JUS 29.08.2003
                case Field.OriginalCD: return 18;   //#define FIELD_ORIGINAL_CD           18     // JUS 29.08.2003
                case Field.Label: return 19;        //#define FIELD_LABEL                 19     // JUS 29.08.2003
                case Field.UPC: return 20;          //#define FIELD_UPC                   20     // JUS 29.08.2003
                case Field.Homepage: return 21;     //#define FIELD_URL                   21     // JUS 29.08.2003
                case Field.Rating: return 22;       //#define FIELD_RATING                22     // JUS 29.08.2003
                case Field.Price: return 23;        //#define FIELD_PRICE                 23     // JUS 29.08.2003
                case Field.Language: return 24;     //#define FIELD_LANGUAGE              24     // JUS 14.12.2005
                case Field.Location: return 25;     //#define FIELD_LOCATION              25     // JUS 14.12.2005

                case Field.User1: return 50;        //#define FIELD_CDUSER1          50
                case Field.User2: return 51;        //#define FIELD_CDUSER2          51
                case Field.User3: return 52;        //#define FIELD_CDUSER3          52
                case Field.User4: return 53;        //#define FIELD_CDUSER4          53
                case Field.User5: return 54;        //#define FIELD_CDUSER5          54

                case Field.ArtistTrackName: return 100; //#define FIELD_TRACK_ARTIST     100
                case Field.TrackTitle: return 101;      //#define FIELD_TRACK_TITLE      101
                case Field.TrackLength: return 102;     //#define FIELD_TRACK_LENGTH     102
                case Field.TrackNumber: return 103;     //#define FIELD_TRACK_NUMBER     103
                case Field.TrackBpm: return 104;        //#define FIELD_TRACK_BPM        104
                case Field.TrackCodes: return 105;      //#define FIELD_TRACK_CODES      105
                case Field.TrackComment: return 106;    //#define FIELD_TRACK_COMMENT    106
                case Field.TrackLyrics: return 109;     //#define FIELD_TRACK_LYRICS     109
                case Field.TrackSoundFile: return 110;  //#define FIELD_TRACK_SOUNDFILE  110
                case Field.TrackYearRecorded: return 112; //#define FIELD_TRACK_YEAR_RECORDED 112    // JUS 17.01.2002
                case Field.TrackRating: return 113;     //#define FIELD_TRACK_RATING        113    // JUS 29.08.2002
                case Field.TrackChecksum: return 114;   //#define FIELD_TRACK_CHECKSUM      114    // JUS 29.08.2002
                case Field.TrackCategory: return 115;   //#define FIELD_TRACK_CATEGORY      115    // JUS 20.12.2005
                case Field.TrackLanguage: return 116;   //#define FIELD_TRACK_LANGUAGE      116    // JUS 20.12.2005

                case Field.TrackUser1: return 150;      //#define FIELD_TRACK_USER1      150
                case Field.TrackUser2: return 151;      //#define FIELD_TRACK_USER2      151
                case Field.TrackUser3: return 152;      //#define FIELD_TRACK_USER3      152
                case Field.TrackUser4: return 153;      //#define FIELD_TRACK_USER4      153
                case Field.TrackUser5: return 154;      //#define FIELD_TRACK_USER5      154

                case Field.ArtistCDSaveAs: return 201;  //#define FIELD_ARTIST_CD_SORTKEY       201
                case Field.ArtistCDType: return 202;    //#define FIELD_ARTIST_CD_GROUPTYPE     202
                case Field.ArtistCDSex: return 203;     //#define FIELD_ARTIST_CD_SEX           203
                case Field.ArtistCDComment: return 204; //#define FIELD_ARTIST_CD_COMMENT       204
                case Field.ArtistCDHomepage: return 205;//#define FIELD_ARTIST_CD_URL           205
                case Field.ArtistCDCountry: return 206; //#define FIELD_ARTIST_CD_COUNTRY       206
                case Field.ArtistCDDateOfBirth: return 207;//#define FIELD_ARTIST_CD_BIRTHDAY      207
                case Field.ArtistCDDateOfDeath: return 208;//#define FIELD_ARTIST_CD_DAYOFDEATH    208
                case Field.ArtistCDImageFilename: return 209;//#define FIELD_ARTIST_CD_IMAGEFILENAME 209

                case Field.ArtistTrackSaveAs: return 251;   //#define FIELD_ARTIST_TRACK_SORTKEY       251
                case Field.ArtistTrackType: return 252;     //#define FIELD_ARTIST_TRACK_GROUPTYPE     252
                case Field.ArtistTrackSex: return 253;      //#define FIELD_ARTIST_TRACK_SEX           253
                case Field.ArtistTrackComment: return 254;  //#define FIELD_ARTIST_TRACK_COMMENT       254
                case Field.ArtistTrackHomepage: return 255; //#define FIELD_ARTIST_TRACK_URL           255
                case Field.ArtistTrackCountry: return 256;  //#define FIELD_ARTIST_TRACK_COUNTRY       256
                case Field.ArtistTrackDateOfBirth: return 257;//#define FIELD_ARTIST_TRACK_BIRTHDAY      257
                case Field.ArtistTrackDateOfDeath: return 258;//#define FIELD_ARTIST_TRACK_DAYOFDEATH    258
                case Field.ArtistTrackImageFilename: return 259;//#define FIELD_ARTIST_TRACK_IMAGEFILENAME 259

                case Field.ComposerCDName: return 300;      //#define FIELD_COMPOSER_CD_NAME          300
                case Field.ComposerCDSaveAs: return 301;    //#define FIELD_COMPOSER_CD_SORTKEY       301
                case Field.ComposerCDType: return 302;      //#define FIELD_COMPOSER_CD_GROUPTYPE     302
                case Field.ComposerCDSex: return 303;       //#define FIELD_COMPOSER_CD_SEX           303
                case Field.ComposerCDComment: return 304;   //#define FIELD_COMPOSER_CD_COMMENT       304
                case Field.ComposerCDHomepage: return 305;  //#define FIELD_COMPOSER_CD_URL           305
                case Field.ComposerCDCountry: return 306;   //#define FIELD_COMPOSER_CD_COUNTRY       306
                case Field.ComposerCDDateOfBirth: return 307; //#define FIELD_COMPOSER_CD_BIRTHDAY      307
                case Field.ComposerCDDateOfDeath: return 308; //#define FIELD_COMPOSER_CD_DAYOFDEATH    308
                case Field.ComposerCDImageFilename: return 309; //#define FIELD_COMPOSER_CD_IMAGEFILENAME 309

                case Field.ComposerTrackName: return 350;       //#define FIELD_COMPOSER_TRACK_NAME          350
                case Field.ComposerTrackSaveAs: return 351;     //#define FIELD_COMPOSER_TRACK_SORTKEY       351
                case Field.ComposerTrackType: return 352;       //#define FIELD_COMPOSER_TRACK_GROUPTYPE     352
                case Field.ComposerTrackSex: return 353;        //#define FIELD_COMPOSER_TRACK_SEX           353
                case Field.ComposerTrackComment: return 354;    //#define FIELD_COMPOSER_TRACK_COMMENT       354
                case Field.ComposerTrackHomepage: return 355;   //#define FIELD_COMPOSER_TRACK_URL           355
                case Field.ComposerTrackCountry: return 356;    //#define FIELD_COMPOSER_TRACK_COUNTRY       356
                case Field.ComposerTrackDateOfBirth: return 357; //#define FIELD_COMPOSER_TRACK_BIRTHDAY      357
                case Field.ComposerTrackDateOfDeath: return 358; //#define FIELD_COMPOSER_TRACK_DAYOFDEATH    358
                case Field.ComposerTrackImageFilename: return 359;//#define FIELD_COMPOSER_TRACK_IMAGEFILENAME 359

                default:
                    System.Diagnostics.Debug.Assert(false);
                    return -1;
            }
        }

        static public int GetDefaultWidth(Field field)
        {
            switch (field)
            {
                case Field.Title:
                case Field.CDSet:
                case Field.Comment:
                case Field.Copyright:
                case Field.Label:
                case Field.CDCoverFront:
                case Field.CDCoverBack:
                case Field.CDCoverLabel:
                case Field.Homepage:
                case Field.AlbumType:
                case Field.User1:
                case Field.User2:
                case Field.User3:
                case Field.User4:
                case Field.User5:
                case Field.TrackTitle:
                case Field.TrackComment:
                case Field.TrackLyrics:
                case Field.TrackSoundFile:
                case Field.TrackChecksum:
                case Field.TrackUser1:
                case Field.TrackUser2:
                case Field.TrackUser3:
                case Field.TrackUser4:
                case Field.TrackUser5:
                case Field.ArtistCDName:
                case Field.ArtistCDSaveAs:
                case Field.ArtistCDHomepage:
                case Field.ArtistCDComment:
                case Field.ArtistCDImageFilename:
                case Field.ArtistTrackName:
                case Field.ArtistTrackSaveAs:
                case Field.ArtistTrackHomepage:
                case Field.ArtistTrackComment:
                case Field.ArtistTrackImageFilename:
                case Field.ComposerCDName:
                case Field.ComposerCDSaveAs:
                case Field.ComposerCDHomepage:
                case Field.ComposerCDComment:
                case Field.ComposerCDImageFilename:
                case Field.ComposerTrackName:
                case Field.ComposerTrackSaveAs:
                case Field.ComposerTrackHomepage:
                case Field.ComposerTrackComment:
                case Field.ComposerTrackImageFilename:
                    return 200;
                case Field.Category:
                case Field.Medium:
                case Field.Date:
                case Field.Codes:
                case Field.Language:
                case Field.Location:
                case Field.UPC:
                case Field.Identity:
                case Field.TrackCodes:
                case Field.TrackCategory:
                case Field.TrackLanguage:
                case Field.TrackSoundFileLastModified:
                case Field.ArtistCDType:
                case Field.ArtistCDSex:
                case Field.ArtistCDCountry:
                case Field.ArtistCDDateOfBirth:
                case Field.ArtistCDDateOfDeath:
                case Field.ArtistTrackType:
                case Field.ArtistTrackSex:
                case Field.ArtistTrackCountry:
                case Field.ArtistTrackDateOfBirth:
                case Field.ArtistTrackDateOfDeath:
                case Field.ComposerCDType:
                case Field.ComposerCDSex:
                case Field.ComposerCDCountry:
                case Field.ComposerCDDateOfBirth:
                case Field.ComposerCDDateOfDeath:
                case Field.ComposerTrackType:
                case Field.ComposerTrackSex:
                case Field.ComposerTrackCountry:
                case Field.ComposerTrackDateOfBirth:
                case Field.ComposerTrackDateOfDeath:
                case Field.Rating:
                case Field.TrackRating:
                case Field.Created:
                case Field.LastModified:
                    return 100;
                case Field.DiscNumberInCDSet:
                case Field.Sampler:
                case Field.TotalLength:
                case Field.NumberOfTracks:
                case Field.YearRecorded:
                case Field.ArchiveNumber:
                case Field.Price:
                case Field.OriginalCD:
                case Field.TrackLength:
                case Field.TrackNumber:
                case Field.TrackBpm:
                case Field.TrackYearRecorded:
                case Field.TrackPlayCount:
                    return 50;

                default:
                    System.Diagnostics.Debug.Assert(false);
                    return 200;
            }
        }
    }

    public enum SortDirection
    {
        Unknown,
        Ascending,
        Descending
    }

    /// <summary>
    /// Klasse für die Definition einer Sortierung für ein Feld
    /// </summary>
    public class SortField
    {
        public SortField()
        {
        }

        public SortField(Field field) 
            : this(field, SortDirection.Ascending)
        {
        }

        public SortField(Field field, SortDirection sortDirection)
        {
            Field = field;
            SortDirection = sortDirection;
        }

        public Field Field { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public class SortFieldCollection : List<SortField>
    {
        public void Add(Field field)
        {
            this.Add(new SortField(field));
        }

        public FieldCollection GetFields()
        {
            FieldCollection fieldCol = new FieldCollection();

            foreach (SortField field in this)
            {
                fieldCol.Add(field.Field);
            }

            return fieldCol;
        }

        /// <summary>
        /// Liefert die Sortierung als String zurück.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public string GetSortString(DataBase db)
        {
            string sortString = "";

            foreach (SortField sf in this)
            {
                if (!string.IsNullOrEmpty(sortString))
                    sortString += ", ";

                sortString += db.GetNameOfField(sf.Field);

                if (sf.SortDirection == SortDirection.Descending)
                    sortString += " (" + StringTable.Descending + ")";
            }

            return sortString;
        }

        public static SortFieldCollection LoadFromRegistry(string regKey, SortFieldCollection defaultFields)
        {
            try
            {
                object fields = Settings.GetValue(regKey, null);
                if (fields == null)
                    return defaultFields;
                XmlSerializer bf = new XmlSerializer(typeof(SortFieldCollection));
                StringReader sr = new StringReader(fields.ToString());

                return (SortFieldCollection)bf.Deserialize(sr);
            }
            catch
            {
                return defaultFields;
            }
        }

        public void SaveToRegistry(string regKey)
        {
            XmlSerializer bf = new XmlSerializer(GetType());
            StringWriter sw = new StringWriter();
            bf.Serialize(sw, this);

            Settings.SetValue(regKey, sw.ToString());

            sw.Close();
        }
    }

    /// <summary>
    /// Klasse für die Definition einer Spalte in einer Tabelle
    /// </summary>
    public class ColumnField : INotifyPropertyChanged
    {
        public ColumnField()
        {
            TextAlignment = System.Windows.TextAlignment.Left;
        }

        public ColumnField(Field field)
        {
            Field = field;
            Width = FieldHelper.GetDefaultWidth(field);
            TextAlignment = System.Windows.TextAlignment.Left;
        }

        public ColumnField(Field field, int width)
        {
            Field = field;
            Width = width;
            TextAlignment = System.Windows.TextAlignment.Left;
        }

        public ColumnField(Field field, int width, TextAlignment textAlignment)
        {
            Field = field;
            Width = width;
            TextAlignment = textAlignment;
        }

        private Field field;
        public Field Field 
        { 
            get
            {
                return field;
            }
            set
            {
                field = value;

                FirePropertyChanged("Field");
            }
        }

        private int width;

        public int Width
        {
            get { return width; }
            set 
            {
                width = value;

                FirePropertyChanged("Width");
            }
        }

        private TextAlignment textAlignment;

        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set 
            { 
                textAlignment = value;

                FirePropertyChanged("TextAlignment");
            }
        }

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColumnFieldCollection : List<ColumnField>
    {
        public ColumnFieldCollection()
        {
        }

        /// <summary>
        /// Fügt ein neues Feld hinzu und definiert als Spaltenbreite den
        /// Standardwert.
        /// </summary>
        /// <param name="field"></param>
        public void Add(Field field)
        {
            base.Add(new ColumnField(field));
        }

        /// <summary>
        /// Berechnet die Spaltenbreiten auf insgesamt 100%.
        /// </summary>
        /// <returns></returns>
        public float[] GetPercentageWidths()
        {
            float[] percentageWidth = new float[this.Count];
            int totalWidth = 0;

            foreach (ColumnField colField in this)
            {
                totalWidth += colField.Width;
            }

            int i = 0;
            foreach (ColumnField colField in this)
            {
                percentageWidth[i++] = (float)100.0 / (float)totalWidth * (float)colField.Width;
            }

            return percentageWidth;
        }

        /// <summary>
        /// Sucht in der Liste nach der Spalte mit dem angegebenen Feld
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public ColumnField GetColumnField(Field field)
        {
            foreach (ColumnField colField in this)
            {
                if (colField.Field == field)
                    return colField;
            }

            return null;
        }

        public FieldCollection GetFields()
        {
            FieldCollection fieldCol = new FieldCollection();

            foreach (ColumnField colField in this)
            {
                fieldCol.Add(colField.Field);
            }

            return fieldCol;
        }

        public static ColumnFieldCollection LoadFromRegistry(string regKey, ColumnFieldCollection defaultFields)
        {
            object fields = Settings.GetValue(regKey, null);
            if (fields == null)
                return defaultFields;
            XmlSerializer bf = new XmlSerializer(typeof(ColumnFieldCollection));
            using (StringReader sr = new StringReader(fields.ToString()))
            {
                ColumnFieldCollection col = (ColumnFieldCollection)bf.Deserialize(sr);

                if (col.Count == 0)
                    return defaultFields;
                else
                    return col;
            }
        }

        public void SaveToRegistry(string regKey)
        {
            XmlSerializer bf = new XmlSerializer(GetType());
            StringWriter sw = new StringWriter();
            bf.Serialize(sw, this);

            Settings.SetValue(regKey, sw.ToString());

            sw.Close();
        }
    }
}
