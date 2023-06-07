using DG.Tweening;
using UnityEngine.UIElements;

namespace Radrat.Tweening
{
    public static class UITookitStyleTween
    {
        public static DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> DOOpacity(this IStyle style, float destValue, float duration) => DOTween.To(() => style.opacity.value, value => style.opacity = value, destValue, duration);
    }
}