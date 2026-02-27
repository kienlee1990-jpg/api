namespace FastFoodAPI.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime Expiration { get; set; }   // ⭐ thêm dòng này
    }
}
