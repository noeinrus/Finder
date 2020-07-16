using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Finder
{

    public partial class FinderForm : Form
    {

        private static TreeNode mainNode;

        public FinderForm()
        {
            InitializeComponent();
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            PrepareToStart(@"G:\");
            Finder f = new Finder();
            f.FindFiles(@"G:\", "444.pas", this);
        }

        private void PrepareToStart(String startDirectory)
        {
            mainNode = new TreeNode(startDirectory);
            mainNode.ImageKey = "folder";
            resultTreeView.Nodes.Clear();
            resultTreeView.Nodes.Add(mainNode);
        }

        public void AddNode(String text, byte type)
        {
            TreeNode aNode;
            aNode = new TreeNode(text, 0, 0);
            aNode.ImageKey = "file";
            mainNode.Nodes.Add(aNode);
            resultTreeView.ExpandAll();
        }

        public void AddCheckingFileInfo(String text)
        {
            fileNameLabel.Text = text;
        }

        private void FinderForm_Load(object sender, EventArgs e)
        {
            
        }
    }

    class Finder
    {
        public void FindFiles (String startDirectory, String fileName, FinderForm mainForm)
        {
            ScanDir(startDirectory, fileName, mainForm);
        }

        public void ScanDir(String dirName, String filename, FinderForm mainForm)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            foreach (FileInfo file in dir.GetFiles())
            {
                mainForm.AddCheckingFileInfo(file.Name);
                if (filename == file.Name)
                    mainForm.AddNode(file.Name, 0);
            }
        }
    }
}
