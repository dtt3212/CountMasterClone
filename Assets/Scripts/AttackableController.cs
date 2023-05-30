using UnityEngine;

namespace CountMasterClone
{
    public class AttackableController : MonoBehaviour
    {
        [SerializeField]
        private EntityLayer targetEnemyLayer;

        [SerializeField]
        protected HitReason attackReason;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int)targetEnemyLayer)
            {
                DamagableController controller = other.GetComponent<DamagableController>();
                controller?.Hit(attackReason);
            }
        }
    }
}