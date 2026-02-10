using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

namespace Kweek
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        public delegate void OptionDelegate(string key);

        [Tooltip("Folder in the \"My Games\" or \"Saved Games\" folder in the persons \"My Documents\"")]
        [SerializeField]
        private string m_OptionsFilename;
        private DirectoryInfo m_RootDirectory;

        private Dictionary<string, string> m_Options;

        public event OptionDelegate OptionChangedEvent;

        protected override void Awake()
        {
            base.Awake();
            m_Options = new Dictionary<string, string>();
        }

        private void Start()
        {
            DirectoryInfo rootRootDirectory = new DirectoryInfo(SaveGameLocation.getSaveGameDirectory());
            m_RootDirectory = ExtentionMethods.FindOrCreateDirectory(rootRootDirectory, SaveGameManager.Instance.SaveGameFolder);

            LoadOptionsFromDisk();
        }


        //Mutators
        public void SetOption(string key, int value)
        {
            SetOption(key, value.ToString());
        }

        public void SetOption(string key, float value)
        {
            SetOption(key, value.ToString());
        }

        public void SetOption(string key, string value)
        {
            if (m_Options.ContainsKey(key))
            {
                m_Options[key] = value;
            }
            else
            {
                m_Options.Add(key, value);
            }

            if (OptionChangedEvent != null)
                OptionChangedEvent(key);
        }


        //Accessors
        public int GetOptionAsInt(string key)
        {
            if (m_Options.ContainsKey(key))
            {
                int result = 0;
                bool success = int.TryParse(m_Options[key], out result);

                if (success)
                    return result;
            }

            return 0;
        }

        public bool GetOptionAsInt(string key, out int result)
        {
            result = 0;

            if (m_Options.ContainsKey(key))
            {
                return int.TryParse(m_Options[key], out result);
            }

            return false;
        }

        public float GetOptionAsFloat(string key)
        {
            if (m_Options.ContainsKey(key))
            {
                float result = 0.0f;
                bool success = float.TryParse(m_Options[key], out result);

                if (success)
                    return result;
            }

            return 0.0f;
        }

        public bool GetOptionAsFloat(string key, out float result)
        {
            result = 0;

            if (m_Options.ContainsKey(key))
            {
                return float.TryParse(m_Options[key], out result);
            }

            return false;
        }

        public string GetOptionAsString(string key)
        {
            if (m_Options.ContainsKey(key))
            {
                return m_Options[key];
            }

            return "";
        }

        public bool GetOptionAsString(string key, out string result)
        {
            result = "";

            if (m_Options.ContainsKey(key))
            {
                result = m_Options[key];
                return true;
            }

            return false;
        }


        //Serialization
        public void SaveOptionsToDisk()
        {
            try
            {
                JSONClass rootObject = new JSONClass();
                Serialize(rootObject);

                //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
                string jsonStr = "";
#if !UNITY_EDITOR
                jsonStr = rootObject.ToString();
#else
                jsonStr = rootObject.ToJSON(0);
#endif

                File.WriteAllText(m_RootDirectory.FullName + "/" + m_OptionsFilename, jsonStr);
            }
            catch (Exception e)
            {
                //The file was probably not found!
                UnityEngine.Debug.LogWarning("Error in SaveOptionsToDisk: " + e.Message);
            }
        }

        public bool LoadOptionsFromDisk()
        {
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(m_RootDirectory.FullName + "/" + m_OptionsFilename);
            }
            catch (Exception e)
            {
                //The file was probably not found!
                UnityEngine.Debug.LogWarning("Error in LoadOptionsFromDisk: " + e.Message);
                return false;
            }

            if (fileText == "")
                return true;

            try
            {
                JSONNode rootNode = JSON.Parse(fileText);
                Deserialize(rootNode);
            }
            catch (Exception e)
            {
                //There were errors parsing the file!
                UnityEngine.Debug.LogWarning("Error in LoadOptionsFromDisk: " + e.Message);
                return false;
            }

            return true;
        }

        private void Serialize(JSONClass rootNode)
        {
            //Save all the options
            foreach (KeyValuePair<string, string> option in m_Options)
            {
                JSONData nameData = new JSONData(option.Value);
                rootNode.Add(option.Key, nameData);
            }
        }

        private void Deserialize(JSONNode jsonNode)
        {
            List<string> keys = jsonNode.Keys;
            if (keys == null)
            {
                throw new Exception("Options file doesn't start with a JSONClass");
            }

            foreach (string key in keys)
            {
                SetOption(key, jsonNode[key].Value);
            }
        }
    }
}