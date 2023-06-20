using UnityEngine;

namespace CountMasterClone
{
    [CreateAssetMenu(menuName = "ScriptableObjects/StickmanDatabase")]
    public class StickmanSellData : ScriptableObject
    {
        public GameObject previewPrefab;
        public int cost;
    }
}