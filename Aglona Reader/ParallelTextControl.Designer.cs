using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace AglonaReader
{
    partial class ParallelTextControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
            drawBrush.Dispose();
            popUpBrush.Dispose();
            AdvancedHighlightFrame.Dispose();
            textFont.Dispose();
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            // Don't need no frame
            //base.OnPaintBackground(e);
        }


        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            //base.OnPaint(e);
            if (PrimaryBG == null)
                e.Graphics.Clear(this.BackColor);
            else
            //{
            //    SecondaryBG.Render();
                PrimaryBG.Render();
            //}

        }

        protected override bool IsInputKey(Keys keyData)
        {

            if (keyData == Keys.Up
                || keyData == Keys.Down
                || keyData == Keys.Right
                || keyData == Keys.Left
                || keyData == Keys.Tab
                )

                return true;

            return base.IsInputKey(keyData);
        }




        
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ParallelTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Name = "ParallelTextControl";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
