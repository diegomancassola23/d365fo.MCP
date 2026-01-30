using d365fo.MCP.Base;

using System.Configuration; // Import per leggere il file di configurazione

namespace d365fo.MCP.Tools
{
    public class GetOrCreateLabelToolInput : ToolInputBase
    {
        public string? Language { get; set; }
        public string? Comment { get; set; }
        public string? Text { get; set; }
        public string? LabelPrefix { get; set; } // Aggiunta proprietà per il prefisso
        public bool ForceNewLabel { get; set; } // New parameter to force label creation
        public List<GetOrCreateLabelToolTransaltions>? Translations { get; set; }

        public new bool Validate(out string validationError)
        {
            base.Validate(out validationError);

            if (string.IsNullOrWhiteSpace(Text))
            {
                validationError = "Text is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Language) || Language != "it")
            {
                validationError = "Primary language must be Italian (it).";
                return false;
            }

            if (Translations == null || !Translations.Any(t => t.Language == "en-us"))
            {
                validationError = "Translations must include English (en-us).";
                return false;
            }

            if (Translations.Any(lang => string.IsNullOrWhiteSpace(lang.Language) || string.IsNullOrWhiteSpace(lang.Text)))
            {
                validationError = "All additional languages must have non-empty language codes and texts.";
                return false;
            }

            validationError = string.Empty;
            return true;
        }
    }

    public class GetOrCreateLabelToolTransaltions
    {
        public string Language { get; set; }
        public string Text { get; set; }
    }

    public class GetOrCreateLabelToolResult : ToolResultBase
    {
        public string? LabelCode { get; set; }
        public string? Text { get; set; }
    }

    public class GetOrCreateLabelTool : BaseTool
    {
        // Cache configurazione
        private static readonly string aosServicePath = ConfigurationManager.AppSettings["aosServicePath"] ?? string.Empty;
        private static readonly string appConfigModel = ConfigurationManager.AppSettings["model"] ?? string.Empty;
        private static readonly string appConfigLabelsModel = ConfigurationManager.AppSettings["labelsModel"] ?? appConfigModel;
        private static readonly string appConfigLabelFileName = ConfigurationManager.AppSettings["labelFileName"] ?? string.Empty;

        public static GetOrCreateLabelToolResult[] Execute(GetOrCreateLabelToolInput[] input)
        {
            List<GetOrCreateLabelToolResult> results = new();

            foreach (var item in input)
            {
                if (!item.Validate(out string validationError))
                {
                    results.Add(new GetOrCreateLabelToolResult { Success = false, ErrorMessage = validationError });
                    continue;
                }

                try
                {

                    string primaryFilePath = GetPrimaryItalianFilePath(appConfigLabelsModel);
                    if (!File.Exists(primaryFilePath))
                    {
                        results.Add(new GetOrCreateLabelToolResult { Success = false, ErrorMessage = $"File etichette non trovato per la lingua 'it': {primaryFilePath}" });
                        continue;
                    }

                    string englishText = item.Translations.First(t => t.Language == "en-us").Text;
                    string existingLabelCode = item.ForceNewLabel ? null : FindLabelByText(primaryFilePath, item.Text);
                    string labelCode = existingLabelCode ?? GenerateUniqueLabelCode(primaryFilePath, GenerateLabelCode(englishText), item.LabelPrefix);

                    if (existingLabelCode == null)
                    {
                        EnsureFileEndsWithSingleEmptyLine(primaryFilePath);
                        using var writer = new StreamWriter(primaryFilePath, append: true);
                        writer.WriteLine($"{labelCode}={item.Text}");
                        writer.WriteLine($" ;{item.Comment}");
                    }

                    foreach (var lang in item.Translations)
                    {
                        string additionalFilePath = Path.Combine($"{aosServicePath}\\{appConfigModel}\\{appConfigLabelsModel}\\AxLabelFile\\LabelResources", lang.Language, $"{appConfigLabelFileName}.{lang.Language}.label.txt");
                        if (!File.Exists(additionalFilePath))
                        {
                            results.Add(new GetOrCreateLabelToolResult { Success = false, ErrorMessage = $"File etichette non trovato per la lingua '{lang.Language}': {additionalFilePath}" });
                            continue;
                        }
                        if (!LabelExists(additionalFilePath, labelCode))
                        {
                            EnsureFileEndsWithSingleEmptyLine(additionalFilePath);
                            using var w = new StreamWriter(additionalFilePath, append: true);
                            w.WriteLine($"{labelCode}={lang.Text}");
                            w.WriteLine($" ;{item.Comment}");
                        }
                    }

                    results.Add(new GetOrCreateLabelToolResult { Success = true, LabelCode = $"@{appConfigLabelFileName}:{labelCode}", Text = item.Text });
                }
                catch (Exception ex)
                {
                    results.Add(new GetOrCreateLabelToolResult { Success = false, ErrorMessage = ex.Message });
                }
            }

            return results.ToArray();
        }

