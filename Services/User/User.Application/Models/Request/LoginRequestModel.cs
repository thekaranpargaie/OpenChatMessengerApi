namespace User.Application.Models.Request
{
    public record LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
