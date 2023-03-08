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
        
    }
}

