using UnityEngine;
using UnityEngine.InputSystem;

namespace CountMasterClone
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float horizontalMoveSpeedPerSec = 10.0f;

        [SerializeField]
        private float attackSpeed = 20.0f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private PlayerGroupController playerGroupController;

        private void OnMove(InputValue value)
        {
            if (playerGroupController.AggressiveMode)
            {
                return;
            }

            Vector2 pos = value.Get<Vector2>();
            Vector3 realWorldPos = gameCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cameraDistance));

            Vector3 direction = (realWorldPos - transform.position);
            direction.Scale(Vector3.right);

            transform.localPosition += direction.normalized * horizontalMoveSpeedPerSec * Time.deltaTime; 
        }
        private void Update()
        {
            if (!playerGroupController.AggressiveMode)
            {
                transform.localPosition += new Vector3(0, 0, verticalMoveSpeedPerSec) * Time.deltaTime;
            }
        }
    }
}
