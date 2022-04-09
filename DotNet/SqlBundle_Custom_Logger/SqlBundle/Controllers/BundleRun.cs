using Microsoft.AspNetCore.Mvc;
using SqlBundle.Models;


namespace SqlBundle.Controllers

{
    [Route("[controller]")]
    public class BundleRun : Controller
    {
        BundleRepo bundleRep;

/*        private readonly ILogger<BundleRun> _logger;
        public BundleRun(ILogger<BundleRun> logger)
        {
            _logger = logger;
        }*/

        public BundleRun(BundleRepo bundleRepos) => bundleRep = bundleRepos;

        [HttpGet("/GetAllItems")] //Получаем все элементы 

        public IEnumerable<History> Get()
        {
            return bundleRep.Get();
        }
        private static string Reverse(string s) //Метод для реверса строки
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        [HttpGet("/Create")] //Создание записи в БД
        public string Get(string param)
        {
            History tables = new History();
            tables.Date = DateTime.Now.ToString("dd/MM/yy");
            tables.Parametrs = param;
            if (param != null)
            {
                string reverse = Reverse(param);
                tables.Results = reverse;
                bundleRep.Create(tables);
                return reverse;
            }
            else
            {
                return "Введите параметр";
            }
        }

        [HttpPost("/Update")] //Обновление записи 
        public void Update([FromBody] History updateTables)
        {
            updateTables.Date = DateTime.Now.ToString("dd/MM/yy");
            updateTables.Results = Reverse(updateTables.Parametrs);
            bundleRep.Update(updateTables);
        }
        [HttpDelete("/Delete")] // Удаление записи 
        public string Delete(int Id)
        {
            return bundleRep.Delete(Id);
        }
    }
}


