using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AglonaReader
{
    public class ParallelText
    {
        public string Author1 { get; set; }
        public string Title1 { get; set; }
        public string Info1 { get; set; }
        public string Lang1 { get; set; }

        public string Author2 { get; set; }
        public string Title2 { get; set; }
        public string Info2 { get; set; }
        public string Lang2 { get; set; }

        public string Info { get; set; }

        public string FileName { get; set; }

        public bool WithAudio { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TextPair> TextPairs { get; }

        /// <summary>
        /// Contains a list of pairs which are at least partially computed.
        /// It is used for speedy truncating.
        /// </summary>
        /// 
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TextPair> ComputedPairs { get; }

        public int Number()
        { return TextPairs.Count; }

        public ParallelText()
        {
            TextPairs = new List<TextPair>();
            ComputedPairs = new List<TextPair>();

            Author1 = "";
            Title1 = "";
            Info1 = "";
            Lang1 = "";

            Author2 = "";
            Title2 = "";
            Info2 = "";
            Lang2 = "";

            Info = "";

        }

        public TextPair this[int pairIndex] => TextPairs[pairIndex];

        public void AddPair(string text1, string text2, bool startParagraph1 = true, bool startParagraph2 = true)
        {
            var newPair = TextPairs.Count == 0 ?
                new TextPair(text1, text2, true, true) :
                new TextPair(text1, text2, startParagraph1, startParagraph2);

            TextPairs.Add(newPair);

        }

        public void Truncate()
        {
            foreach (var p in ComputedPairs)
                p.ClearComputedWords();
            ComputedPairs.Clear();
        }

        public static void InsertWords(Collection<CommonWordInfo> list, int spaceLeft)
        {

            if (list == null)
                return;

            Collection<WordInfo> l = null;
            TextPair prevP = null;
            byte prevSide = 0;

            var bias = 0;

            // Spaces can be only in cases like W E or E W or W W,
            // where W is a "western" word and E is an eastern character
            // they can't be between EE

            CommonWordInfo previousWord = null;

            var numberOfSpacesLeft = 0;

            // So before extending spaces we must know their number.
            foreach (var r in list)
            {
                if (previousWord != null)
                    if (!(r.Eastern && previousWord.Eastern))
                        numberOfSpacesLeft++;
                previousWord = r;
            }

            previousWord = null;

            foreach (var r in list)
            {

                if (spaceLeft != 0 && previousWord != null && !(r.Eastern && previousWord.Eastern))
                {
                    var inc = spaceLeft / numberOfSpacesLeft;
                    bias += inc;
                    spaceLeft -= inc;
                    numberOfSpacesLeft--;
                }

                if (prevP != r.TextPair)
                {
                    prevP = r.TextPair;
                    prevSide = 0;
                }

                if (r.side != prevSide)
                {
                    prevSide = r.side;
                    l = prevP.ComputedWords(r.side, true);
                }

                l.Add(new WordInfo(r.Word, r.Line, r.X1 + bias, r.X2 + bias, r.Position, r.Eastern));

                previousWord = r;
            }

            list.Clear();
        }


        public void Save(string newFileName)
        {
            var whitespace = Environment.NewLine;
            
            using (var writer = new XmlTextWriter(newFileName, Encoding.UTF8))
            {
                writer.WriteStartElement("ParallelBook");

                writer.WriteAttributeString("lang1", Lang1);
                writer.WriteAttributeString("author1", Author1);
                writer.WriteAttributeString("title1", Title1);
                writer.WriteAttributeString("info1", Info1);

                writer.WriteAttributeString("lang2", Lang2);
                writer.WriteAttributeString("author2", Author2);
                writer.WriteAttributeString("title2", Title2);
                writer.WriteAttributeString("info2", Info2);

                writer.WriteAttributeString("info", Info);
                
                writer.WriteWhitespace(whitespace);

                foreach (var p in TextPairs)
                {
                    writer.WriteStartElement("p");

                    byte level = 0;

                    if (p.StructureLevel == 1)
                        level = 4;
                    else if (p.StructureLevel == 2)
                        level = 5;
                    else if (p.StructureLevel == 3)
                        level = 6;
                    else
                    {
                        if (p.StartParagraph1)
                            level = 1;
                        if (p.StartParagraph2)
                            level += 2;
                    }

                    if (level != 0)
                    {
                        writer.WriteStartAttribute("l");
                        writer.WriteValue(level);
                        writer.WriteEndAttribute();
                    }

                    writer.WriteAttributeString("s", p.Sb1 == null ? p.Text1 : p.Sb1.ToString());
                    writer.WriteAttributeString("t", p.Sb2 == null ? p.Text2 : p.Sb2.ToString());

                    if (WithAudio && p.AudioFileNumber > 0)
                    {
                        writer.WriteAttributeString("f", p.AudioFileNumber.ToString());
                        writer.WriteAttributeString("b", p.TimeBeg.ToString());
                        writer.WriteAttributeString("e", p.TimeEnd.ToString());
                    }
                    
                    writer.WriteEndElement();
                    
                    writer.WriteWhitespace(whitespace);
                }

                writer.WriteEndElement();
                writer.Flush();
            }

            FileName = newFileName;
        }


        public void Load(string newFileName)
        {
            WithAudio = newFileName.EndsWith(".pbs");

            var xmlReaderSettings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            
            using (var reader = XmlReader.Create(newFileName, xmlReaderSettings))
            {
                try
                {
                    reader.Read();
                }
                catch
                {
                    MessageBox.Show("File not found or unavailable: " + newFileName);
                    return;
                }

                if (reader.NodeType != XmlNodeType.Element) return;

                if (reader.Name != "ParallelBook") return;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "lang1") return;

                Lang1 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "author1") return;

                Author1 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "title1") return;

                Title1 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "info1") return;

                Info1 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "lang2") return;

                Lang2 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "author2") return;

                Author2 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "title2") return;

                Title2 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "info2") return;

                Info2 = reader.Value;

                if (!reader.MoveToNextAttribute()) return;

                if (reader.Name != "info") return;

                Info = reader.Value;


                NextPair:

                if (!reader.Read()) return;

                if (reader.Name == "p" && reader.NodeType == XmlNodeType.Element)
                {
                    var p = new TextPair();

                    while (reader.MoveToNextAttribute())
                    {
                        switch (reader.Name)
                        {
                            case "l":
                                switch (reader.Value)
                                {
                                    case "1":
                                        p.StartParagraph1 = true;
                                        break;
                                    case "2":
                                        p.StartParagraph2 = true;
                                        break;
                                    case "3":
                                        p.StartParagraph1 = true;
                                        p.StartParagraph2 = true;
                                        break;
                                    case "4":
                                        p.SetStructureLevel(1);
                                        break;
                                    case "5":
                                        p.SetStructureLevel(2);
                                        break;
                                    case "6":
                                        p.SetStructureLevel(3);
                                        break;
                                }
                                break;
                            
                            case "s":
                                if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                                    p.Sb1 = new StringBuilder(reader.Value);
                                else
                                    p.Text1 = reader.Value;
                                p.totalTextSize = reader.Value.Length;
                                break;
                            
                            case "t":
                                if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                                    p.Sb2 = new StringBuilder(reader.Value);
                                else
                                    p.Text2 = reader.Value;
                                p.totalTextSize += reader.Value.Length;
                                break;
                            
                            case "f":
                                if (WithAudio)
                                    p.AudioFileNumber = uint.Parse(reader.Value);
                                break;
                            
                            case "b":
                                if (WithAudio)
                                    p.TimeBeg = uint.Parse(reader.Value);
                                break;
                            
                            case "e":
                                if (WithAudio)
                                    p.TimeEnd = uint.Parse(reader.Value);
                                break;
                        }
                    }

                    TextPairs.Add(p);

                    goto NextPair;
                }

                reader.Close();

                FileName = newFileName;

                if (TextPairs.Count > 0)
                    UpdateAggregates(0);
            }
        }


        internal void UpdateAggregates(int pairIndex)
        {
            int accLength;

            if (pairIndex <= 0)
                accLength = -2;
            else
                accLength = TextPairs[pairIndex - 1].aggregateSize;

            for (var i = Math.Max(pairIndex, 0); i < Number(); i++)
            {
                var tp = TextPairs[i];
                accLength += 2 + tp.totalTextSize;
                tp.aggregateSize = accLength;
            }
        }


        private static void WriteIfNotEmpty(TextWriter outfile, string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            
            outfile.WriteLine(s);
            outfile.WriteLine();
        }

        private static string EscapeForHtml(string src)
        {
            var sb = new StringBuilder(src);

            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br>");
            sb.Replace("\n\r", "<br>");
            sb.Replace("\r", "<br>");
            sb.Replace("\n", "<br>");

            return sb.ToString();


        }


        private static void WriteHtmlRow(TextWriter outfile, int leftNumber, string c1, string c2)
        {

            outfile.WriteLine("<tr>");
            outfile.WriteLine("<td>");
            outfile.WriteLine($"<sup>{leftNumber}</sup>");
            outfile.WriteLine("</td>");
            outfile.WriteLine("<td>");
            outfile.WriteLine(c1);
            outfile.WriteLine("</td>");
            outfile.WriteLine("<td>");
            outfile.WriteLine(c2);
            outfile.WriteLine("</td>");
            outfile.WriteLine("</tr>");

        }

        internal void ExportHtml(string fileName)
        {

            using (var outfile = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                outfile.WriteLine("<!DOCTYPE html><html><body>");
                outfile.WriteLine("<style type=\"text/css\">");
                outfile.WriteLine(".tg  {border-collapse:collapse;border-spacing:0;}");
                outfile.WriteLine(".tg td{border-style:solid;border-width:1px;vertical-align:top;}");
                outfile.WriteLine(".tg td:first-child{border-style:solid;border-width:0px;text-align:right;}");
                outfile.WriteLine("</style>");
                
                outfile.WriteLine("<table  class=\"tg\">");

                var c1 = "";
                var c2 = "";

                var leftNumber = 0;
                
                for (var i = 0; i < Number(); i++)
                {
                    var p = this[i];

                    if (p.StartParagraph1 || p.StartParagraph2)
                    {
                        if (leftNumber > 0)
                            WriteHtmlRow(outfile, leftNumber, c1, c2);

                        c1 = EscapeForHtml(p.Text1);
                        c2 = EscapeForHtml(p.Text2);

                        if (p.StructureLevel > 0)
                        {
                            c1 = "<h" + p.StructureLevel + ">" + c1 + "</h" + p.StructureLevel + ">";
                            c2 = "<h" + p.StructureLevel + ">" + c2 + "</h" + p.StructureLevel + ">";
                        }

                        leftNumber = i + 1;
                        
                    }

                    if (leftNumber >= i + 1) continue;
                    
                    var q = $" <sup>{(i + 1) % 100}</sup> ";
                    c1 += (p.StartParagraph1 ? "<br>" : " ") + q + EscapeForHtml(p.Text1);
                    c2 += (p.StartParagraph2 ? "<br>" : " ") + q + EscapeForHtml(p.Text2);
                }

                if (leftNumber > 0)
                    WriteHtmlRow(outfile, leftNumber, c1, c2);

                outfile.WriteLine("</table>");
                outfile.WriteLine("</body>");
                outfile.WriteLine("</html>");
                
                outfile.Close();

            }

        }

        internal void ExportText(string fileName, int sideToExport)
        {

            using (var outfile = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                TextPair p;
                TextPair pPrev = null;

                if (sideToExport == 1)
                {
                    WriteIfNotEmpty(outfile, Author1);
                    WriteIfNotEmpty(outfile, Title1);
                    WriteIfNotEmpty(outfile, Info1);
                    WriteIfNotEmpty(outfile, Info);

                    for (var i = 0; i < Number(); i++)
                    {
                        p = this[i];

                        if (pPrev != null && (p.StructureLevel > 0 || pPrev.StructureLevel > 0))
                            outfile.WriteLine();

                        if (p.StartParagraph1)
                            outfile.WriteLine();
                        else
                            outfile.Write(' ');

                        if (p.Sb1 == null)
                            outfile.Write(p.Text1);
                        else
                            outfile.Write(p.Sb1);

                        pPrev = p;

                    }

                }
                else
                {

                    WriteIfNotEmpty(outfile, Author2);
                    WriteIfNotEmpty(outfile, Title2);
                    WriteIfNotEmpty(outfile, Info2);
                    WriteIfNotEmpty(outfile, Info);

                    for (var i = 0; i < Number(); i++)
                    {
                        p = this[i];

                        if (pPrev != null && (p.StructureLevel > 0 || pPrev.StructureLevel > 0))
                            outfile.WriteLine();

                        if (p.StartParagraph2)
                            outfile.WriteLine();
                        else
                            outfile.Write(' ');

                        if (p.Sb2 == null)
                            outfile.Write(p.Text2);
                        else
                            outfile.Write(p.Sb2);

                        pPrev = p;
                    }
                }

                outfile.Close();
            }
        }

        // Physically reverses book contents
        internal void ReverseContents()
        {
            var tmp = Author1;
            
            Author1 = Author2;
            Author2 = tmp;

            tmp = Title1;
            Title1 = Title2;
            Title2 = tmp;

            tmp = Info1;
            Info1 = Info2;
            Info2 = tmp;

            tmp = Lang1;
            Lang1 = Lang2;
            Lang2 = tmp;

            foreach (var tp in TextPairs)
            {
                var tmpSb = tp.Sb1;
                
                tp.Sb1 = tp.Sb2;
                tp.Sb2 = tmpSb;

                tmp = tp.Text1;
                tp.Text1 = tp.Text2;
                tp.Text2 = tmp;

                var tmpBool = tp.StartParagraph1;
                tp.StartParagraph1 = tp.StartParagraph2;
                tp.StartParagraph2 = tmpBool;
            }


        }

        internal void DeletePair(int p)
        {
            TextPairs.RemoveAt(p);

            if (p != 0 || TextPairs.Count <= 0) return;
            
            var tp0 = TextPairs[0];
            tp0.StartParagraph1 = true;
            tp0.StartParagraph2 = true;
        }
    }
}