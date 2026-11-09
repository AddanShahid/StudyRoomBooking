using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    // Page to return to after login
    public string? ReturnUrl { get; set; }
}