namespace WebAppp.Models
{
    public class NlpAnalysisRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class WordAnalysis
    {
        public string Word { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class NlpAnalysisResponse
    {
        public string Text { get; set; } = string.Empty;
        public List<WordAnalysis> Analysis { get; set; } = new List<WordAnalysis>();
    }
}