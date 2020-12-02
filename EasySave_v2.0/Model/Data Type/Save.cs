using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace EasySave.Model_Folder.Data_Type
{
    public class Save
    {
        public string Name { get; set; } 
        public string Src { get; set; }
        public string Dest { get; set; }
        public bool Type { get; set; }
        public int Id { get; set; }
        public State State { get; set; }
        private List<string> files = new List<string>();

        /*
         * Save constructor
         */ 
        public Save(string nameNewSave, string srcNewSave, string destNewSave, bool typeNewSave, int idNewSave)
        {
            Name = nameNewSave;
            Src = srcNewSave;
            Dest = destNewSave;
            Type = typeNewSave;
            Id = idNewSave;
            
        }
        
        /*
         * create new instance of State
         * determine which type of save to run
         * copy file stored in files var
         */ 
        public void Run()
        {
            State = new State();

            if (Type) DifferentialSave(Src, Dest);
            else CompleteSave(this.Src, this.Dest);
            this.files.ForEach(file =>
            {
                string src = Path.Combine(this.Src, file);
                string dest = Path.Combine(this.Dest, file);
                File.Copy(src, dest, true);
                State.UpdateState(this.Name, false, this.Src, this.Dest);
            });
            State.UpdateState(this.Name, true, this.Src, this.Dest);
        }

        /*
         * recursive method which list all files to save if is newer
         * create the directory and subdirectory in the destination folder
         */ 
        public void DifferentialSave(string src, string dest, string prefixe = "")
        {
            DirectoryInfo dir = new DirectoryInfo(src);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            Directory.CreateDirectory(dest);

            foreach (FileInfo file in files)
            {
                string filePath = Path.Combine(prefixe, file.Name);
                string destFile = Path.Combine(dest, filePath);
                DateTime srcTime = File.GetLastWriteTimeUtc(file.FullName);
                DateTime destTime = File.GetLastWriteTimeUtc(destFile);
                if (this.Type && ((DateTime.Compare(srcTime, destTime) > 0) || (!file.Exists))) this.files.Add(filePath);

            }

            foreach (DirectoryInfo folder in dirs)
            {
                string tmp = Path.Combine(dest, folder.Name);
                CompleteSave(folder.FullName, tmp, folder.Name);
            }
        }

        /*
         * recursive method which list all files to save
         * create the directory and subdirectory in the destination folder
         */
        public void CompleteSave(string src, string dest, string prefixe = "")
        {
            DirectoryInfo dir = new DirectoryInfo(src);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            Directory.CreateDirectory(dest);

            foreach (FileInfo file in files)
            {
                string filePath = Path.Combine(prefixe, file.Name);
                this.files.Add(filePath);
            }

            foreach (DirectoryInfo folder in dirs)
            {
                string tmp = Path.Combine(dest, folder.Name);
                CompleteSave(folder.FullName, tmp, folder.Name);
            }
        }

        /*
         * store worksave in json file
         */ 
        public void Store()
        {
            string filePath = "../../../../saves.json";
            FileInfo json = new FileInfo(filePath);
            List<Save> listSaveForSerialize = new List<Save>();

            // if json is not empty get the save present in json
            if (json.Exists && json.Length != 0)
            {
                string jsonStringDeserialize;
                jsonStringDeserialize = File.ReadAllText(filePath);
                listSaveForSerialize = JsonConvert.DeserializeObject<List<Save>>(jsonStringDeserialize);
            }

            // if id exist, replace this
            if (listSaveForSerialize.Where(list => list.Id == Id).Count() > 0)
            {
                listSaveForSerialize.RemoveAll(list => list.Id == Id);
            }
            
            // complete the list of save and order by it
            listSaveForSerialize.Add(this);
            listSaveForSerialize.OrderBy(list => list.Id);

            // write in json file
            string jsonStringSerialize;
            jsonStringSerialize = JsonConvert.SerializeObject(listSaveForSerialize, Formatting.Indented);
            File.WriteAllText(filePath, jsonStringSerialize);
        }

        /*
         * delete worksave (not implemented)
         */ 
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
