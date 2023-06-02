using UnityEngine;

namespace CountMasterClone
{
    public class StairStepManager : MonoBehaviour
    {
        [SerializeField]
        private Transform nextStepPoint;

        [SerializeField]
        private Transform destPlatformPoint;

        [SerializeField]
        private TMPro.TMP_Text multiplierText;

        [SerializeField]
        private Transform cloneRester;

        [SerializeField]
        private float multiplier;

        public void Initialize(Material material, float multiplier)
        {
            this.multiplier = multiplier;
            multiplierText.text = $"x{multiplier:0.0}";
            
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = material;
        }

        public Vector3 NextStepPoint => nextStepPoint.position;
        public Vector3 DestPlatformPoint => destPlatformPoint.position;
        public Transform CloneRester => cloneRester;
        public float Multiplier => multiplier;
    }
}