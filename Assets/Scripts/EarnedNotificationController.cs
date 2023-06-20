using DG.Tweening;
using UnityEngine;

namespace CountMasterClone
{
    public class EarnedNotificationController : MonoBehaviour
    {
        private TMPro.TMP_Text text;

        [SerializeField]
        private float animationDuration = 0.5f;

        [SerializeField]
        private float displayDuration = 2.0f;

        private Vector3 targetPosition;

        public void Display(string target)
        {
            DOTween.Init(this);

            text = GetComponent<TMPro.TMP_Text>();
            text.color = new Color(0, 0, 0, 0);

            targetPosition = transform.position;
            transform.position -= new Vector3(0, 0.5f, 0);

            text.text = target;

            DOVirtual.Color(new Color(0, 0, 0, 0), Color.black, animationDuration, color => text.color = color);
            DOTween.Sequence()
                .Append(transform.DOMove(targetPosition, animationDuration))
                .AppendInterval(displayDuration)
                .Append(DOVirtual.Color(Color.black, new Color(0, 0, 0, 0), animationDuration, color => text.color = color))
                .OnComplete(() => Destroy(gameObject));
        }
    }
}