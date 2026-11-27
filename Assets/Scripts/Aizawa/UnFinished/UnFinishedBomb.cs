using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kato;
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

            if(Random.Range(0, 3) == 1)
            {
                AudioManager.instance.Play("BombMissSE");
                _debrisAnimator.SetActive(true);
            }
            else
            {
                AudioManager.instance.Play("BombCutSE");
            }
            await transform.DORotate(new(0, 180, 0), 0.2f).AsyncWaitForCompletion();
            await UniTask.WaitUntil(() => !_debrisAnimator.activeSelf);

            col.isTrigger = false;

            base.InteractExit(player);
        }
    }
}