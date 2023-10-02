namespace IUstaApi.Models.DTOs.Admin
{
    public class AppUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
