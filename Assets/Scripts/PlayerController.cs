using UnityEngine;
using UnityEngine.InputSystem;

namespace CountMasterClone
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private GameObject clonePrefab;

        private int cloneCount = 1;

        private void OnMove(InputValue value)
        {
            Vector2 mousePosition = value.Get<Vector2>();
            Vector2 pointInWorld = gameCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cameraDistance));

            transform.position = new Vector3(pointInWorld.x, transform.position.y, transform.position.z);
        }

        private void AddClone(int count)
        {
        }

        private void Start()
        {
        }

        private void Update()
        {
            transform.position += new Vector3(0, 0, verticalMoveSpeedPerSec * Time.deltaTime);
        }
    }
}
