using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MetaTraderDeveloperTools
{
    class Main
    {
        public void Start()
        {
            string[] mqlPathList = GetMqlPathList();
            if (mqlPathList.Count() < 1)
            {
                return;
            }

            DistributeEA(mqlPathList);
        }

        private string[] GetMqlPathList()
        {
            string[] mqlPath = { };
            int idx = 0;

            string[] userList = Directory.GetDirectories(@"C:\Users");

            for (int i = 0; i <= userList.Length - 1; ++i)
            {
                string tmPath = userList[i] + @"\AppData\Roaming\MetaQuotes\Terminal";
                if (Directory.Exists(tmPath) == false)
                {
                    continue;
                }
                foreach (string d in Directory.GetDirectories(tmPath))
                {
                    Array.Resize(ref mqlPath, idx + 1);
                    mqlPath.SetValue(d, idx);
                    ++idx;
                }
            }

            return mqlPath;
        }

        private void DistributeEA(string[] mqlPathList)
        {
            for (int i = 0; i <= mqlPathList.Length - 1; ++i)
            {
                string mqlVer = GetMtVerStr_MTDataDir(mqlPathList[i]);

                string orgPath;
                if ((mqlVer == "MQL4") || (mqlVer == "MQL5"))
                {
                    orgPath = "[Folder path from which to distribute]" + mqlVer;
                }
                else
                {
                    continue;
                }

                try
                {
                    CopyDirectory(orgPath, mqlPathList[i] + @"\" + mqlVer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private string GetMtVerStr_MTDataDir(string mqlPath)
        {
            foreach (string d in Directory.GetDirectories(mqlPath))
            {
                string mqlVer = Path.GetFileName(d).ToUpper();
                if ((mqlVer == "MQL4") || (mqlVer == "MQL5"))
                {
                    return mqlVer;
                }
            }

            return "";
        }

        private string MyDir()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
        }

        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            if (Directory.Exists(destDirName) == false)
            {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }

            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
            {
                destDirName = destDirName + Path.DirectorySeparatorChar;
            }

            string[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                string dst = destDirName + Path.GetFileName(file);

                if (File.Exists(dst))
                {
                    FileAttributes fa = File.GetAttributes(dst);

                    fa = fa & ~FileAttributes.ReadOnly;
                    File.SetAttributes(dst, fa);
                }

                File.Copy(file, dst, true);
            }

            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
            {
                CopyDirectory(dir, destDirName + Path.GetFileName(dir));
            }
        }
    }
}
