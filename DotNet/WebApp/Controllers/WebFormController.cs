using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class WebFormController : Controller
{
    private readonly ILogger<WebFormController> _logger;

    public WebFormController(ILogger<WebFormController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index(string word = "")
    {
        static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        ViewData["word"] = Reverse(word);
        return View();
    }
}
