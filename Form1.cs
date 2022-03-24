using Newtonsoft.Json;


namespace WinFormsApp

{
    public partial class Form1 : Form
    {
        HttpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
        }


        private void Button1_ClickAsync(object sender, EventArgs e)
        {
            var word = "";
            var httpClient = new HttpClient();
            var strQuery = $"https://localhost:7043/WebForm?word={word}";
            var resultStr = httpClient.GetStringAsync(strQuery).Result;
            var result = JsonConvert.DeserializeObject<Reverse>(resultStr);

        }
        public void Label1_Click(object sender, EventArgs e)
        {

        }
       
    }
}