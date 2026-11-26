using UnityEngine;

namespace Yajima
{
    public abstract class IngameBehaviour : MonoBehaviour, IPause
    {
        protected bool IsPaused { get; private set; }
        protected bool IsFinished { get; private set; }
        protected bool IsStopped => IsPaused || IsFinished;

        protected virtual void OnEnable() => IngameManager.RegisterObj(this);
        protected virtual void OnDisable() => IngameManager.UnregisterObj(this);

        public void Pause()
        {
            if (IsStopped) return;

            IsPaused = true;
            OnPaused();
        }

        public void Resume()
        {
            if (IsFinished || !IsPaused) return;

            IsPaused = false;
            OnResumed();
        }

        public void Finish()
        {
            if (IsFinished) return;

            bool wasPaused = IsPaused;
            IsFinished = true;
            IsPaused = false;
            if (!wasPaused) OnPaused();
            OnFinished();
        }

        protected virtual void OnPaused() { }
        protected virtual void OnResumed() { }
        protected virtual void OnFinished() { }
    }
}
