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

        private static TreeNode mainNode;

        public delegate void AddCheckingFileInfo(String text);
        public AddCheckingFileInfo DelegateAddCheckingFileInfo;
        public delegate void IncrFilesCount();
        public IncrFilesCount DelegateIncrFilesCount;
        public delegate void AddNode(String text);
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
            //Data data = new Data(startDirTextBox.Text, fileNameTextBox.Text, this);
            Thread t = new Thread(new ParameterizedThreadStart(Finder.FindFiles));
            t.Start(new Data(startDirTextBox.Text, fileNameTextBox.Text, this));
            //f.FindFiles(startDirTextBox.Text, fileNameTextBox.Text, this);
        }

        private void PrepareToStart(String startDirectory)
        {
            mainNode = new TreeNode(startDirectory);
            mainNode.ImageKey = "folder";
            resultTreeView.Nodes.Clear();
            resultTreeView.Nodes.Add(mainNode);
            filesCountLabel.Text = "0";
        }

        public void AddNodeMethod(String text)
        {
            TreeNode aNode;
            aNode = new TreeNode(text, 0, 0);
            aNode.ImageKey = "file";
            mainNode.Nodes.Add(aNode);
            resultTreeView.ExpandAll();
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
        public Data(String startDirectory, String fileName, FinderForm mainForm)
        {
            this.startDirectory = startDirectory;
            this.fileName = fileName;
            this.mainForm = mainForm;
        }
    }

    public class Finder
    {
        public static void FindFiles (object data)
        {
            Data entData = (Data)data;
            ScanDir(entData.startDirectory, entData.fileName, entData.mainForm);
        }

        public static void ScanDir(String dirName, String filename, FinderForm mainForm)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            foreach (FileInfo file in dir.GetFiles())
            {
                mainForm.Invoke(mainForm.DelegateAddCheckingFileInfo, new Object[] { file.Name });
                
                if (file.Name.Contains(filename))
                    mainForm.Invoke(mainForm.DelegateAddNode, new Object[] { file.Name });           
                
                mainForm.Invoke(mainForm.DelegateIncrFilesCount);
                Thread.Sleep(300);
            }
            mainForm.Invoke(mainForm.DelegateAddCheckingFileInfo, new Object[] { "" });
        }
    }
}
