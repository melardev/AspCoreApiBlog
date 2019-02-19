namespace BlogDotNet.Dtos.Requests.Auth
{
    public class RegisterDtoRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}