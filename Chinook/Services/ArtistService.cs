using Chinook.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
	public class ArtistService : IArtistService
	{
		[Inject] IDbContextFactory<ChinookContext> DbFactory { get; set; }

		public async Task<List<Artist>> GetArtists()
		{
			var dbContext = await DbFactory.CreateDbContextAsync();
			var users = dbContext.Users.Include(a => a.UserPlaylists).ToList();

			//Changed by Rasika Samith 
			//return dbContext.Artists.ToList();
			return dbContext.Artists.Include(a => a.Albums).ToList();
		}
	}
}
