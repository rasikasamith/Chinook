using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Chinook.Models
{
    //Added by Rasika Samith
    public class PlaylistTrack
    {
            
        public long PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        
        public long TrackId { get; set; }
        public Track Track { get; set; }
    }
}
