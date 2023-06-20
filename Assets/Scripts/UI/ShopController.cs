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
        private StickmanDatabase database;

        [SerializeField]
        private float fadeInOutDuration = 0.4f;

        [SerializeField]
        private float scrollMultiplier = 2.0f;

        [SerializeField]
        private StickmanPreviewController previewController;

        [SerializeField]
        private ValuableState valuableState;

        [SerializeField]
        private float moneyReduceAnimationDuration = 0.5f;

        [SerializeField]
        private float notBuyableAnimationDuration = 0.3f;

        private VisualElement root;
        private Label moneyLabel;
        private Button purchaseBtn;
        private RenderTexture renderTexture;
        private bool isTrackingScroll = false;

        public event Action Closed;

        private void Start()
        {
            root = document.rootVisualElement;
            Image previewImage = root.Q<Image>("ve_preview_image");

            Button backBtn = root.Q<Button>("action_back");
            backBtn.clicked += OnBackButton;

            moneyLabel = root.Q<Label>("lb_money_value");
            purchaseBtn = root.Q<Button>("action_purchase");

            root.style.display = DisplayStyle.None;
            root.style.opacity = 0.0f;

            previewController.ViewChanged += i =>
            {
                if (valuableState.ownedStickmans.Contains(i))
                {
                    if (valuableState.activeStickman.Value == i)
                    {
                        purchaseBtn.text = "Selected";
                    }
                    else
                    {
                        purchaseBtn.text = "Select";
                    }
                }
                else
                {
                    purchaseBtn.text = $"${database.stickmans[i].cost}";
                }
            };

            purchaseBtn.clicked += OnPurchaseClicked;
        }

        private void OnPurchaseClicked()
        {
            int active = previewController.CurrentStickman;
            if (valuableState.ownedStickmans.Contains(active))
            {
                if (valuableState.activeStickman.Value != active)
                {
                    purchaseBtn.text = "Selected";

                    valuableState.activeStickman.Value = active;
                    valuableState.Save();
                }
            }
            else
            {
                int cost = database.stickmans[active].cost;
                int currentHaving = valuableState.money;

                if (valuableState.Purchase(cost))
                {
                    valuableState.ownedStickmans.Add(active);
                    valuableState.Save();

                    purchaseBtn.text = "Select";

                    DOVirtual.Int(currentHaving, valuableState.money, moneyReduceAnimationDuration, f =>
                    {
                        moneyLabel.text = $"{f}";
                    });
                }
                else
                {
                    purchaseBtn.transform.DOShakePosition(notBuyableAnimationDuration, 20, 10, 90);
                }
            }
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

            previewController.Initialize(database);

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

        private void SetupShow()
        {
            moneyLabel.text = $"{valuableState.money}";
            if (!renderTexture)
            {
                StartCoroutine(SetupPreview());
            }
        }

        public void Show()
        {
            root.style.display = DisplayStyle.Flex;
            root.style.DOOpacity(1.0f, fadeInOutDuration)
                .OnComplete(() =>
                {
                    SetupShow();
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