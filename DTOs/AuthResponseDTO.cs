namespace E_CommerceSystem.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime Expiration { get; set; }
    }
}
