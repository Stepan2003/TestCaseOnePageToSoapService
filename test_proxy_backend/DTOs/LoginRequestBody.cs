namespace test_proxy_backend.DTOs
{
    public class LoginRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? IPs { get; set; } = string.Empty; 
    }
}
