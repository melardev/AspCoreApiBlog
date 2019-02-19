using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlogDotNet.ViewModels.Account
{
  public class LoginDtoRequest
  {
    [Required]
    [JsonProperty(PropertyName = "username")]
    public string UserName { get; set; }
    [Required]
    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
  }
}
