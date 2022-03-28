using Microsoft.AspNetCore.Mvc;
using SqlBundle.Models;

namespace SqlBundle.Controllers

{
    [Route("[controller]")]
    public class BundleRun : Controller
    {
        BundleRepo bundleRep;
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
            string reverse = Reverse(param);
            tables.Results = reverse;
            bundleRep.Create(tables);
            return reverse;
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


/* [HttpPost(Name = "Text")]
 public void TextReverse([FromBody] string parameter, string id)
 {
     Tables tables = new Tables();
     tables.Id = Int32.Parse(id);
     tables.Parametrs = parameter;
     tables.Results = Reverse(parameter);
     bundleRep.Update(tables);
 }*/

/*        [HttpPost("/Create")]
        public void Create([FromBody] History tables)
        {
            tables.Results = Reverse(tables.Parametrs);
            bundleRep.Create(tables);
        }*/
