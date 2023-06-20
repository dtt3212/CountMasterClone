using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CountMasterClone
{
    [CreateAssetMenu(fileName = "ValuableState", menuName = "ScriptableObjects/ValuableState")]
    public class ValuableState : ScriptableObject
    {
        public int money = 350;

        public int activeStickman;

        public List<int> ownedStickmans = new();

        private string SavePath => Path.Join(Application.persistentDataPath, "valuableState.json");

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
        }

        public bool Purchase(int cost)
        {
            if (money < cost)
            {
                return false;
            }

            money -= cost;
            return true;
        }
    }
}