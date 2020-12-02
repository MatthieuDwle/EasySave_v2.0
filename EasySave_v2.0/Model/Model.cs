using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EasySave.Interface;
using EasySave.Model_Folder.Data_Type;
using Newtonsoft.Json;

namespace EasySave.Model_Folder
{
    public class Model : IModel
    {
        /*
         * suffix for log json files name
         */
        const string suffix = "-log.json";

        /*
         * erase daily log file with new log
         */
        public void CreateLog(List<Log> logs)
        {
            DateTime date = DateTime.Now;
            string day = date.ToString("dd-MM-yyyy");
            string filePath = "../../../../logs/" + day + suffix;
            string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /*
         * create new Save instance and run Store
         */ 
        public void CreateSave(bool type, string name, string src, string dest, int id)
        {
            Save newSave = new Save(name,src,dest,type,id);
            newSave.Store();
        }

        /*
         * get all logs in daily log file
         */ 
        public List<Log> GetLogs()
        {
            DateTime date = DateTime.Now;
            string day = date.ToString("dd-MM-yyyy");
            string filePath = "../../../../logs/"+ day + suffix;
            FileInfo json = new FileInfo(filePath);
            List<Log> logs = new List<Log>();

            if (json.Exists && json.Length != 0)
            {
                string jsonStringDeserialize;
                jsonStringDeserialize = File.ReadAllText(filePath);
                logs = JsonConvert.DeserializeObject<List<Log>>(jsonStringDeserialize);
            }

            return logs;
        }

        /*
         * get all save in saves.json
         */ 
        public Save[] GetSaves()
        {
            string filePath = "../../../../saves.json";
            FileInfo json = new FileInfo(filePath);
            List<Save> saveList = new List<Save>();
            Save[] saves = new Save[5];

            if (json.Exists && json.Length != 0)
            {
                string jsonStringDeserialize;
                jsonStringDeserialize = File.ReadAllText(filePath);
                saveList = JsonConvert.DeserializeObject<List<Save>>(jsonStringDeserialize);
            }

            saveList.ForEach(e =>
            {
                saves[e.Id-1] = e;
            });

            return saves;
        }
    }
}
