using Chinook.Models;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chinook.Pages
{
	public partial class ArtistPage
    {
        [Parameter] public long ArtistId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }
        [Inject] IDbContextFactory<ChinookContext> DbFactory { get; set; }
        private Modal PlaylistDialog { get; set; }

        private Artist Artist;
        private List<ClientModels.PlaylistTrack> Tracks;
        //private DbContext DbContext;
        private ClientModels.PlaylistTrack SelectedTrack;
        private string InfoMessage;
        private string CurrentUserId;

        //Added by Rasika Samith
        private string ?newPlayList=null;
        private List<Models.Playlist> existingPlayList = new List<Models.Playlist>();
        private int selectedPlaylistId;
        private string ?selectedPlaylistName;

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            CurrentUserId = await GetUserId();
            var DbContext = await DbFactory.CreateDbContextAsync();

            Artist = DbContext.Artists.SingleOrDefault(a => a.ArtistId == ArtistId);

            Tracks = DbContext.Tracks.Where(a => a.Album.ArtistId == ArtistId)
                .Include(a => a.Album)
                .Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
					IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites")).Any()
					
				})
                .ToList();

            //Added by Rasika Samith
            existingPlayList=DbContext.Playlists.ToList();
            List<TrackFavorite> favoritTackList = new List<TrackFavorite>();
            favoritTackList = DbContext.TrackFavorites.Where(tf => tf.UserId == CurrentUserId).ToList();
			foreach (var item in Tracks)
            {
                foreach (var ft in favoritTackList)
                {
                    if(item.TrackId==ft.TrackId)
                    {
                        item.IsFavorite = true;
                    }
                }
            }            
        }

        private async Task<string> GetUserId()
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        //Added by Rasika Samith
        private void FavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId) ;
            //Added by Rasika Samith
            track.IsFavorite = true;           

            TrackFavorite trackFavorite=new TrackFavorite() { TrackId= trackId , UserId= CurrentUserId };
            addFavoriteTrack(trackFavorite);

            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
        }
        //Added by Rasika Samith
        private async Task addFavoriteTrack(TrackFavorite trackFavorite)
        {
            var dbContext = await DbFactory.CreateDbContextAsync();
            await dbContext.TrackFavorites.AddAsync(trackFavorite);
            await dbContext.SaveChangesAsync();
        }

       

        private void UnfavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            //Added by Rasika Samith
            track.IsFavorite = false;

            TrackFavorite trackFavorite = new TrackFavorite() { TrackId = trackId, UserId = CurrentUserId };
            removeFavoriteTrack(trackFavorite);

            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        //Added by Rasika Samith
        private async Task removeFavoriteTrack(TrackFavorite trackFavorite)
        {
            var dbContext = await DbFactory.CreateDbContextAsync();
            dbContext.TrackFavorites.Remove(trackFavorite);
            await dbContext.SaveChangesAsync();
        }

        private void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);           
            PlaylistDialog.Open();
        }

        private async void AddTrackToPlaylist()
        {
            //Added by Rasika Samith - Create new playlist    
            var dbContext = await DbFactory.CreateDbContextAsync();
            long playListId;

           if (newPlayList is not null)
           {
                //Check entered playlist is exist or not 
                bool isDuplicate =await dbContext.Playlists.AnyAsync(x => x.Name == newPlayList);

				if(isDuplicate==false) 
                { 
                    var data = await createNewPlaylist(newPlayList);
                    playListId = data.PlaylistId;

				}
                else
                {
                   var row=await dbContext.Playlists.Where(x => x.Name == newPlayList).FirstOrDefaultAsync();
                   playListId = row.PlaylistId;
				}
                
                //Add track to new playlist  
                
                bool isExist =await dbContext.PlaylistTracks.AnyAsync(x => (x.PlaylistId == playListId) && (x.TrackId == SelectedTrack.TrackId));

				if (!isExist)
                {
					Models.PlaylistTrack playlistTrack = new Models.PlaylistTrack();
					playlistTrack.PlaylistId = playListId;
					playlistTrack.TrackId = SelectedTrack.TrackId;
					await dbContext.PlaylistTracks.AddAsync(playlistTrack);
					await dbContext.SaveChangesAsync();
					CloseInfoMessage();
					InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {newPlayList}.";
					PlaylistDialog.Close();
				}
                else
                {
					CloseInfoMessage();
					InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} is already exist in playlist {newPlayList}.";
					PlaylistDialog.Close();
				}

                newPlayList = null;
            }
           else
           {
                //track to existing playList
                //Add track to existing playlist
                //get playlist name from drop down and insert record to playlistTrack
                Models.PlaylistTrack playlistTrack = new Models.PlaylistTrack();
                playlistTrack.PlaylistId = selectedPlaylistId;
                playlistTrack.TrackId = SelectedTrack.TrackId;
                selectedPlaylistName = await dbContext.Playlists.Where(x => x.PlaylistId == selectedPlaylistId).Select(x => x.Name).FirstOrDefaultAsync();

                bool isExist=await dbContext.PlaylistTracks.Where(x => (x.PlaylistId == selectedPlaylistId) && (x.TrackId == SelectedTrack.TrackId)).AnyAsync();
                if (!isExist)
                {                    
                    await dbContext.PlaylistTracks.AddAsync(playlistTrack);
                    await dbContext.SaveChangesAsync();                   

                    CloseInfoMessage();
                    InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {selectedPlaylistName}.";
                    PlaylistDialog.Close();
                }
                else
                {
                    CloseInfoMessage();
                    InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} already added to playlist {selectedPlaylistName}.";
                    PlaylistDialog.Close();
                } 
               
            }           
        }

        private void CloseInfoMessage()
        {
            InfoMessage = "";
        }

        //Added by Rasika Samith - Create new playlist
        private async Task<Models.Playlist> createNewPlaylist(string playListName)
        {  
            var dbContext = await DbFactory.CreateDbContextAsync();
            var data=dbContext.Playlists.Where(a => a.Name == playListName).FirstOrDefault();
            if (data is not null)
            {
                //Alread exist name of playlist
                return data;
			}
            else
            {
				Chinook.Models.Playlist playlist = new Chinook.Models.Playlist();                
                playlist.PlaylistId = genNextId();
                playlist.Name = playListName;                

				await dbContext.Playlists.AddAsync(playlist);
				await dbContext.SaveChangesAsync();
                await RefreshPlaylistData();
                return playlist;
			}          
        }

        //Added by Rasika Samith - Create new playlist
        private int genNextId()
        {
            var dbContext = DbFactory.CreateDbContext();

            var id = (from a in dbContext.Playlists
                      select a.PlaylistId).Max();

            return Convert.ToInt32(id)+1;
        }

        //Added by Rasika Samith-To refresh Playlist drop down after press save button
        private async Task RefreshPlaylistData()
        {
            var dbContext = await DbFactory.CreateDbContextAsync();
            existingPlayList = await dbContext.Playlists.ToListAsync();
            StateHasChanged(); 
        }
    }
}
