using UnityEngine;

namespace CountMasterClone
{
    public class GateController : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text valueLabel;

        public const int LayerNumber = 7;

        private GateType gateType;
        private int rhs;

        private GateGroupController belongingGroup;

        public GateType GateType => gateType;
        public int Value => rhs;

        private void Start()
        {
            belongingGroup = transform.parent.GetComponent<GateGroupController>();
        }

        public void Initialize(GateType type, int rhs)
        {
            this.gateType = type;
            this.rhs = rhs;

            valueLabel.text = $"{type.ToValueString()}{rhs}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.LayerNumber)
            {
                // Destroy this gate, and disable other gates (because clone's collider may hit them)
                if (belongingGroup)
                {
                    belongingGroup.DisableGates();
                    Destroy(gameObject);
                }
            }
        }
    }
}