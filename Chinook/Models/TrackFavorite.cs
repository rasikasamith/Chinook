using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Chinook.Models
{
    public class TrackFavorite
    {
        
        public string UserId { get; set; }
        
        public long TrackId { get; set; } 
        public Track Track { get; set; }
    }
}
