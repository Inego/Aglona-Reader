using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AglonaReader
{
    public enum SelectionAction
    {
        CopyToClipboard = 0,
        OpenInGoogleTranslate = 1
    }

    public enum HtmlExportStyle
    {
        TwoColumnTable = 0,
        Alternating = 1 // TODO support
    }

    public class HtmlExportTctSettings
    {
        public bool ShowBorders { get; set; }
        public bool UseColors { get; set; }
        public bool ShowPairNumbers { get; set; }
    }
    
    public class AppSettings
    {
        public bool HighlightFragments { get; set; }
        public bool HighlightFirstWords { get; set; }
        public double Brightness { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        
        public SelectionAction SelectionAction { get; set; }
        
        public HtmlExportStyle HtmlExportStyle { get; set; }
        
        public HtmlExportTctSettings HtmlExportTctSettings { get; set; }

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
            
            HtmlExportStyle = HtmlExportStyle.TwoColumnTable;
            HtmlExportTctSettings = new HtmlExportTctSettings
            {
                ShowBorders = true,
                ShowPairNumbers = true,
                UseColors = false
            };

        }
    }
}
