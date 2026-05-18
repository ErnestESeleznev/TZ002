namespace WebApi.Library
{
    public interface IMyService
    {
        Task<string> GetDataAsync(string link);
    }

    public class MyService : IMyService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetDataAsync(string link)
        {
            HttpClient client = _httpClientFactory.CreateClient("MyApiClient");
            HttpResponseMessage response = await client.GetAsync(link);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            throw new HttpRequestException("Failed to get data");
        }
    }
}
