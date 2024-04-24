using System.Text;
namespace FilleEngToVN
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //get path
            string inputPath = ".\\input";
            string outPath = ".\\output";

            //get files
            string[] filenames = Directory.GetFiles(inputPath);

            StringBuilder errLog = new StringBuilder();

            //translate file
            try
            {
                foreach (string filename in filenames)
                {
                    //check if not file need to translate then skip
                    if (!filename.Contains(".subtitle.en.txt"))
                    {
                        continue;
                    }

                    //get data
                    Encoding encoding = Encoding.UTF8;
                    string lines = File.ReadAllText(filename, encoding);
                    string targetLanguage = "vi"; // Vietnamese language code

                    string output = await TranslateText(lines, targetLanguage);
                    if (output.Length == 0)
                    {
                        errLog.AppendLine(filename + " : " + output);
                        continue;
                    }

                    string[] outfilename = Path.GetFileName(filename).Split(".");
                    if (!PrintData(outPath + "\\" + outfilename[0] + ".subtitle.txt", output))
                    {
                        errLog.AppendLine(filename);
                        continue;
                    }
                }

                if (errLog.Length > 0)
                {
                    if (!PrintData(outPath + "\\errLog.txt", errLog.ToString()))
                    {
                        throw new Exception(errLog.ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static bool PrintData(string path, string text)
        {
            try
            {
                using (FileStream file = File.Create(path))
                {

                }

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(text);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static async Task<string> TranslateText(string text, string targetLanguage)
        {
            try
            {
                string url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair=en|{targetLanguage}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                    string translatedText = result.responseData.translatedText;

                    return translatedText;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}