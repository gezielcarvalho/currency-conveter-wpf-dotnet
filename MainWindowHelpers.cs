using currency_converter_wpf_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

internal static class MainWindowHelpers
{

    // Rate update url
    private static readonly string RateUpdateUrl = "https://openexchangerates.org/api/latest.json";

    public static async Task<RateUpdateDto> GetRatesAsync()
    {
        var myRateUpdate = new RateUpdateDto();
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            HttpResponseMessage response = await client.GetAsync(RateUpdateUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                myRateUpdate = JsonConvert.DeserializeObject<RateUpdateDto>(content);
            }
        }
        return myRateUpdate;
    }
}