//namespace AuthService.Service
//{
//    public class ProductServiceClient
//    {
//        private readonly HttpClient _httpClient;

//        public ProductServiceClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }        

//        // Get products from ProductService
//        public async Task<List<Product>> GetProductsAsync()
//        {
//            var response = await _httpClient.GetAsync("https://localhost:7124/api/Product");

//            if (response.IsSuccessStatusCode)
//            {
//                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
//                return products;
//            }

//            return new List<Product>();
//        }

//    }

//    public class Product
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public decimal Price { get; set; }
//        public string Description { get; set; }
//    }
//}

