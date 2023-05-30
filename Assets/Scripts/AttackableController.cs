using UnityEngine;

namespace CountMasterClone
{
    public class AttackableController : MonoBehaviour
    {
        [SerializeField]
        private int targetEnemyLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == targetEnemyLayer)
            {
                DamagableController controller = other.GetComponent<DamagableController>();
                controller?.Hit();
            }
        }
    }
}