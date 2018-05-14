using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Big3.Hitbase.SoundEngine2
{
    public class SoundFileInformation
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Composer { get; set; }
        public string Comment { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public int TrackNumber { get; set; }
        public int Rating { get; set; }
        public string Language { get; set; }
        public int BPM { get; set; }
        public int Length { get; set; }
        public string Filename { get; set; }
        public List<byte[]> Images { get; set; }

        /// <summary>
        /// Die Version der ID3-Tags (0 = keine gefunden, 1 = ID3v1, 2 = ID3v2)
        /// </summary>
        public int ID3Version { get; set; }
    }
}
