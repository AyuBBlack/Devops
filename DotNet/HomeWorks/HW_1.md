##### Была задача: реализовать что-нибудь из списка дз:  

Я выбрал задачу "Разворачивание строки"

>Для начала я установил .Net 6, а потом VS Code.

После чего в VS Code сделал проект через терминал:

`dotnet new mvc -o WebApp `

Далее я пошел писать свой контроллер для дз под номеро 10.

"10)	Разворачивание строки"

Реализовал я его следующим кодом:

```
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
```

Тут у нас каждый символ передается в массив, потом делается реверс массива и возвращается новый массив.

После чего переданное слово из index файла передается в функцию Reverse и возвращается в ViewData на самой страницу index. Дальше идет решение второй части дз.

Собственно страница index выглядит таким образом:

```
<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Впишите слово</a>.</p>
    <div>
        <form action="WebForm/index">
        <input name="word">
        <input type="submit" class="button" value="Развернуть">
        <br>
        </form>
        <br>
        @ViewData["word"]
    </div>
</div>
```

Тут у нас одна форма в которой имеется 2 инпута, это само слово, что мы вписываем и кнопка отправки Get запроса на сервер.

Ну и в конце возвращается то самое реверсированное слово, которое надо было вернуть. 

Также я немного поменять css для кнопки в файле site.css

```
.button {
  background-color: #4CAF50; /* Green */
  border: none;
  color: white;
  padding: 3px;
  text-align: center;
  text-decoration: none;
  display: inline-block;
  font-size: 16px;
  margin: 4px 2px;
  cursor: pointer;
}
.disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
```

После чего я решил, что не совсем то, что мне нужно и решил сделать просто API

Для этого в Visual Studio 2022 я создал проект 
"ReverseTextApi"

Далее написал тот же контроллер, что и ранее:

```
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
```

Но уже на этот раз добавил модуль "FuncRevers" чтобы была возможность отправлять GET запросы.

namespace ReverseTextApi.Models

```
{
    public class FuncRevers
    {
        public string ReverseResult { get; set; }  
    }
}
```

Далее остовалось это всё затестить, протестировал через SWAGER, а потом и через POSTMAN, чтобы просто посмотреть как работать с POSTMAN)

