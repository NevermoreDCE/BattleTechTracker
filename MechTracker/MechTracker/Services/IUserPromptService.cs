namespace MechTracker.Services
{
    public interface IUserPromptService
    {
        Task<string> ShowActionSheet(string title, string cancel, string? destruction, params string[] buttons);
        Task<bool> ShowAlert(string title, string message, string accept, string cancel);
        Task<int> ShowDamagePickerModal();
    }
}
