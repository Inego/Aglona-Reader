using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class BookInfoForm : Form
    {
        public ParallelTextControl pTC;

        public BookInfoForm()
        {
            InitializeComponent();
        }

        private void BookInfoForm_Load(object sender, EventArgs e)
        {
            if (pTC == null)
                return;

            ParallelText pText = pTC.pText;

            if (pTC.reversed)
            {
                author1.Text = pText.author2;
                title1.Text = pText.title2;
                information1.Text = pText.info2;
                lang1.Text = pText.lang2;

                author2.Text = pText.author1;
                title2.Text = pText.title1;
                information2.Text = pText.info1;
                lang2.Text = pText.lang1;
            }
            else
            {
                author1.Text = pText.author1;
                title1.Text = pText.title1;
                information1.Text = pText.info1;
                lang1.Text = pText.lang1;

                author2.Text = pText.author2;
                title2.Text = pText.title2;
                information2.Text = pText.info2;
                lang2.Text = pText.lang2;
            }

            information.Text = pText.info;

        }

        private void okButton_Click(object sender, EventArgs e)
        {

            ParallelText pText = pTC.pText;

            if (pTC.reversed)
            {
                pText.author1 = author2.Text;
                pText.title1 = title2.Text;
                pText.info1 = information2.Text;
                pText.lang1 = lang2.Text;

                pText.author2 = author1.Text;
                pText.title2 = title1.Text;
                pText.info2 = information1.Text;
                pText.lang2 = lang1.Text;
            }
            else
            {
                pText.author1 = author1.Text;
                pText.title1 = title1.Text;
                pText.info1 = information1.Text;
                pText.lang1 = lang1.Text;

                pText.author2 = author2.Text;
                pText.title2 = title2.Text;
                pText.info2 = information2.Text;
                pText.lang2 = lang2.Text;
            }

            pText.info = information.Text;

            Close();

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
