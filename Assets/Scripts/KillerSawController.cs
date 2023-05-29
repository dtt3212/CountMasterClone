using UnityEngine;

namespace CountMasterClone
{
    public class KillerSawController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == DamagableController.LayerNumber)
            {
                DamagableController controller = other.GetComponent<DamagableController>();
                controller?.InstantKill();
            }
        }
    }
}