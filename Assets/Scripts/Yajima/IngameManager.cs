using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yajima
{
    public class IngameManager : MonoBehaviour
    {
        [SerializeField] LightMoveManager _lightMoveManager;
        [SerializeField] LifeTimePresenter _lifeTimePresenter;
        [SerializeField] LifeEnergyPresenter _lifeEnergyPresenter;
        [SerializeField] CriticalEventPresenter _criticalEventPresenter;
        [Header("Player")]
        [SerializeField] Player _player;
        [Header("Score")]
        [SerializeField] IngameScorePresenter _scorePresenter;
        [SerializeField] UnfinishedPool _unfinishedPool;
        List<IDisposable> _disposables = new();
        static readonly HashSet<IngameBehaviour> _ingameObjects = new();
        static bool _isPaused;
        static bool _isFinished;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetIngameState()
        {
            _ingameObjects.Clear();
            _isPaused = false;
            _isFinished = false;
        }

        internal static void RegisterObj(IngameBehaviour obj)
        {
            if (!_ingameObjects.Add(obj)) return;

            if (_isFinished)
                obj.Finish();
            else if (_isPaused)
                obj.Pause();
            else
                obj.Resume();
        }

        internal static void UnregisterObj(IngameBehaviour obj) => _ingameObjects.Remove(obj);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _scorePresenter?.Init();
            _lifeTimePresenter?.Init();
            _lifeEnergyPresenter?.Init();
            _criticalEventPresenter?.StartSchedule(_lifeTimePresenter?.RemainingTime?.CurrentValue ?? 0f);
        }

        private void OnDestroy()
        {
            _isPaused = false;
            _isFinished = false;
        }

        private void OnEnable()
        {
            var remainingTime = _lifeTimePresenter?.RemainingTime;
            if (_criticalEventPresenter != null && remainingTime != null)
            {
                if (_lifeEnergyPresenter != null)
                    _disposables.Add(_criticalEventPresenter.IsCritical.Subscribe(_lifeEnergyPresenter.SetCriticalCondition));

                _disposables.Add(remainingTime.Subscribe(_criticalEventPresenter.UpdateRemainingTime));
            }

            if (_unfinishedPool != null && _scorePresenter != null)
                _unfinishedPool.OnInteractedPlayer += _scorePresenter.AddScore;
            if (_lifeEnergyPresenter != null)
            {
                _lifeEnergyPresenter.Depleted += FinishGame;
                if (_unfinishedPool != null)
                    _unfinishedPool.OnInteractedPlayer += _lifeEnergyPresenter.OnUnfinishedResolved;
            }

            if (_player == null) return;
            _disposables.Add(_player.OpenedMenu.Subscribe(OpenMenu));
            _disposables.Add(_player.LightMove.Subscribe(LightMove));
        }

        private void OnDisable()
        {
            if (_unfinishedPool != null && _scorePresenter != null)
                _unfinishedPool.OnInteractedPlayer -= _scorePresenter.AddScore;
            if (_lifeEnergyPresenter != null)
            {
                _lifeEnergyPresenter.Depleted -= FinishGame;
                if (_unfinishedPool != null)
                    _unfinishedPool.OnInteractedPlayer -= _lifeEnergyPresenter.OnUnfinishedResolved;
            }

            foreach (var item in _disposables)
            {
                item?.Dispose();
            }
            _disposables?.Clear();
            _lifeEnergyPresenter?.SetCriticalCondition(false);
            OpenMenu(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        async void StartGame()
        {

        }

        void OpenMenu(bool openMenu)
        {
            if (_isFinished || openMenu == _isPaused) return;

            _isPaused = openMenu;
            foreach (var obj in new List<IngameBehaviour>(_ingameObjects))
            {
                if (obj == null || !_ingameObjects.Contains(obj)) continue;

                if (openMenu)
                    obj.Pause();
                else
                    obj.Resume();
            }
        }

        void LightMove(bool lightMoved)
        {
            _lightMoveManager?.ChangeLightMoving(lightMoved);
        }

        async void BackTitle()
        {

        }

        async void Restart()
        {

        }

        void FinishGame()
        {
            if (_isFinished) return;

            _isFinished = true;
            _isPaused = false;
            foreach (var obj in new List<IngameBehaviour>(_ingameObjects))
            {
                if (obj != null && _ingameObjects.Contains(obj))
                    obj.Finish();
            }
        }
    }
}
