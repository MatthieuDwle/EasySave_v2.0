using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySave.Model_Folder.Data_Type
{
    public class State
    {

        //this object contains properties of the information we want to register 

        public string hours { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public int totalFiles { get; set; }
        public long totalSize { get; set; }
        public int progress { get; set; }
        public int remainingFiles { get; set; }
        public int sizeRemainingFiles { get; set; }
        public string srcFile { get; set; }
        public string destFile { get; set; }


        public void CreateState(string hours, string name, string state, int totalFiles, long totalSize, int progress, int remainingFiles, int sizeRemainingFiles, string srcFile, string destFile)
        {

            //we create a State object with required information and convert it to string type formatted

            string jsonString = JsonConvert.SerializeObject(new State
            {
                hours = hours,
                name = name,
                state = state,
                totalFiles = totalFiles,
                totalSize = totalSize,
                progress = progress,
                remainingFiles = remainingFiles,
                sizeRemainingFiles = sizeRemainingFiles,
                srcFile = srcFile,
                destFile = destFile

            }, Formatting.Indented);

            //Our object is export at this path

            File.WriteAllText("../../../../state.json", jsonString);

        }

        //This method update the state.json file with current information

        public void UpdateState(string name, bool isDone, string src, string dest)
        {

            //here we get some information for the file

            var date = DateTime.Now;
            string hours = "" + date.Hour + " H " + date.Minute + " M";

            string state = "not active";
            int totalFiles = 0;
            long totalSize = 0;
            int progress = 0;
            int reminingFiles = 0;
            int sizeRemainingFiles = 0;

            //if the save is in progress, we need some more data about progress

            if (!isDone)
            {

                /*here we get the two directory and all their files
                  in a format that allows us to extract information from them */

                DirectoryInfo dir1 = new DirectoryInfo(src);
                DirectoryInfo dir2 = new DirectoryInfo(dest);

                IEnumerable<FileInfo> files1 = dir1.GetFiles("*.*", SearchOption.AllDirectories);
                IEnumerable<FileInfo> files2 = dir2.GetFiles("*.*", SearchOption.AllDirectories);
                FileCompare fileCompare = new FileCompare();

                IEnumerable<FileInfo> queryList2Only = files1.Except(files2, fileCompare);

                //information about directory to save, total of files and total length

                foreach (FileInfo sFile in files1)
                {

                    totalFiles++;
                    totalSize += (long)sFile.Length;

                }

                //information about files that need to be saved

                foreach (FileInfo sFile in queryList2Only)
                {

                    reminingFiles++;
                    sizeRemainingFiles += (int)sFile.Length;

                }

                progress = ((totalFiles - reminingFiles) * 100) / totalFiles;
                state = "active";
            }

            //all these information are export to json file

            this.CreateState(hours, name, state, totalFiles, totalSize, progress, reminingFiles, sizeRemainingFiles, src, dest);

        }

    }
}
