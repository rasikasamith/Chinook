using Chinook.Models;

namespace Chinook.Services
{
	public interface IArtistService
	{
		public Task<List<Artist>> GetArtists();
	}
}
