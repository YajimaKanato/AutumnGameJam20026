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

        bool IsFinished => _fadeDirector !=null && _fadeDirector.time >= _fadeDirector.duration;

        public async UniTask FadeIn(CancellationToken cancellationToken = default)
        {
            if (_fadeDirector == null) return;
            if (_fadeInAsset == null) return;

            _fadeDirector.playableAsset = _fadeInAsset;
            _fadeDirector.initialTime = 0;
            _fadeDirector.extrapolationMode = DirectorWrapMode.Hold;
            _fadeDirector.Play();
            await UniTask.WaitUntil(() => _fadeDirector == null || IsFinished, cancellationToken: cancellationToken);
        }

        public async UniTask FadeOut()
        {
            if (_fadeDirector == null) return;
            if (_fadeOutAsset == null) return;

            _fadeDirector.playableAsset = _fadeOutAsset;
            _fadeDirector.initialTime = 0;
            _fadeDirector.Play();
            await UniTask.WaitUntil(() => IsFinished);
        }
    }
}
