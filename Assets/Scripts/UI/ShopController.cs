using DG.Tweening;
using Radrat.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace CountMasterClone
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField]
        private UIDocument document;

        [SerializeField]
        private Camera previewCamera;

        [SerializeField]
        private StickmanSellData[] sellDatas;

        [SerializeField]
        private float fadeInOutDuration = 0.4f;

        [SerializeField]
        private float scrollMultiplier = 2.0f;

        [SerializeField]
        private StickmanPreviewController previewController;

        private VisualElement root;
        private RenderTexture renderTexture;
        private bool isTrackingScroll = false;

        public event Action Closed;

        private void Start()
        {
            root = document.rootVisualElement;
            Image previewImage = root.Q<Image>("ve_preview_image");

            Button backBtn = root.Q<Button>("action_back");
            backBtn.clicked += OnBackButton;

            root.style.display = DisplayStyle.None;
            root.style.opacity = 0.0f;
        }

        private IEnumerator SetupPreview()
        {
            yield return null;

            VisualElement root = document.rootVisualElement;
            Image previewImage = root.Q<Image>("ve_preview_image");

            previewCamera.gameObject.SetActive(true);

            Vector2 renderTextureSize = new Vector2(previewImage.resolvedStyle.width, previewImage.resolvedStyle.height);
            RenderTexture renderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 32);

            previewCamera.targetTexture = renderTexture;
            previewImage.image = renderTexture;

            previewController.Initialize(sellDatas);

            previewImage.RegisterCallback<PointerDownEvent>(evt =>
            {
                isTrackingScroll = true;
            });

            previewImage.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!isTrackingScroll)
                {
                    return;
                }

                previewController.ScrollInViewportUnits = ((evt.localPosition.x - renderTextureSize.x / 2) / renderTextureSize.x) * scrollMultiplier;
            });

            previewImage.RegisterCallback<PointerOutEvent>(evt =>
            {
                OnPointerRelease();
            });

            previewImage.RegisterCallback<PointerUpEvent>(evt =>
            {
                OnPointerRelease();
            });
        }

        private void OnPointerRelease()
        {
            if (isTrackingScroll)
            {
                previewController.EatScroll();
            }

            isTrackingScroll = false;
        }

        public void Show()
        {
            root.style.display = DisplayStyle.Flex;
            root.style.DOOpacity(1.0f, fadeInOutDuration)
                .OnComplete(() =>
                {
                    if (!renderTexture)
                    {
                        StartCoroutine(SetupPreview());
                    }
                });
        }

        private void OnBackButton()
        {
            root.style.DOOpacity(0.0f, fadeInOutDuration)
                .OnComplete(() =>
                {
                    root.style.display = DisplayStyle.None;
                    previewCamera.gameObject.SetActive(false);
                    Closed?.Invoke();
                });
        }
    }
}