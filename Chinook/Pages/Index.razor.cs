using Chinook.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Pages
{
	public partial class Index
	{
        private List<Artist> Artists;
        [Inject] IDbContextFactory<ChinookContext> DbFactory { get; set; }

        //Added by Rasika Samith
        private List<Artist> filteredArtists;        


        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            //Changed by Rasika Samith
            Artists = await GetArtists();            
        }

        public async Task<List<Artist>> GetArtists()
        {
            var dbContext = await DbFactory.CreateDbContextAsync();
            var users = dbContext.Users.Include(a => a.UserPlaylists).ToList();
                       

            //Changed by Rasika Samith 
            //return dbContext.Artists.ToList();
            return dbContext.Artists.Include(a=>a.Albums).ToList();
		}

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            var dbContext = await DbFactory.CreateDbContextAsync();
            return dbContext.Albums.Where(a => a.ArtistId == artistId).ToList();
        }

        //Added by Rasika Samith 
        public async Task<List<Artist>> SearchArtistsByName(string artistName )
        {
			var dbContext = await DbFactory.CreateDbContextAsync();

            //var fillterdMovie = allMovies.Where(x => x.Name.ToLower().Contains(searchString.ToLower())||x.Description.ToLower().Contains(searchString.ToLower())).ToList();
            //return dbContext.Artists.Include(a => a.Albums).ContainsAsync(a=>a.artistName)

            return await dbContext.Artists.Where(s => s.Name.ToLower().Contains(artistName.ToLower())).Include(a => a.Albums).ToListAsync();

		}

        //Added by Rasika Samith 
        public async void searchArtists(string searchTerm)
        {
            var dbContext = DbFactory.CreateDbContext();
            if (string.IsNullOrEmpty(searchTerm))
            {
                Artists = await GetArtists();                
            }
            else
            {              
                Artists = await dbContext.Artists.Where(a => a.Name.Contains(searchTerm)).Include(a => a.Albums).ToListAsync();                
            }
        }
    }
}
