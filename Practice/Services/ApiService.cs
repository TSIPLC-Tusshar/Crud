namespace Practice.Services
{
    public class ApiService(IHttpClientFactory clientFactory)
    {
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers)
        {
            HttpClient client = clientFactory.CreateClient();
            if (headers != null && headers.Count > 0)
            {
                foreach (var headerItem in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(headerItem.Key, headerItem.Value);
                }
            }

            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> SendAsync(string url, HttpMethod method, Dictionary<string, string> headers = null, string json = null)
        {
            HttpClient client = clientFactory.CreateClient("httpClient");
            if (headers != null && headers.Count > 0)
            {
                foreach (var headerItem in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(headerItem.Key, headerItem.Value);
                }
            }

            HttpRequestMessage httpRequest = new HttpRequestMessage(method, url);
            if(json != null)
            {
                httpRequest.Content = new StringContent(json, encoding: System.Text.Encoding.UTF8, "application/json");
            }

            return await client.SendAsync(httpRequest);
        }
    }
}
