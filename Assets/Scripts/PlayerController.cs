using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace CountMasterClone
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float horizontalMoveSpeedPerSec = 10.0f;

        [SerializeField]
        private float touchHorizontalMoveFactor = 2.0f;

        [SerializeField]
        private float attackSpeed = 20.0f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private PlayerGroupController playerGroupController;

        [SerializeField]
        private PlayerGroupManager playerGroupManager;

        [SerializeField]
        private GameObject normalViewCinecam;

        [SerializeField]
        private GameObject stairViewCinecam;

        private Vector3 lastTouchPosition;
        private float touchTimestamp;

        public float PlatformWidth { get; set; }

        public event System.Action<bool> GameEnded;

        private bool kickedUp = false;

        public void TotalReset()
        {
            stairViewCinecam.SetActive(false);
            normalViewCinecam.SetActive(true);

            playerGroupController.TotalReset();
            kickedUp = false;
        }

        private void Start()
        {
            TotalReset();
        }

        public void Kickup()
        {
            kickedUp = true;
            playerGroupController.Kickup();

            playerGroupController.GameEnded += (win) =>
            {
                GameEnded?.Invoke(win);
            };
        }

        private void OnTouchMove(InputValue value)
        {
            if (!kickedUp)
            {
                return;
            }

            if (playerGroupController.AggressiveMode || playerGroupController.ReachedFinish)
            {
                return;
            }

            TouchState touchValue = value.Get<TouchState>();

            if (touchValue.phase == UnityEngine.InputSystem.TouchPhase.Ended || touchValue.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                touchTimestamp = 0;
                return;
            }

            Vector2 pos = touchValue.position;
            Vector3 realWorldPos = gameCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cameraDistance));

            float current = Time.time;

            if (touchTimestamp == 0)
            {
                touchTimestamp = current;
                lastTouchPosition = realWorldPos;

                return;
            }

            Vector3 direction = (realWorldPos - lastTouchPosition);
            direction.Scale(Vector3.right * touchHorizontalMoveFactor);

            lastTouchPosition = realWorldPos;
            touchTimestamp = current;

            if (Mathf.Abs(direction.magnitude) <= 0.2f)
            {
                return;
            }

            transform.localPosition += direction;

            float groupWidth = playerGroupManager.GroupWidth;
            float xClamped = Mathf.Clamp(transform.localPosition.x, -(PlatformWidth - groupWidth) / 2.0f, (PlatformWidth - groupWidth) / 2.0f);

            transform.localPosition = new Vector3(xClamped, transform.localPosition.y, transform.localPosition.z);
        }

        private void OnMouseMove(InputValue value)
        {
            if (!kickedUp)
            {
                return;
            }

            if (playerGroupController.AggressiveMode || playerGroupController.ReachedFinish)
            {
                return;
            }

            Vector2 pos = value.Get<Vector2>();
            Vector3 realWorldPos = gameCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cameraDistance));

            Vector3 direction = (realWorldPos - transform.position);
            direction.Scale(Vector3.right);

            if (Mathf.Abs(direction.magnitude) <= 0.2f)
            {
                return;
            }

            transform.localPosition += direction.normalized * horizontalMoveSpeedPerSec * Time.deltaTime;
            float groupWidth = playerGroupManager.GroupWidth;
            float xClamped = Mathf.Clamp(transform.localPosition.x, -(PlatformWidth - groupWidth) / 2.0f, (PlatformWidth - groupWidth) / 2.0f);
        
            transform.localPosition = new Vector3(xClamped, transform.localPosition.y, transform.localPosition.z);
        }

        private void Update()
        {
            if (!kickedUp || playerGroupController.ReachedEndgame)
            {
                return;
            }

            if (playerGroupController.ReachedFinish)
            {
                normalViewCinecam.SetActive(false);

                switch (playerGroupController.FinishDestinationType)
                {
                    case FinishDestinationType.Staircase:
                        stairViewCinecam.SetActive(true);
                        break;
                }
            }

            if (!playerGroupController.AggressiveMode)
            {
                transform.localPosition += new Vector3(0, 0, verticalMoveSpeedPerSec) * Time.deltaTime;
            }
        }
    }
}
