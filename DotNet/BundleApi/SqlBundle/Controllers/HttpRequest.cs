using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SqlBundle.Models;

namespace SqlBundle.Controllers

{
    [Route("[controller]")]
    public class HttpRequest : Controller
    {
        readonly ChangeDB getChange;
        public HttpRequest(ChangeDB change) => getChange = change;

        [HttpGet("/GetAllItems")] //Получаем все элементы 

        public IEnumerable<History> GetAll()
        {
            return getChange.Get();
        }
        private static string Reverse(string s) //Метод для реверса строки
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        [HttpPost("/Create")] //Создание записи в БД
        public string Get(string param)
        {
            History tables = new()
            {
                Date = DateTime.Now.ToString("dd/MM/yy"),
                Parametrs = param
            };
            if (param != null)
            {
                string reverse = Reverse(param);
                tables.Results = reverse;
                getChange.Create(tables);
                return reverse;
            }
            else
            {
                return "Введите параметр";
            }
        }

        [HttpPut("/Update")] //Обновление записи 
        public void Update([FromBody] History updateTables)
        {
            updateTables.Date = DateTime.Now.ToString("dd/MM/yy");
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            updateTables.Results = Reverse(updateTables.Parametrs);
            getChange.Update(updateTables);
        }
        [HttpDelete("/Delete")] // Удаление записи 
        public string Delete(int Id)
        {
            return getChange.Delete(Id);
        }

        readonly Counter counter = Metrics.CreateCounter("my_counter", "index page counter");

        [HttpGet]
        public string Get()
        {
            counter.Inc();
            return "hello";
        }
    }
}