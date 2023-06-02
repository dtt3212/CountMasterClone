using UnityEngine;

namespace CountMasterClone
{
    public class FinishLineInfo : MonoBehaviour
    {
        [SerializeField]
        private FinishDestinationType destinationType;

        public FinishDestinationType DestinationType => destinationType;

        public void Initialize(FinishDestinationType type)
        {
            destinationType = type;
        }
    }
}