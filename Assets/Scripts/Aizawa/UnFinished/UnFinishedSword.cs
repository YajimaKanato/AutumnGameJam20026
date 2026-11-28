using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kato;
using UnityEngine;
using Yajima;

namespace Aizawa
{
    /// <summary>
    /// 勇者の剣のやり残しオブジェクト
    /// </summary>
    public class UnFinishedSword : Unfinished
    {
        [SerializeField]
        private Rigidbody2D _sword;

        private float _swordHeight;

        public override void Entry()
        {
            base.Entry();
            _swordHeight = _sword.transform.localPosition.y;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override async void InteractExit(Player player)
        {
            if (IsStopped || !_entered || _interacted) return;
            _entered = false;

            AudioManager.instance?.Play("SwordSE");
            _sword.gameObject.SetActive(true);
            transform.DORotate(new(0, 180, 0), 0.2f);
            _sword.angularVelocity = Random.Range(1, 5) * 200;
            _sword.linearVelocity = Vector2.up * 10f;
            if (await UniTask.WaitUntil(() => _sword.transform.position.y < transform.position.y,
                    cancellationToken: destroyCancellationToken).SuppressCancellationThrow()) return;
            var p = _sword.transform.localPosition;
            p.y = _swordHeight;
            _sword.transform.localPosition = p;
            _sword.transform.localEulerAngles = Vector3.zero;

            _sword.gameObject.SetActive(false);

            if (await UniTask.WaitUntil(() => !IsPaused,
                    cancellationToken: destroyCancellationToken).SuppressCancellationThrow()) return;

            base.InteractExit(player);
        }
    }
}
