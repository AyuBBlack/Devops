
namespace WinFormsApp

{
    internal class ReverseService
    {
        public async Task<string> GetReverseText(string text)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync($"https://localhost:7100/ReverseText?text={text}");
            return await result.Content.ReadAsStringAsync();
        }

    }
}


