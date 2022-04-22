##### Была задача: Написать WinForm которое отправляет запросы в WebApi, которое было написано в первом задании.

>Для начала я создал проект WinForm.

После чего сделал модуль FuncReverce


```
namespace WinFormsApp
{
    public class FuncRevers
    {
        public string ?ReverseResult { get; set; }
    }
}

```

Далее нужно было реализовать форму. В конструкторе формы я добавил 2 text box и 2 label, убрав последний зависимости от кликов.

После чего добавил кнопку и начал писать обработчик.

Предварительно указав инициализацию 
```
public Form1()
{
    InitializeComponent();
    client = new HttpClient();
}
```
```
private void ButtonReverse_Click(object sender, EventArgs e)
{
    var text = InputBox.Text;

    var strQuery = $"https://localhost:7100/ReverseText/?text={text}";

    var response = client.GetAsync(strQuery).Result;

    var resultStr = response.Content.ReadAsStringAsync().Result;

    var result = JsonConvert.DeserializeObject<FuncRevers>(resultStr);

    ResultBox.Text = result?.ReverseResult.ToString();
}
```

В ходе написания срипта были ошибки с конвертацией json, но в итоге я их решил)