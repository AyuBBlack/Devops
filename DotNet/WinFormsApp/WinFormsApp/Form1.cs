using Newtonsoft.Json;
namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        private readonly ReverseService _ReverseService = new();
        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonReverse_Click(object sender, EventArgs e)
        {
            var text = InputBox.Text;

            var result = await _ReverseService.GetReverseText(text);

            var resultConvertJson = JsonConvert.DeserializeObject<ReverseService>(result);

            ResultBox.Text = resultConvertJson.ToString();
        }
    }
}