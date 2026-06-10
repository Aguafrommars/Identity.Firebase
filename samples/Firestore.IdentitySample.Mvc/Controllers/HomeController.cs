// Project: aguacongas/Identity.Firebase
// Copyright (c) 2026 @Olivier Lefebvre
using Microsoft.AspNetCore.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}