using UnityEngine;

namespace CountMasterClone
{
    public class BillboardLabelController : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text text;

        [SerializeField]
        private Camera lookatCamera;

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public Camera LookatCamera
        {
            get => lookatCamera;
            set => lookatCamera = value;
        }

        private void Update()
        {
            if (lookatCamera != null)
            {
                transform.forward = lookatCamera.transform.forward;
            }
        }
    }
}