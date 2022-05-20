namespace SqlBundle.Models
{
    public class ChangeDB
    {
        private readonly SqlContext Context;

        public void Create(History tables) //Создание таблицы
        {
            Context.Tables.Add(tables);
            Context.SaveChanges();
        }

        public ChangeDB(SqlContext context)
        {
            Context = context;
        }
        public IEnumerable<History> Get() //Получение таблиц
        {
            return Context.Tables;
        }
        #pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        public History Get(int Id) => Context.Tables.Find(Id); //Поиск id в таблице

        public void Update(History updatetables) //Обновление тело таблицы
        {
            History curretTables = Get(updatetables.Id);
            curretTables.Date = updatetables.Date;
            curretTables.Parametrs = updatetables.Parametrs;
            curretTables.Results = updatetables.Results;
            Context.Tables.Update(curretTables);
            Context.SaveChanges();
        }

        public string Delete(int Id) //Удаление таблицы
        {
            #pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
            History history = Context.Tables.Find(Id);
            if (history != null)
            {
                Context.Remove(history);
                Context.SaveChanges();
                return "Ok";
            }
            else
            {
                return "Error";
            }
        }
    }
}

