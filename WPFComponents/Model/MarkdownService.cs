using Markdig;

namespace WPFComponents.Model
{
    public static class MarkdownService
    {
        public static string TestMarkdownShow()
        {
            string markdownText = "# Заголовок первого уровня\r\n\r\nЭто пример текста в формате Markdown. Ниже приведен пример кода на Python:\r\n\r\n```python\r\ndef greet(name):\r\n    print(f\"Привет, {name}!\")\r\n\r\ngreet(\"Мир\")";
            string htmlText = Markdown.ToHtml(markdownText);

            return $"<html><head><meta charset=\"utf-8\"></head><body>{htmlText}</body></html>";

        }
    }
}
