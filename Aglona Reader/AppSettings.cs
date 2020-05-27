using System.Collections.ObjectModel;

namespace AglonaReader
{
    public enum SelectionAction
    {
        CopyToClipboard = 0,
        OpenInGoogleTranslate = 1
    }
    
    public class AppSettings
    {
        public bool HighlightFragments { get; set; }
        public bool HighlightFirstWords { get; set; }
        public double Brightness { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        
        public SelectionAction SelectionAction { get; set; }

        public string GoogleTranslateTargetLanguage { get; set; }

        public Collection<FileUsageInfo> FileUsages { get; }

        public AppSettings()
        {
            HighlightFragments = true;
            HighlightFirstWords = true;
            Brightness = 0.974;
            FontName = "Arial";
            FontSize = 18.0F;
            FileUsages = new Collection<FileUsageInfo>();
        }
    }
}
