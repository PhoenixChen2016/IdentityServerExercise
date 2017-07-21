using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebApplication4.Controllers
{
	public class HomeController : Controller
	{
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}

		public async Task<IActionResult> CallApiUsingUserAccessToken()
		{
			var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");

			var client = new HttpClient();
			client.SetBearerToken(accessToken);
			var content = await client.GetStringAsync("http://localhost:5001/identity");

			ViewBag.Json = JArray.Parse(content).ToString();
			return View("json");
		}
	}
}
