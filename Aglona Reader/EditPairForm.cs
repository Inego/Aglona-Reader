using System;
using System.Globalization;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class EditPairForm : Form
    {

        public bool Result { get; private set; }

        public ParallelTextControl ParallelTextControl { get; set; }

        private TextPair TextPair { get; set; }

        public int PairIndex { get; set; }

        //private string Text1;
        //private string Text2;
        //private byte level;

        public EditPairForm()
        {
            InitializeComponent();
        }

        private void PressOK()
        {
            Result = true;

            if (ParallelTextControl.Reversed)
            {
                TextPair.Text1 = textBox2.Text;
                TextPair.Text2 = textBox1.Text;
                TextPair.StartParagraph1 = start2.Checked;
                TextPair.StartParagraph2 = start1.Checked;
            }
            else
            {
                TextPair.Text1 = textBox1.Text;
                TextPair.Text2 = textBox2.Text;
                TextPair.StartParagraph1 = start1.Checked;
                TextPair.StartParagraph2 = start2.Checked;
            }

            TextPair.SB1 = null;
            TextPair.SB2 = null;

            if (level0.Checked)
                TextPair.StructureLevel = 0;
            else if (level1.Checked)
                TextPair.StructureLevel = 1;
            else if (level2.Checked)
                TextPair.StructureLevel = 2;
            else if (level3.Checked)
                TextPair.StructureLevel = 3;

            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            PressOK();
        }

        private void EditPairForm_Shown(object sender, EventArgs e)
        {
            TextPair = ParallelTextControl[PairIndex];

            if (ParallelTextControl.Reversed)
            {
                textBox1.Text = TextPair.SB2 == null ? TextPair.Text2 : TextPair.SB2.ToString();
                textBox2.Text = TextPair.SB1 == null ? TextPair.Text1 : TextPair.SB1.ToString();
                start1.Checked = TextPair.StartParagraph2;
                start2.Checked = TextPair.StartParagraph1;
            }
            else
            {
                textBox1.Text = TextPair.SB1 == null ? TextPair.Text1 : TextPair.SB1.ToString();
                textBox2.Text = TextPair.SB2 == null ? TextPair.Text2 : TextPair.SB2.ToString();
                start1.Checked = TextPair.StartParagraph1;
                start2.Checked = TextPair.StartParagraph2;
            }

            switch (TextPair.StructureLevel)
            {
                case 0:
                    level0.Checked = true;
                    break;
                case 1:
                    level1.Checked = true;
                    break;
                case 2:
                    level2.Checked = true;
                    break;
                case 3:
                    level3.Checked = true;
                    break;
            }


            Result = false;
            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            ShiftLanguage(1);
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            ShiftLanguage(2);
        }
        
        private void ShiftLanguage(int side)
        {
            if (ParallelTextControl.Reversed)
                side = 3 - side;

            var langCode = side == 1 ? ParallelTextControl.PText.Lang1 : ParallelTextControl.PText.Lang2;
            
            if (langCode == null)
                return;
            
            langCode = langCode.Trim().ToUpper();

            string cult;

            switch (langCode)
            {
                case "EN":
                    cult = "en-US";
                    break;
                case "RU":
                    cult = "ru-RU";
                    break;
                case "DE":
                    cult = "de-DE";
                    break;
                case "ES":
                    cult = "es-ES";
                    break;
                case "IT":
                    cult = "it-IT";
                    break;
                case "FR":
                    cult = "fr-FR";
                    break;
                case "PL":
                    cult = "pl-PL";
                    break;
                case "EL":
                case "GR":
                    cult = "el-GR";
                    break;
                case "TR":
                    cult = "tr-TR";
                    break;
                case "LV":
                    cult = "lv-LV";
                    break;
                default:
                    return;
            }

            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo(cult));

        }


        internal void setFocusNewPair()
        {
            ActiveControl = textBox1;
        }
    }
}
