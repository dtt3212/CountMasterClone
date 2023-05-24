using UnityEngine;
using UnityEngine.InputSystem;

namespace CountMasterClone
{
    public class GateController : MonoBehaviour
    {
        private GateType gateType;
        private int rhs;

        public GateType GateType => gateType;
        public int Value => rhs;

        private void Start()
        {
            
        }

        public void Initialize(GateType type, int rhs)
        {
            this.gateType = type;
            this.rhs = rhs;
        }
    }
}