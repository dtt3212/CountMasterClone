using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CountMasterClone
{
    public abstract class SerializableState : ScriptableObject
    {
        protected abstract string SaveName { get; }
        private string SavePath => Path.Join(Application.persistentDataPath, SaveName);

        private void Awake()
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            JsonUtility.FromJsonOverwrite(File.ReadAllText(SavePath), this);
        }

        public void Save()
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(this));

#if UNITY_WEBGL
            // Fast solution for WebGL flushing
            Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
        }
    }
}