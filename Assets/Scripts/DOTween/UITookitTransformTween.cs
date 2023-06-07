using DG.Tweening;

using UnityEngine;
using UnityEngine.UIElements;

namespace Radrat.Tweening
{
    public static class UITookitTransformTween
    {
        public static DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> DOScale(this ITransform transform, Vector3 dest, float duration) => DOTween.To(() => transform.scale, value => transform.scale = value, dest, duration);
    }
}