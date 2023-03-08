using Microsoft.AspNetCore.Mvc;
using System.Net;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Services;

namespace MoviesMafia.Controllers
{
    public class AboutUsController : Controller
    {
        
        public IActionResult AboutUs()
        {
            return View();
        }
        public async Task<IActionResult> TestYtMusicAPI()
        {
            var proxy = new WebProxy("144.168.217.88:8780");
            proxy.Credentials = new NetworkCredential("ccaicvwl", "bhatti123");

            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = true
            };
            var httpClient = new HttpClient(httpClientHandler);
            YoutubeMusicSearchClient client = new YoutubeMusicSearchClient(httpClient);

            var responseObject = await client.SearchAsync("krsna");

            Console.WriteLine(responseObject.Results);

            foreach (YoutubeVideo video in responseObject.Results)
            {
                Console.WriteLine(video);
                Console.WriteLine("");
            }

            return BadRequest(responseObject.Results);
        }
    }
}

