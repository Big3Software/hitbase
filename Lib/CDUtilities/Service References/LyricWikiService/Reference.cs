﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Big3.Hitbase.CDUtilities.LyricWikiService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:LyricWiki", ConfigurationName="LyricWikiService.LyricWikiPortType")]
    public interface LyricWikiPortType {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#checkSongExists", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        bool checkSongExists(string artist, string song);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#searchArtists", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        string[] searchArtists(string searchString);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#searchAlbums", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        string[] searchAlbums(string artist, string album, int year);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#searchSongs", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Big3.Hitbase.CDUtilities.LyricWikiService.SongResult searchSongs(string artist, string song);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getSOTD", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Big3.Hitbase.CDUtilities.LyricWikiService.SOTDResult getSOTD();
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getSong", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Big3.Hitbase.CDUtilities.LyricWikiService.LyricsResult getSong(string artist, string song);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getSongResult", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="songResult")]
        Big3.Hitbase.CDUtilities.LyricWikiService.LyricsResult getSongResult(string artist, string song);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getArtist", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void getArtist(ref string artist, out string[] albums);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getAlbum", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void getAlbum(ref string artist, ref string album, ref int year, out string amazonLink, out string[] songs);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#getHometown", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="country")]
        string getHometown(out string state, out string hometown, string artist);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#postArtist", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void postArtist(bool overwriteIfExists, ref string artist, out bool dataUsed, out string message, string[] albums);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#postAlbum", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void postAlbum(bool overwriteIfExists, ref string artist, ref string album, ref int year, out bool dataUsed, out string message, string asin, string[] songs);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#postSong", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void postSong(bool overwriteIfExists, ref string artist, ref string song, out bool dataUsed, out string message, string lyrics, string[] onAlbums);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:LyricWiki#postSong_flags", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        void postSong_flags(bool overwriteIfExists, ref string artist, ref string song, out bool dataUsed, out string message, string lyrics, string[] onAlbums, string flags);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.79.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:LyricWiki")]
    public partial class SongResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string artistField;
        
        private string songField;
        
        /// <remarks/>
        public string artist {
            get {
                return this.artistField;
            }
            set {
                this.artistField = value;
                this.RaisePropertyChanged("artist");
            }
        }
        
        /// <remarks/>
        public string song {
            get {
                return this.songField;
            }
            set {
                this.songField = value;
                this.RaisePropertyChanged("song");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.79.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:LyricWiki")]
    public partial class LyricsResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string artistField;
        
        private string songField;
        
        private string lyricsField;
        
        private string urlField;
        
        /// <remarks/>
        public string artist {
            get {
                return this.artistField;
            }
            set {
                this.artistField = value;
                this.RaisePropertyChanged("artist");
            }
        }
        
        /// <remarks/>
        public string song {
            get {
                return this.songField;
            }
            set {
                this.songField = value;
                this.RaisePropertyChanged("song");
            }
        }
        
        /// <remarks/>
        public string lyrics {
            get {
                return this.lyricsField;
            }
            set {
                this.lyricsField = value;
                this.RaisePropertyChanged("lyrics");
            }
        }
        
        /// <remarks/>
        public string url {
            get {
                return this.urlField;
            }
            set {
                this.urlField = value;
                this.RaisePropertyChanged("url");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.79.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:LyricWiki")]
    public partial class SOTDResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string artistField;
        
        private string songField;
        
        private string nominatedByField;
        
        private string reasonField;
        
        private string lyricsField;
        
        /// <remarks/>
        public string artist {
            get {
                return this.artistField;
            }
            set {
                this.artistField = value;
                this.RaisePropertyChanged("artist");
            }
        }
        
        /// <remarks/>
        public string song {
            get {
                return this.songField;
            }
            set {
                this.songField = value;
                this.RaisePropertyChanged("song");
            }
        }
        
        /// <remarks/>
        public string nominatedBy {
            get {
                return this.nominatedByField;
            }
            set {
                this.nominatedByField = value;
                this.RaisePropertyChanged("nominatedBy");
            }
        }
        
        /// <remarks/>
        public string reason {
            get {
                return this.reasonField;
            }
            set {
                this.reasonField = value;
                this.RaisePropertyChanged("reason");
            }
        }
        
        /// <remarks/>
        public string lyrics {
            get {
                return this.lyricsField;
            }
            set {
                this.lyricsField = value;
                this.RaisePropertyChanged("lyrics");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LyricWikiPortTypeChannel : Big3.Hitbase.CDUtilities.LyricWikiService.LyricWikiPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LyricWikiPortTypeClient : System.ServiceModel.ClientBase<Big3.Hitbase.CDUtilities.LyricWikiService.LyricWikiPortType>, Big3.Hitbase.CDUtilities.LyricWikiService.LyricWikiPortType {
        
        public LyricWikiPortTypeClient() {
        }
        
        public LyricWikiPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LyricWikiPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LyricWikiPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LyricWikiPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool checkSongExists(string artist, string song) {
            return base.Channel.checkSongExists(artist, song);
        }
        
        public string[] searchArtists(string searchString) {
            return base.Channel.searchArtists(searchString);
        }
        
        public string[] searchAlbums(string artist, string album, int year) {
            return base.Channel.searchAlbums(artist, album, year);
        }
        
        public Big3.Hitbase.CDUtilities.LyricWikiService.SongResult searchSongs(string artist, string song) {
            return base.Channel.searchSongs(artist, song);
        }
        
        public Big3.Hitbase.CDUtilities.LyricWikiService.SOTDResult getSOTD() {
            return base.Channel.getSOTD();
        }
        
        public Big3.Hitbase.CDUtilities.LyricWikiService.LyricsResult getSong(string artist, string song) {
            return base.Channel.getSong(artist, song);
        }
        
        public Big3.Hitbase.CDUtilities.LyricWikiService.LyricsResult getSongResult(string artist, string song) {
            return base.Channel.getSongResult(artist, song);
        }
        
        public void getArtist(ref string artist, out string[] albums) {
            base.Channel.getArtist(ref artist, out albums);
        }
        
        public void getAlbum(ref string artist, ref string album, ref int year, out string amazonLink, out string[] songs) {
            base.Channel.getAlbum(ref artist, ref album, ref year, out amazonLink, out songs);
        }
        
        public string getHometown(out string state, out string hometown, string artist) {
            return base.Channel.getHometown(out state, out hometown, artist);
        }
        
        public void postArtist(bool overwriteIfExists, ref string artist, out bool dataUsed, out string message, string[] albums) {
            base.Channel.postArtist(overwriteIfExists, ref artist, out dataUsed, out message, albums);
        }
        
        public void postAlbum(bool overwriteIfExists, ref string artist, ref string album, ref int year, out bool dataUsed, out string message, string asin, string[] songs) {
            base.Channel.postAlbum(overwriteIfExists, ref artist, ref album, ref year, out dataUsed, out message, asin, songs);
        }
        
        public void postSong(bool overwriteIfExists, ref string artist, ref string song, out bool dataUsed, out string message, string lyrics, string[] onAlbums) {
            base.Channel.postSong(overwriteIfExists, ref artist, ref song, out dataUsed, out message, lyrics, onAlbums);
        }
        
        public void postSong_flags(bool overwriteIfExists, ref string artist, ref string song, out bool dataUsed, out string message, string lyrics, string[] onAlbums, string flags) {
            base.Channel.postSong_flags(overwriteIfExists, ref artist, ref song, out dataUsed, out message, lyrics, onAlbums, flags);
        }
    }
}
