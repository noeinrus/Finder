using System;
using System.Windows.Forms;
using System.Threading;

namespace Finder
{
    public class Data
    {
        public EventWaitHandle ewh;
        public string startDirectory, fileName, subString;
        public FinderForm mainForm;
        public TreeNode Node;
        public Data(EventWaitHandle ewh, string startDirectory, string fileName, string subString, FinderForm mainForm, TreeNode Node)
        {
            this.ewh = ewh;
            this.startDirectory = startDirectory;
            this.fileName = fileName;
            this.subString = subString; 
            this.mainForm = mainForm;
            this.Node = Node;
        }
    }
}
