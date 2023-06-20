using UnityEngine;

namespace CountMasterClone
{
    public class StairDestinationInfo : MonoBehaviour
    {
        [SerializeField]
        private ObjectCoinValue chestValue;

        public int ChestValue
        {
            get => chestValue.value;
            set => chestValue.value = value;
        }
    }
}