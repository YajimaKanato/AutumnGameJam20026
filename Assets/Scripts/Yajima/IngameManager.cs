using Aizawa;
using Cysharp.Threading.Tasks;
using Kato;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Yajima
{
    public class IngameManager : MonoBehaviour
    {
        [Header("LifeTime")]
        [SerializeField] LightMoveManager _lightMoveManager;
        [SerializeField] LifeTimePresenter _lifeTimePresenter;
        [SerializeField] LifeEnergyPresenter _lifeEnergyPresenter;
        [SerializeField] CriticalEventPresenter _criticalEventPresenter;
        [Header("Player")]
        [SerializeField] Player _player;
        [Header("Score")]
        [SerializeField] IngameScorePresenter _scorePresenter;
        [SerializeField] UnfinishedPool _unfinishedPool;
        [Header("その他")]
        [SerializeField] GameObject _menu;
        [SerializeField] FadeManager _fadeManager;
        [SerializeField] IngameCallWindow _callWindow;
        [SerializeField, Min(0)] int _survivalBonus = 1000;
        [SerializeField] bool _useFallbackControls = true;
        [SerializeField] UnityEvent<string> _onPlayerProfile;
        [SerializeField] UnityEvent<int, bool> _onGameFinished;
        [SerializeField] UnityEvent<int, Vector3> _onScoreAwarded;
        [SerializeField] UnityEvent<bool> _onLightMoveChanged;
        GameObject _pauseControls;
        RectTransform _controlsCanvas;
        OptionWindow _optionWindow;
        Font _nameFont;
        List<IDisposable> _disposables = new();
        static readonly HashSet<IngameBehaviour> _ingameObjects = new();
        static bool _isPaused;
        static bool _isFinished;
        internal static bool HasStarted { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetIngameState()
        {
            _ingameObjects.Clear();
            _isPaused = false;
            _isFinished = false;
            HasStarted = false;
        }

        internal static void RegisterObj(IngameBehaviour obj)
        {
            if (!_ingameObjects.Add(obj)) return;

            if (_isFinished)
                obj.Finish();
            else if (!HasStarted || _isPaused)
                obj.Pause();
            else
                obj.Resume();
        }

        internal static void UnregisterObj(IngameBehaviour obj) => _ingameObjects.Remove(obj);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _scorePresenter?.Init();
            if (_menu != null) _optionWindow = _menu.GetComponent<OptionWindow>();
            _lightMoveManager?.SetUnfinishedPool(_unfinishedPool);
            _lifeTimePresenter?.Init();
            _lifeEnergyPresenter?.Init();
            _criticalEventPresenter?.StartSchedule(_lifeTimePresenter?.RemainingTime?.CurrentValue ?? 0f);
            _menu?.gameObject.SetActive(false);
        }

        private void Start()
        {
            if (_menu != null) _menu.SetActive(false);
            if (_useFallbackControls) CreateControls();
            _onPlayerProfile?.Invoke(ScoreManager.CurrentName);
            StartGame().Forget();
        }

        private void OnDestroy()
        {
            if (_nameFont != null) Destroy(_nameFont);
            _isPaused = false;
            _isFinished = false;
            HasStarted = false;
        }

        private void OnEnable()
        {
            if (_optionWindow != null) _optionWindow.Closed += ResumeGame;

            var remainingTime = _lifeTimePresenter?.RemainingTime;
            if (_criticalEventPresenter != null && remainingTime != null)
            {
                if (_lifeEnergyPresenter != null)
                    _disposables.Add(_criticalEventPresenter.IsCritical.Subscribe(_lifeEnergyPresenter.SetCriticalCondition));

                _disposables.Add(remainingTime.Subscribe(_criticalEventPresenter.UpdateRemainingTime));
            }
            if (remainingTime != null)
                _disposables.Add(remainingTime.Where(time => time <= 0f).Subscribe(_ => FinishGame(true).Forget()));

            if (_unfinishedPool != null && _scorePresenter != null)
                _unfinishedPool.OnScoreRequested += AwardScore;
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
            if (_optionWindow != null) _optionWindow.Closed -= ResumeGame;

            if (_unfinishedPool != null && _scorePresenter != null)
                _unfinishedPool.OnScoreRequested -= AwardScore;
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

        async UniTask StartGame()
        {
            AudioManager.instance?.Play("InGameBGM");
            
            if (_fadeManager != null &&
                await _fadeManager.FadeIn(destroyCancellationToken).SuppressCancellationThrow())
                return;

            _unfinishedPool?.SelectStartingType();

            if (_callWindow != null && _callWindow.isActiveAndEnabled)
            {
                _callWindow.SetThirdCall(_unfinishedPool?.SelectedType ?? UnfinishedType.Default);
                _callWindow.OnFourthCall += EnableGameplay;
                await _callWindow.PlayCalls(destroyCancellationToken).SuppressCancellationThrow();
                _callWindow.OnFourthCall -= EnableGameplay;
            }
            else
                EnableGameplay();
        }

        void EnableGameplay()
        {
            if (this == null || HasStarted || _isFinished) return;

            if (_player != null) _unfinishedPool?.FitSplinesToStage(_player.FollowCamera);
            HasStarted = true;
            if (_lifeTimePresenter?.RemainingTime?.CurrentValue <= 0f)
            {
                FinishGame(true).Forget();
                return;
            }

            foreach (var obj in new List<IngameBehaviour>(_ingameObjects))
            {
                if (obj != null && _ingameObjects.Contains(obj))
                    obj.Resume();
            }
        }

        void OpenMenu(bool openMenu)
        {
            if (!HasStarted || _isFinished || openMenu == _isPaused) return;

            _isPaused = openMenu;
            foreach (var obj in new List<IngameBehaviour>(_ingameObjects))
            {
                if (obj == null || !_ingameObjects.Contains(obj)) continue;

                if (openMenu)
                    obj.Pause();
                else
                    obj.Resume();
            }

            _menu?.SetActive(_isPaused);
            if (_isPaused && _optionWindow != null) _optionWindow.OpenWindow().Forget();
            if (_pauseControls != null) _pauseControls.SetActive(_isPaused);
        }

        void LightMove(bool lightMoved)
        {
            _lightMoveManager?.ChangeLightMoving(lightMoved);
            _onLightMoveChanged?.Invoke(lightMoved);
        }

        void AwardScore(int baseScore, Vector3 position)
        {
            int earned = _scorePresenter.AwardScore(baseScore);
            if (earned <= 0) return;

            _onScoreAwarded?.Invoke(earned, position);
            ShowScorePopup(earned, position);
        }

        void ShowScorePopup(int score, Vector3 position)
        {
            var camera = Camera.main;
            if (_controlsCanvas == null || camera == null) return;

            var point = camera.WorldToScreenPoint(position);
            if (point.z <= 0f || !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _controlsCanvas, point, null, out var canvasPoint)) return;

            var popup = SceneControls.CreateRect("Score Popup", _controlsCanvas, _controlsCanvas.pivot,
                canvasPoint + new Vector2(54f, 45f), new Vector2(150f, 40f));
            var label = SceneControls.AddText(popup, $"+{score}", 32);
            label.fontStyle = FontStyle.Bold;
            Destroy(popup.gameObject, 0.8f);
        }

        public void ResumeGame()
        {
            if (_player != null) _player.CloseMenu();
            OpenMenu(false);
        }

        public void BackTitle()
        {
            FinishGame(false, Scene.Title).Forget();
        }

        void FinishGame() => FinishGame(false).Forget();

        async UniTask FinishGame(bool survived, Scene destination = Scene.Result)
        {
            if (!HasStarted || _isFinished) return;

            _isFinished = true;
            _isPaused = false;
            if (_pauseControls != null) _pauseControls.SetActive(false);
            if (_menu != null) _menu.SetActive(false);
            if (destination == Scene.Result)
            {
                if (survived) _scorePresenter?.AddSurvivalBonus(_survivalBonus);
                int score = _scorePresenter?.TotalScore ?? 0;
                ScoreManager.ReceiveScore(score);
                ScoreManager.UpdateRanking();
                _onGameFinished?.Invoke(score, survived);
            }
            foreach (var obj in new List<IngameBehaviour>(_ingameObjects))
            {
                if (obj != null && _ingameObjects.Contains(obj))
                    obj.Finish();
            }

            if (_fadeManager != null &&
                await _fadeManager.FadeOut(destroyCancellationToken).SuppressCancellationThrow())
                return;

            await SceneTransitionManager.TransitionScene(destination);
        }

        void CreateControls()
        {
            SceneControls.EnsureEventSystem(transform);
            var canvas = SceneControls.CreateCanvas("Ingame Controls", transform);
            _controlsCanvas = canvas;
            var name = SceneControls.CreateRect("Player Name", canvas, new Vector2(0f, 1f), new Vector2(200f, -412f),
                new Vector2(260f, 40f));
            var label = SceneControls.AddText(name, ScoreManager.CurrentName ?? "");
            label.supportRichText = false;
            _nameFont = Font.CreateDynamicFontFromOSFont(new[] { "Yu Gothic", "Arial" }, 28);
            if (_nameFont != null) label.font = _nameFont;
            var radar = canvas.gameObject.AddComponent<UnfinishedRadar>();
            radar.Initialize(canvas, _player != null ? _player.transform : null, _unfinishedPool);
            var controls = SceneControls.CreateRect("Pause Actions", canvas, Vector2.one * 0.5f, Vector2.zero, Vector2.zero);
            SceneControls.AddButton(controls, "Resume", Vector2.one * 0.5f, new Vector2(-116f, -315f), ResumeGame);
            SceneControls.AddButton(controls, "Back to Title", Vector2.one * 0.5f, new Vector2(116f, -315f), BackTitle);
            _pauseControls = controls.gameObject;
            _pauseControls.SetActive(false);
        }
    }
}
