using UnityEngine;

namespace CountMasterClone
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField]
        private MenuController menuController;

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private PlatformSpawner spawner;

        [SerializeField]
        private PlayState playState;

        public void RestartGame()
        {
            spawner.Clean();
            spawner.Spawn();

            menuController.Show();
        }

        private void AdvanceGame()
        {
            playState.level++;
            playState.Save();

            RestartGame();
        }

        private void Start()
        {
            if (Application.isMobilePlatform)
            {
                Application.targetFrameRate = 60;
            }
            else
            {
                QualitySettings.vSyncCount = 1;
            }

            playerController.GameEnded += (win) =>
            {
                if (win)
                {
                    AdvanceGame();
                }
                else
                {
                    RestartGame();
                }
            };

            RestartGame();
        }
    }
}