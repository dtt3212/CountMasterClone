using System.Collections.Generic;
using UnityEngine;

namespace CountMasterClone
{
    [CreateAssetMenu(fileName = "StickmanDatabase", menuName = "ScriptableObjects/StickmanDatabase")]
    public class StickmanDatabase: ScriptableObject
    {
        public StickmanSellData[] stickmans;
    }
}