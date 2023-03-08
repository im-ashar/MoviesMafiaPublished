using Microsoft.AspNetCore.Mvc;
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
            var httpClient = new HttpClient();
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

