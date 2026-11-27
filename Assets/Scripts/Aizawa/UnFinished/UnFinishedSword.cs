using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            if (IsStopped || _interacted) return;
            _entered = false;
            var col = GetComponent<Collider>();
            col.isTrigger = true;

            _sword.gameObject.SetActive(true);
            transform.DORotate(new(0, 180, 0), 0.2f);
            _sword.angularVelocity = Random.Range(1, 5) * 200;
            _sword.linearVelocityY = 10;
            await UniTask.WaitUntil(() => _sword.transform.position.y < transform.position.y);
            var p = _sword.transform.localPosition;
            p.y = _swordHeight;
            _sword.transform.localPosition = p;
            _sword.transform.localEulerAngles = Vector3.zero;

            col.isTrigger = false;
            _sword.gameObject.SetActive(false);

            base.InteractExit(player);
        }
    }
}