using UnityEngine;
using System.IO;

namespace BT.Save
{
    public class LoadSaveData : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RSE_LoadData rseLoad;
        [SerializeField] private RSE_SaveData rseSave;
        [SerializeField] private RSE_ClearData rseClear;
        [SerializeField] private RSO_ContentSaved rsoContentSave;
        
        private string _filepath;

        private void OnEnable()
        {
            rseLoad.Action += LoadFromJson;
            rseSave.Action += SaveToJson;
            rseClear.Action += ClearContent;
        }

        private void OnDisable()
        {
            rseLoad.Action -= LoadFromJson;
            rseSave.Action -= SaveToJson;
        }

        private void Start()
        {
            _filepath = Application.persistentDataPath + "/Save.json";

            if (FileAlreadyExist()) LoadFromJson();
            else SaveToJson();
        }

        private void SaveToJson()
        {
            string infoData = JsonUtility.ToJson(rsoContentSave.Value);
            File.WriteAllText(_filepath, infoData);
        }
        private void LoadFromJson()
        {
            string infoData = File.ReadAllText(_filepath);
            rsoContentSave.Value = JsonUtility.FromJson<ContentSaved>(infoData);
        }
        private void ClearContent()
        {
            rsoContentSave.Value = new ContentSaved();
            SaveToJson();
        }

        private bool FileAlreadyExist()
        {
            return File.Exists(_filepath);
        }        
    }   
}
