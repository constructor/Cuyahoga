using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cuyahoga.Core.Infrastructure.Transactions
{
    class Customs
    {

        private static void CollateSubfoldersList(DirectoryInfo directory, IList<DirectoryInfo> directorylist)
        {
            DirectoryInfo[] subfolders = directory.GetDirectories();
            foreach (DirectoryInfo d in subfolders)
            {
                CollateSubfoldersList(d, directorylist);
            }
            directorylist.Add(directory);
        }

        public static void RecursiveDelete(string directorypath, IList<DirectoryInfo> directorylist)
        {
            if (Directory.Exists(directorypath))
            {
                DirectoryInfo directory = new DirectoryInfo(directorypath);

                CollateSubfoldersList(directory, directorylist); //Populate the subfolders to be deleted

                foreach (DirectoryInfo d in directorylist)
                {
                    try
                    {
                        if (d.Exists)
                            d.Delete(true);
                    }
                    catch (IOException)
                    {
                        // Hack: http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
                        // If you are getting IOException deleting directories that are open 
                        // in Explorer, add a sleep(0) to give Explorer a chance to release 
                        // the directory handle.
                        //System.Threading.Thread.Sleep(0);

                        if (d.Exists)
                            d.Delete(true);
                    }
                }
                directorylist.Clear();
            }
        }

    }
}
