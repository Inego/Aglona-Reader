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
    public partial class BookStructureForm : Form
    {

        public ParallelTextControl parallelTextControl;
        public byte screenSide;
        public int pairIndex;


        public BookStructureForm()
        {
            InitializeComponent();
            pairIndex = -1;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
                pairIndex = (int) treeView.SelectedNode.Tag;

            Close();
        }

        private void BookStructureForm_Shown(object sender, EventArgs e)
        {

            TreeNode selected = null;

            byte side = (byte) (parallelTextControl.Reversed ? 3 - screenSide : screenSide);

            TextPair p;
            TreeNode t = null;
            for (int i = 0; i < parallelTextControl.Number; i++)
            {
                p = parallelTextControl[i];
                if (p.StructureLevel > 0)
                {
                    TreeNode newNode = new TreeNode();

                    if (side == 1)
                        if (p.SB1 == null)
                            newNode.Text = p.Text1;
                        else
                            newNode.Text = p.SB1.ToString();
                    else
                        if (p.SB2 == null)
                            newNode.Text = p.Text2;
                        else
                            newNode.Text = p.SB2.ToString();

                    newNode.Tag = i;
                    if (selected == null && i >= parallelTextControl.HighlightedPair)
                        selected = newNode;

                    AddToParentRecursively(newNode, t, p.StructureLevel);

                    t = newNode;
                }

                

            }

            
            treeView.ExpandAll();
            treeView.SelectedNode = selected;

            treeView.Select();

        }

        private void AddToParentRecursively(TreeNode newNode, TreeNode t, int structureLevel)
        {
            if (structureLevel == 1)
            {
                treeView.Nodes.Add(newNode);
                return;
            }

            if (t == null)
            {
                TreeNode emptyNode = new TreeNode();
                AddToParentRecursively(emptyNode, t, structureLevel - 1);
                emptyNode.Nodes.Add(newNode);
            }
            else
            {
                while (t.Level + 1 >= structureLevel)
                    t = t.Parent;

                if (t.Level + 1 == structureLevel - 1)
                    t.Nodes.Add(newNode);

                else
                {
                    TreeNode emptyNode = new TreeNode();
                    AddToParentRecursively(emptyNode, t, structureLevel - 1);
                    emptyNode.Nodes.Add(newNode);
                }
            }
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            OKButton_Click(sender, e);
        }
    }
}
