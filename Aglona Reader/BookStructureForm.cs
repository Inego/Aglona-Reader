using System;
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
            TreeNode previousNode = null;

            var side = (byte) (parallelTextControl.Reversed ? 3 - screenSide : screenSide);

            TreeNode t = null;
            TreeNode newNode = null;

            for (var i = 0; i < parallelTextControl.Number; i++)
            {
                var p = parallelTextControl[i];
                if (p.StructureLevel > 0)
                {
                    newNode = new TreeNode();

                    if (side == 1)
                        newNode.Text = p.SB1 == null ? p.Text1 : p.SB1.ToString();
                    else
                        if (p.SB2 == null)
                            newNode.Text = p.Text2;
                        else
                            newNode.Text = p.SB2.ToString();

                    newNode.Tag = i;
                    
                    

                    previousNode = newNode;

                    AddToParentRecursively(newNode, t, p.StructureLevel);

                    t = newNode;
                }

                if (selected == null && i >= parallelTextControl.HighlightedPair)
                    selected = i == parallelTextControl.HighlightedPair ? newNode : previousNode;

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
                var emptyNode = new TreeNode();
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
                    var emptyNode = new TreeNode();
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
