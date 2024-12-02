//namespace ProductService.Service
//{
//    public class AuthServiceClient
//    {
//        private readonly HttpClient _httpClient;

//        public AuthServiceClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<bool> ValidateTokenAsync(string token)
//        {
//            var response = await _httpClient.GetAsync($"http://localhost:5262/api/auth/validate?token={token}");
//            return response.IsSuccessStatusCode;
//        }
//    }
//}