        private static string FindLabelByText(string filePath, string text)
        {
            foreach (string line in File.ReadLines(filePath))
            {
                if (line.Contains("=") && line.Split('=')[1].Trim().Equals(text, StringComparison.Ordinal))
                    return line.Split('=')[0];
            }
            return null;
        }

        private static string GenerateLabelCode(string text)
        {
            string sanitizedText = new(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
            return string.Join(string.Empty, sanitizedText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }

        private static string GenerateUniqueLabelCode(string filePath, string baseLabelCode, string? externalPrefix)
        {
            string prefix = GetLabelPrefix(externalPrefix);
            int counter = 1;
            string uniqueLabelCode = string.IsNullOrEmpty(prefix) ? baseLabelCode : $"{prefix}{baseLabelCode}";
            while (File.ReadLines(filePath).Any(line => line.StartsWith(uniqueLabelCode + "=")))
            {
                uniqueLabelCode = string.IsNullOrEmpty(prefix) ? $"{baseLabelCode}_{counter}" : $"{prefix}_{baseLabelCode}_{counter}";
                counter++;
            }
            return uniqueLabelCode;
        }

        private static string GetLabelPrefix(string? externalPrefix = null)
        {
            string prefix = ConfigurationManager.AppSettings["LabelPrefix"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(prefix)) return string.Empty;
            if (prefix.Contains("$$$") && string.IsNullOrEmpty(externalPrefix))
                throw new InvalidOperationException("Il prefisso $$$ è presente ma non è stato passato un valore per il prefisso esterno.");
            if (!string.IsNullOrEmpty(externalPrefix)) prefix = prefix.Replace("$$$", externalPrefix);
            if (prefix.Contains("###"))
            {
                string truncatedPrefix = prefix.Substring(0, prefix.IndexOf("###"));
                string primaryFilePath = GetPrimaryItalianFilePath(appConfigLabelsModel);
                int maxNumber = GetMaxLabelNumber(truncatedPrefix, primaryFilePath);
                prefix = prefix.Replace("###", (maxNumber + 1).ToString("D3"));
            }
            return prefix;
        }

        private static int GetMaxLabelNumber(string basePrefix, string filePath)
        {
            if (!File.Exists(filePath)) return 0;
            int maxNumber = 0;
            foreach (string line in File.ReadLines(filePath))
            {
                if (line.StartsWith(basePrefix) && line.Contains("="))
                {
                    string labelCode = line.Split('=')[0];
                    string numberPart = labelCode.Substring(basePrefix.Length).Split('_')[0];
                    if (int.TryParse(numberPart, out int number))
                        maxNumber = Math.Max(maxNumber, number);
                }
            }
            return maxNumber;
        }

        private static void EnsureFileEndsWithSingleEmptyLine(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var lines = File.ReadAllLines(filePath).ToList();
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
                lines.RemoveAt(lines.Count - 1);
            File.WriteAllLines(filePath, lines);
        }

        private static string GetPrimaryItalianFilePath(string _model)
        {
            return Path.Combine($"{aosServicePath}\\{_model}\\{_model}\\AxLabelFile\\LabelResources", "it", $"{appConfigLabelFileName}.it.label.txt");
        }

        protected static bool LabelExists(string filePath, string labelName)
        {
            if (!File.Exists(filePath)) return false;
            using var reader = new StreamReader(filePath);
            string? line;
            while ((line = reader.ReadLine()) != null)
                if (line.StartsWith(labelName + "=")) return true;
            return false;
        }
    }
}