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