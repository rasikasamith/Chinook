namespace Chinook.Pages
{
    using Chinook.ClientModels;    
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;

    public partial class Favorite
    {
        [Inject] IDbContextFactory<ChinookContext> DbFactory { get; set; }
		[CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }
		private List<ClientModels.PlaylistTrack> tracks;
        private string currentUserId;        
        public List<FavoriteTrack> favoriteTracks = new List<FavoriteTrack>();
		

		protected override async Task OnInitializedAsync()
        {
            var DbContext = await DbFactory.CreateDbContextAsync();
            currentUserId = await getUserId();

			var result = (from t in DbContext.Tracks
						 join tf in DbContext.TrackFavorites
						 on t.TrackId equals tf.TrackId
						 join a in DbContext.Albums
						 on t.AlbumId equals a.AlbumId
						 where tf.UserId == currentUserId
						 select new
						 {
							 tf.TrackId,
							 t.Name,
							 a.Title
						 }).ToList();

			foreach (var item in result)
			{
				favoriteTracks.Add(new FavoriteTrack() { TrackId = item.TrackId, TrackName = item.Name, AlbumTitle = item.Title });
			}

        }

        private async Task<string> getUserId()
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }
    }
}
