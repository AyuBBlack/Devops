using Newtonsoft.Json;
namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        readonly HttpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
        }

        private void ButtonReverse_Click(object sender, EventArgs e)
        {
            var text = InputBox.Text;

            var strQuery = $"https://localhost:7100/ReverseText/?text={text}";

            var response = client.GetAsync(strQuery).Result;

            var resultStr = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<FuncRevers>(resultStr);

            ResultBox.Text = result?.ReverseResult.ToString();
        }
    }
 }

