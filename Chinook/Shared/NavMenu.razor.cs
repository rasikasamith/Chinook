using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Shared
{
    public partial class NavMenu
    {
        [Inject] IDbContextFactory<ChinookContext> DbFactory { get; set; }
        private bool collapseNavMenu = true;
        private List<PlayListMenu> PlayListsForMenu = new List<PlayListMenu>();

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        //Added by Rasika Samith
        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            await getPlaylists();
        }

        private async Task getPlaylists()
        {
            var dbContext = await DbFactory.CreateDbContextAsync();            
            var data = dbContext.Playlists.ToList();
            foreach (var item in data)
            {
                PlayListsForMenu.Add(new PlayListMenu() { PlayListId = item.PlaylistId, Name = item.Name });
            }            
        }
    }
}
