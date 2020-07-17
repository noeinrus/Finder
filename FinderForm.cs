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
using System.Threading;

namespace Finder
{

    public partial class FinderForm : Form
    {

        public static TreeNode mainNode;

        public delegate void AddCheckingFileInfo(String text);
        public AddCheckingFileInfo DelegateAddCheckingFileInfo;
        public delegate void IncrFilesCount();
        public IncrFilesCount DelegateIncrFilesCount;
        public delegate TreeNode AddNode(String text, TreeNode node);
        public AddNode DelegateAddNode;

        

        public FinderForm()
        {
            DelegateAddCheckingFileInfo = new AddCheckingFileInfo(AddCheckingFileInfoMethod);
            DelegateIncrFilesCount = new IncrFilesCount(IncrFilesCountMethod);
            DelegateAddNode = new AddNode(AddNodeMethod);
            InitializeComponent();
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            PrepareToStart(startDirTextBox.Text);
            Finder f = new Finder();
            Thread t = new Thread(new ParameterizedThreadStart(Finder.FindFiles));
            t.Start(new Data(startDirTextBox.Text, fileNameTextBox.Text, this, mainNode));
        }

        private void PrepareToStart(String startDirectory)
        {
            mainNode = new TreeNode(startDirectory);
            mainNode.ImageKey = "folder";
            resultTreeView.Nodes.Clear();
            resultTreeView.Nodes.Add(mainNode);
            filesCountLabel.Text = "0";
        }

        public TreeNode AddNodeMethod(String text, TreeNode Node)
        {
            TreeNode aNode;
            aNode = new TreeNode(text, 0, 0);
            aNode.ImageKey = "file";
            Node.Nodes.Add(aNode);
            resultTreeView.ExpandAll();
            return aNode;          
        }

        public void AddCheckingFileInfoMethod(String text)
        {
            fileNameLabel.Text = text;
        }

        public void IncrFilesCountMethod()
        {
            int fc = int.Parse(filesCountLabel.Text);
            fc++;
            filesCountLabel.Text = fc.ToString();
        }
    }

    public class Data
    {
        public String startDirectory, fileName;
        public FinderForm mainForm;
        public TreeNode Node;
        public Data(String startDirectory, String fileName, FinderForm mainForm, TreeNode Node)
        {
            this.startDirectory = startDirectory;
            this.fileName = fileName;
            this.mainForm = mainForm;
            this.Node = Node;
        }
    }

    public class Finder
    {
        public static void FindFiles (object data)
        {
            Data entData = (Data)data;
            ScanDir(entData.startDirectory, entData.fileName, entData.mainForm, entData.Node);
            entData.mainForm.Invoke(entData.mainForm.DelegateAddCheckingFileInfo, new Object[] { "" });
        }

        public static void ScanDir(String dirName, String filename, FinderForm mainForm, TreeNode Node)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            foreach (FileInfo file in dir.GetFiles())
            {
                mainForm.Invoke(mainForm.DelegateAddCheckingFileInfo, new Object[] { file.Name });
                
                if (file.Name.Contains(filename))
                    mainForm.Invoke(mainForm.DelegateAddNode, new Object[] { file.Name, Node });                   
                mainForm.Invoke(mainForm.DelegateIncrFilesCount);
                Thread.Sleep(17);
            }

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {             
                TreeNode newNode = (TreeNode) mainForm.Invoke(mainForm.DelegateAddNode, new Object[] { directory.Name, Node });
                Thread.Sleep(17);
                ScanDir(directory.FullName, filename, mainForm, newNode);
            }
        }
    }
}
