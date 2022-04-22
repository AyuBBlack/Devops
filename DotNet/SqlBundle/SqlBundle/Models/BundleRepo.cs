namespace SqlBundle.Models
{
    public class BundleRepo
    {
        private SqlContext Context;

        public void Create(History tables) //Создание таблицы
        {
            Context.Tables.Add(tables);
            Context.SaveChanges();
        }

        public BundleRepo(SqlContext context)
        {
            Context = context;
        }
        public IEnumerable<History> Get() //Получение таблиц
        {
            return Context.Tables;
        }
        public History Get(int Id) 
        {
            return Context.Tables.Find(Id); //Поиск id в таблице
        }
        public void Update(History updatetables) //Обновление тело таблицы
        {
            History curretTables   = Get(updatetables.Id);
            curretTables.Date      = updatetables.Date;
            curretTables.Parametrs = updatetables.Parametrs;
            curretTables.Results   = updatetables.Results;

            Context.Tables.Update(curretTables);
            Context.SaveChanges();
        }

        public string Delete(int Id) //Удаление таблицы
        {
            History history = Context.Tables.Find(Id);
            if (history != null)
            {
                Context.Remove(history);
                Context.SaveChanges();
                return "Ok";
            }
            else
                return "Error";
        }
    }
}

