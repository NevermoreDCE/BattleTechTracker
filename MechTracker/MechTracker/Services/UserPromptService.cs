using System.Threading.Tasks;
using MechTracker.Views;
using Microsoft.Maui.Controls;

namespace MechTracker.Services
{
    public class UserPromptService : IUserPromptService
    {
        private static Page GetCurrentPage()
        {
            // Gets the current MainPage or the topmost modal page
            Page? page = Application.Current?.MainPage;
            while (page is NavigationPage nav && nav.CurrentPage != null)
                page = nav.CurrentPage;

            // Ensure the method does not return null
            if (page == null)
            {
                throw new InvalidOperationException("Unable to determine the current page.");
            }

            return page;
        }

        public async Task<string> ShowActionSheet(string title, string cancel, string? destruction, params string[] buttons)
        {
            Page page = GetCurrentPage();
            return await page.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task<bool> ShowAlert(string title, string message, string accept, string cancel)
        {
            Page page = GetCurrentPage();
            return await page.DisplayAlert(title, message, accept, cancel);
        }
        public async Task<int> ShowDamagePickerModal()
        {
            var page = GetCurrentPage();
            var modal = new DamagePickerModal();
            await page.Navigation.PushModalAsync(modal);
            return await modal.Result;
        }

    }
}