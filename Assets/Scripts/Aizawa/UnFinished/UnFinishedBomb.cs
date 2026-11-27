using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Yajima;

namespace Aizawa
{
    /// <summary>
    /// 爆弾のやり残しオブジェクト
    /// </summary>
    public class UnFinishedBomb : Unfinished
    {
        [SerializeField]
        private GameObject _debrisAnimator;

        public override void Entry()
        {
            base.Entry();
            var p = transform.position;
            p.y = 0;
            transform.position = p;
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

            _debrisAnimator.SetActive(true);
            transform.DORotate(new(0, 180, 0), 0.2f);
            await UniTask.WaitUntil(() => !_debrisAnimator.activeSelf);

            col.isTrigger = false;

            base.InteractExit(player);
        }
    }
}