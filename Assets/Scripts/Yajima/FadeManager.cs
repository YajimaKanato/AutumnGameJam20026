using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Yajima
{
    public class FadeManager : MonoBehaviour
    {
        [SerializeField] PlayableDirector _fadeDirector;
        [SerializeField] TimelineAsset _fadeInAsset;
        [SerializeField] TimelineAsset _fadeOutAsset;

        public UniTask FadeIn(CancellationToken cancellationToken = default)
            => PlayFade(_fadeInAsset, cancellationToken);

        public UniTask FadeOut(CancellationToken cancellationToken = default)
            => PlayFade(_fadeOutAsset, cancellationToken);

        async UniTask PlayFade(TimelineAsset asset, CancellationToken cancellationToken)
        {
            var director = _fadeDirector;
            if (director == null || !director.isActiveAndEnabled || asset == null) return;

            director.Stop();
            director.initialTime = 0;
            director.time = 0;
            director.Play(asset, DirectorWrapMode.Hold);
            await UniTask.WaitUntil(() => director == null || !director.isActiveAndEnabled
                || director.time >= director.duration, cancellationToken: cancellationToken);
        }
    }
}
