using System;
using System.Windows.Forms;
using System.Threading;

namespace Finder
{

    public partial class FinderForm : Form
    {
        public static EventWaitHandle ewh;
        public static TreeNode mainNode;
        public delegate void AddCheckingFileInfo(string text);
        public AddCheckingFileInfo DelegateAddCheckingFileInfo;
        public delegate void IncrFilesCount();
        public IncrFilesCount DelegateIncrFilesCount;
        public delegate void EnableStartStopButton();
        public EnableStartStopButton DelegateEnableStartStopButton;
        public delegate TreeNode AddNode(string text, TreeNode node);
        public AddNode DelegateAddNode;
        private Thread t;
        DateTime dold;



        public FinderForm()
        {
            
            DelegateAddCheckingFileInfo = new AddCheckingFileInfo(AddCheckingFileInfoMethod);
            DelegateIncrFilesCount = new IncrFilesCount(IncrFilesCountMethod);
            DelegateAddNode = new AddNode(AddNodeMethod);
            DelegateEnableStartStopButton = new EnableStartStopButton(EnableStartStopButtonMethod);
            InitializeComponent();
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            findButton.Enabled = false;
            if(t != null)
            {
                ewh.Set();
                t.Abort();
                t = null;
                ewh.Dispose();
            }
            PrepareToStart(startDirTextBox.Text);
            Finder f = new Finder();
            t = new Thread(new ParameterizedThreadStart(Finder.FindFiles));
            t.Start(new Data(ewh, startDirTextBox.Text, fileNameTextBox.Text, subStringTextBox.Text, this, mainNode));
            findButton.Enabled = true;
        }

        private void PrepareToStart(string startDirectory)
        {
            pauseButton.Text = "Стоп";
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
            mainNode = new TreeNode(startDirectory);
            mainNode.ImageKey = "folder";
            resultTreeView.Nodes.Clear();
            resultTreeView.Nodes.Add(mainNode);
            filesCountLabel.Text = "0";
            dold = DateTime.Now;
            timer1.Start();
        }

        public TreeNode AddNodeMethod(string text, TreeNode Node)
        {
            TreeNode aNode;
            aNode = new TreeNode(text, 0, 0);
            aNode.ImageKey = "file";
            Node.Nodes.Add(aNode);
            //resultTreeView.ExpandAll();
            return aNode;          
        }

        public void AddCheckingFileInfoMethod(string text)
        {
            fileNameLabel.Text = text;
        }

        public void IncrFilesCountMethod()
        {
            int fc = int.Parse(filesCountLabel.Text);
            fc++;
            filesCountLabel.Text = fc.ToString();
        }

        public void EnableStartStopButtonMethod()
        {
            pauseButton.Enabled = true;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (pauseButton.Text == "Стоп")
            {
                pauseButton.Enabled = false;
                t.Interrupt();
                timer1.Stop();
                pauseButton.Text = "Старт";
            }
            else
            {
                ewh.Set();
                timer1.Start();
                pauseButton.Text = "Стоп";
            }
        }

        private void FinderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(t != null)
            { 
                t.Abort(); 
            }
            SaveToIni();
        }

        private void SaveToIni()
        {
            IniFile File = new IniFile(@".\ini.ini");
            File.IniWriteValue("startDirTextBox", "text", startDirTextBox.Text);
            File.IniWriteValue("fileNameTextBox", "text", fileNameTextBox.Text);
            File.IniWriteValue("subStringTextBox", "text", subStringTextBox.Text);
        }

        private void LoadFromIni()
        {
            IniFile File = new IniFile(@".\ini.ini");
            startDirTextBox.Text = File.IniReadValue("startDirTextBox", "text");
            fileNameTextBox.Text = File.IniReadValue("fileNameTextBox", "text");
            subStringTextBox.Text = File.IniReadValue("subStringTextBox", "text");
        }

        private void FinderForm_Load(object sender, EventArgs e)
        {
            LoadFromIni();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan WorkTime = DateTime.Now - dold;
            string StrTime = WorkTime.Hours.ToString() + "." + WorkTime.Minutes.ToString() + "." + WorkTime.Seconds.ToString() + ".";
            timeLabel.Text = WorkTime.ToString(@"hh\:mm\:ss");
        }
    }

    

    
}
