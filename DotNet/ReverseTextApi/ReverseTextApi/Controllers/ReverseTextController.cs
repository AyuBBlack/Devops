using Microsoft.AspNetCore.Mvc;
using ReverseTextApi.Models;

namespace ReverseTextApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReverseTextController
    {
        [HttpGet(Name = "GetFuncRevers")]
        public FuncRevers Get(string text)
        {
            return new FuncRevers
            { 
            ReverseResult = Reverse(text)
            };
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
