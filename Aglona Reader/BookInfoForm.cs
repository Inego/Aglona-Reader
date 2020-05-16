using System;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class BookInfoForm : Form
    {
        public ParallelTextControl ParallelTc { get; set; }

        public BookInfoForm()
        {
            InitializeComponent();
        }

        private void BookInfoForm_Load(object sender, EventArgs e)
        {
            if (ParallelTc == null)
                return;

            var pText = ParallelTc.PText;

            if (ParallelTc.Reversed)
            {
                author1.Text = pText.Author2;
                title1.Text = pText.Title2;
                information1.Text = pText.Info2;
                lang1.Text = pText.Lang2;

                author2.Text = pText.Author1;
                title2.Text = pText.Title1;
                information2.Text = pText.Info1;
                lang2.Text = pText.Lang1;
            }
            else
            {
                author1.Text = pText.Author1;
                title1.Text = pText.Title1;
                information1.Text = pText.Info1;
                lang1.Text = pText.Lang1;

                author2.Text = pText.Author2;
                title2.Text = pText.Title2;
                information2.Text = pText.Info2;
                lang2.Text = pText.Lang2;
            }

            information.Text = pText.Info;

            pairCountLabel.Text = pText.Number().ToString();

        }

        private void okButton_Click(object sender, EventArgs e)
        {

            var pText = ParallelTc.PText;

            if (ParallelTc.Reversed)
            {
                pText.Author1 = author2.Text;
                pText.Title1 = title2.Text;
                pText.Info1 = information2.Text;
                pText.Lang1 = lang2.Text;

                pText.Author2 = author1.Text;
                pText.Title2 = title1.Text;
                pText.Info2 = information1.Text;
                pText.Lang2 = lang1.Text;
            }
            else
            {
                pText.Author1 = author1.Text;
                pText.Title1 = title1.Text;
                pText.Info1 = information1.Text;
                pText.Lang1 = lang1.Text;

                pText.Author2 = author2.Text;
                pText.Title2 = title2.Text;
                pText.Info2 = information2.Text;
                pText.Lang2 = lang2.Text;
            }

            pText.Info = information.Text;

            Close();

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
