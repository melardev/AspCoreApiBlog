using System.ComponentModel.DataAnnotations;

namespace BlogDotNet.Dtos.Requests.Auth
{
    public class ExternalLoginConfirmationViewModel
    {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
