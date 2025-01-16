using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;

namespace BT.Save
{
    public class LoadSaveData : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RSE_LoadData rseLoad;
        [SerializeField] private RSE_SaveData rseSave;
        [SerializeField] private RSE_ClearData rseClear;
        [SerializeField] private RSO_ContentSaved rsoContentSave;
        
        private string filepath;

        private void OnEnable()
        {
            rseLoad.action += LoadFromJson;
            rseSave.action += SaveToJson;
            rseClear.action += ClearContent;
        }

        private void OnDisable()
        {
            rseLoad.action -= LoadFromJson;
            rseSave.action -= SaveToJson;
        }

        private void Start()
        {
            filepath = Application.persistentDataPath + "/Save.json";

            if (FileAlreadyExist()) LoadFromJson();
            else SaveToJson();
        }

        private void SaveToJson()
        {
            string infoData = JsonUtility.ToJson(rsoContentSave.Value);
            File.WriteAllText(filepath, infoData);
        }
        private void LoadFromJson()
        {
            string infoData = System.IO.File.ReadAllText(filepath);
            rsoContentSave.Value = JsonUtility.FromJson<ContentSaved>(infoData);
        }
        private void ClearContent()
        {
            rsoContentSave.Value = new();
            SaveToJson();
        }

        private bool FileAlreadyExist()
        {
            return File.Exists(filepath);
        }        
    }   
}
