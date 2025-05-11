using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Services
{
    public class LLMActionService
    {
        private readonly string apiUrl = "https://openrouter.ai/api/v1/chat/completions";
        private readonly string apiKey = "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";
        private readonly string model = "google/gemini-2.0-flash-thinking-exp:free";

        public async Task<string> GenerateCodeAsync(string userCommand)
        {
            using (var client = new HttpClient())
            {
                // Настроить заголовки запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Подготовить тело запроса
                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                    new
                    {
                        role = "system",
                        content = "Ты – AI-помощник, который преобразует команды пользователя в исполняемый код. Проанализируй команду и сгенерируй код для её выполнения в командной строке (CMD) или на Python. Выведи только код без лишних комментариев и пояснений. Если команда может быть выполнена в CMD, сгенерируй CMD-код, иначе используй Python."
                    },
                    new
                    {
                        role = "user",
                        content = userCommand
                    }
                }
                };

                var jsonBody = JsonConvert.SerializeObject(requestBody);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return responseJson.choices[0].message.content;
                }
                else
                {
                    throw new Exception("Ошибка при отправке запроса: " + response.ReasonPhrase);
                }
            }
        }
            public async Task ExecuteGeneratedCodeAsync(string code)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Ошибка: нет кода для выполнения.");
                    return;
                }

                // Если это CMD код
                if (code.StartsWith("```cmd") || code.Contains("mkdir") || code.Contains("echo") || code.Contains("del"))
                {
                    await ExecuteCmdAsync(code);
                }
                // Если это Python код
                else if (code.Contains("```python") || code.Contains("import"))
                {
                    await ExecutePythonAsync(code);
                }
                else
                {
                    Console.WriteLine("Ошибка: неподдерживаемый формат кода.");
                }
        }

        private async Task ExecuteCmdAsync(string cmdCode)
        {
            try
            {
                cmdCode = cmdCode.Replace("```cmd","");
                cmdCode = cmdCode.Replace("```", "");
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {cmdCode}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = await reader.ReadToEndAsync();
                        Console.WriteLine("Результат выполнения команды:\n" + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении CMD команды: " + ex.Message);
            }
        }

        private async Task ExecutePythonAsync(string pythonCode)
        {
            try
            {
                // Сохраняем Python код во временный файл
                string tempFilePath = Path.Combine(Path.GetTempPath(), "generated_script.py");
                await File.WriteAllTextAsync(tempFilePath, pythonCode);

                // Запуск Python скрипта
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "python", // Убедитесь, что Python установлен и добавлен в PATH
                    Arguments = tempFilePath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = await reader.ReadToEndAsync();
                        Console.WriteLine("Результат выполнения Python кода:\n" + result);
                    }
                }

                // Удаляем временный файл после выполнения
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении Python скрипта: " + ex.Message);
            }
    }
}
}
