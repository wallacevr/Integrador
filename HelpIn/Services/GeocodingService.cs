using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HelpIn.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;

        public GeocodingService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesByCep(string cep)
        {
            string url = $"https://nominatim.openstreetmap.org/search?postalcode={cep}&country=Brazil&format=json";

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HelpInApp/1.0");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao consultar geolocalização.");

            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<NominatimResponse>>(json);

            if (results != null && results.Count > 0)
            {
                double lat = double.Parse(results[0].lat, System.Globalization.CultureInfo.InvariantCulture);
                double lon = double.Parse(results[0].lon, System.Globalization.CultureInfo.InvariantCulture);

                return (lat, lon);
            }

            throw new Exception("Coordenadas não encontradas.");
        }

        private class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
    }
}
