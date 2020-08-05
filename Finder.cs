using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Finder
{
    public class Finder
    {
        public static EventWaitHandle ewh;

        public static void FindFiles(object data)
        {
            Data entData = (Data)data;
            ewh = entData.ewh;
            ScanDir(entData.startDirectory, entData.fileName, entData.subString, entData.mainForm, entData.Node);
            entData.mainForm.Invoke(entData.mainForm.DelegateAddCheckingFileInfo, new Object[] { "" });
        }

        public static void ScanDir(string dirName, string filename, string subString, FinderForm mainForm, TreeNode Node)
        {
            
            DirectoryInfo dir = new DirectoryInfo(dirName);
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    mainForm.Invoke(mainForm.DelegateAddCheckingFileInfo, new Object[] { file.Name });

                    if (file.Name.Contains(filename) || (subString != "" && FileChecker.CheckFile(file.FullName, subString)))
                        mainForm.Invoke(mainForm.DelegateAddNode, new Object[] { file.Name, Node });
                    mainForm.Invoke(mainForm.DelegateIncrFilesCount);
                    Thread.Sleep(19);
                }
                catch (ThreadInterruptedException)
                {
                    mainForm.Invoke(mainForm.DelegateEnableStartStopButton, new Object[] { });
                    ewh.WaitOne();
                }
            }

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                try
                {
                    TreeNode newNode = (TreeNode)mainForm.Invoke(mainForm.DelegateAddNode, new Object[] { directory.Name, Node });
                    Thread.Sleep(17);
                    ScanDir(directory.FullName, filename, subString, mainForm, newNode);

                }
                catch (ThreadInterruptedException)
                {
                    mainForm.Invoke(mainForm.DelegateEnableStartStopButton, new Object[] { });
                    ewh.WaitOne();
                }
            }
            
        }
    }
}
