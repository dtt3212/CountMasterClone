using UnityEngine;

namespace CountMasterClone
{
    public class StickmanBodyManipulator : MonoBehaviour
    {
        [SerializeField]
        private StickmanDatabase database;

        [SerializeField]
        private ValuableState valuableState;

        private void Start()
        {
            GameObject body = database.stickmans[valuableState.activeStickman].previewPrefab;
            Instantiate(body, transform, false);
        }
    }
}