using DG.Tweening;

using UnityEngine;
using UnityEngine.UIElements;

namespace Radrat.Tweening
{
    public static class UITookitTransformTween
    {
        public static DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> DOScale(this ITransform transform, Vector3 dest, float duration) => DOTween.To(() => transform.scale, value => transform.scale = value, dest, duration);
        public static DG.Tweening.Core.TweenerCore<Vector3, Vector3[], DG.Tweening.Plugins.Options.Vector3ArrayOptions> DOShakePosition(this ITransform transform, float duration, float strength = 90, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full) => DOTween.Shake(() => transform.position, value => transform.position = value, duration, strength, vibrato, randomness, true, fadeOut, randomnessMode);
        public static DG.Tweening.Core.TweenerCore<Vector3, Vector3[], DG.Tweening.Plugins.Options.Vector3ArrayOptions> DOShakeRotation(this ITransform transform, float duration, float strength = 90, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full) => DOTween.Shake(() => transform.rotation.eulerAngles, value => transform.rotation = Quaternion.Euler(0, 0, value.z), duration, strength, vibrato, randomness, false, fadeOut, randomnessMode);
    }
}