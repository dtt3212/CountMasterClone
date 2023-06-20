using Cinemachine;
using System;
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

        [SerializeField]
        private PlayerGroupManager playerGroupManager;

        [SerializeField]
        private GameObject normalViewCinecam;

        [SerializeField]
        private GameObject stairViewCinecam;

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

        private void Awake()
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

        private void OnMove(InputValue value)
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
