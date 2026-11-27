
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yajima;

namespace Tokita
{
    public class Life_Energy : MonoBehaviour
    {
        // ========================================
        // Player接続
        // ========================================

        private Player _Player;
        private bool _IsLightMove;


        // ========================================
        // 生命エネルギー
        // ========================================

        [Header("生命エネルギー")]

        [Range(0f, 1f)]
        [SerializeField]
        private float _Energy = 1f;

        public float Energy => _Energy;

        [Header("エネルギー消費時間")]
        [Min(0.01f)]
        [SerializeField]
        private float _AirTime = 7.5f;

        [Header("エネルギー回復時間")]
        [Min(0.01f)]
        [SerializeField]
        private float _FullTime = 0.5f;


        // ========================================
        // タスク回復
        // ========================================

        [Header("タスク回復")]

        [Min(1)]
        [SerializeField]
        private int _TaskRequired = 5;

        [Range(0f, 1f)]
        [SerializeField]
        private float _TaskRecovery = 0.25f;

        private int _TaskCount;

        public int TaskCount => _TaskCount;


        // ========================================
        // 容態急変
        // ========================================

        [Header("容態急変")]

        [SerializeField]
        private float _EmergencyTriggerMinTime = 2f;

        [SerializeField]
        private float _EmergencyTriggerMaxTime = 7f;

        [Min(0.01f)]
        [SerializeField]
        private float _EmergencyNormalDuration = 0.5f;

        [Min(0.01f)]
        [SerializeField]
        private float _EmergencyLightMoveDuration = 7.5f;

        [Min(0f)]
        [SerializeField]
        private float _EmergencyConsumeMultiplier = 2f;

        [Min(0f)]
        [SerializeField]
        private float _EmergencyRecoveryMultiplier = 0.5f;

        private bool _IsEmergency;
        private bool _EmergencyTriggered;
        private float _EmergencyTriggerTime;
        private float _EmergencyProgress;

        public bool IsEmergency => _IsEmergency;


        // ========================================
        // 余命タイマー
        // ========================================

        [Header("余命タイマー")]

        [Min(0.01f)]
        [SerializeField]
        private float _LifeTime = 10f;

        [Min(0.01f)]
        [SerializeField]
        private float _LightMoveTimeScale = 15f;

        [SerializeField]
        private Text _LifeTimeText;

        [SerializeField]
        private Color _TimerNormalColor = Color.white;

        [SerializeField]
        private Color _TimerWarningColor = Color.red;

        [Min(0.01f)]
        [SerializeField]
        private float _TimerBlinkInterval = 0.5f;

        [Min(0f)]
        [SerializeField]
        private float _TimerBlinkThreshold = 5f;

        private float _RemainingLifeTime;
        private float _ElapsedLifeTime;

        public float RemainingLifeTime => _RemainingLifeTime;


        // ========================================
        // タンクUI
        // ========================================

        [Header("タンクUI")]

        [SerializeField]
        private Image _TankFill;

        [SerializeField]
        private Image _TankFrame;

        [Min(0.01f)]
        [SerializeField]
        private float _UISmoothSpeed = 3f;

        private float _DisplayEnergy;

        [System.Serializable]
        public class TankLevel
        {
            [Range(0f, 1f)]
            public float MinEnergy;

            public Sprite Image;
        }

        [SerializeField]
        private List<TankLevel> _TankLevels = new List<TankLevel>();

        private int _CurrentLevel = -1;
        private float _PreviousDisplayEnergy;


        // ========================================
        // タンク揺れ
        // ========================================

        [Header("タンク揺れ")]

        [SerializeField]
        private RectTransform _TankTransform;

        [Min(0f)]
        [SerializeField]
        private float _ShakeStrength = 5f;

        [Min(0.01f)]
        [SerializeField]
        private float _ShakeDuration = 0.2f;

        private Vector2 _DefaultPosition;
        private Coroutine _ShakeCoroutine;


        // ========================================
        // 回復エフェクト
        // ========================================

        [Header("回復エフェクト")]

        [SerializeField]
        private ParticleSystem _RecoveryEffect;

        [SerializeField]
        private Color _FlashColor = Color.white;

        [Min(0.01f)]
        [SerializeField]
        private float _FlashDuration = 0.3f;

        private Color _DefaultColor;
        private Coroutine _FlashCoroutine;


        // ========================================
        // イベント
        // ========================================

        [Header("イベント")]

        [SerializeField]
        private UnityEvent _OnEnergyDepleted = new UnityEvent();

        [SerializeField]
        private UnityEvent _OnLifeTimeExpired = new UnityEvent();

        [SerializeField]
        private UnityEvent _OnEmergencyStarted = new UnityEvent();

        [SerializeField]
        private UnityEvent _OnEmergencyEnded = new UnityEvent();


        // ========================================
        // ゲーム状態
        // ========================================

        private bool _IsGameEnded;

        public bool IsGameEnded => _IsGameEnded;


        // ========================================
        // 初期化
        // ========================================

        private void Awake()
        {
            _Energy = Mathf.Clamp01(_Energy);

            _TaskRequired = Mathf.Max(1, _TaskRequired);

            _AirTime = Mathf.Max(0.01f, _AirTime);
            _FullTime = Mathf.Max(0.01f, _FullTime);

            _LifeTime = Mathf.Max(0.01f, _LifeTime);
            _LightMoveTimeScale = Mathf.Max(0.01f, _LightMoveTimeScale);

            _RemainingLifeTime = _LifeTime;
            _DisplayEnergy = _Energy;
            _PreviousDisplayEnergy = _Energy;

            if (_TankTransform == null && _TankFill != null)
            {
                _TankTransform = _TankFill.rectTransform;
            }

            if (_TankTransform != null)
            {
                _DefaultPosition = _TankTransform.anchoredPosition;
            }

            if (_TankFrame != null)
            {
                _DefaultColor = _TankFrame.color;
            }

            if (_Player == null)
            {
                _Player = GetComponent<Player>();
            }

            UpdateTankUI(true);
            UpdateLifeTimeUI();
        }


        private void OnEnable()
        {
            if (_Player == null)
            {
                _Player = GetComponent<Player>();
            }

            if (_Player != null)
            {
                //_Player.OnLightMove += SetLightMove;
            }
            else
            {
                Debug.LogError(
                    "Life_Energy: 同じGameObjectにPlayerが見つかりません。",
                    this
                );
            }
        }


        private void OnDisable()
        {
            if (_Player != null)
            {
                //_Player.OnLightMove -= SetLightMove;
            }

            if (_ShakeCoroutine != null)
            {
                StopCoroutine(_ShakeCoroutine);
                _ShakeCoroutine = null;
            }

            if (_FlashCoroutine != null)
            {
                StopCoroutine(_FlashCoroutine);
                _FlashCoroutine = null;
            }

            if (_TankTransform != null)
            {
                _TankTransform.anchoredPosition = _DefaultPosition;
            }

            if (_TankFrame != null)
            {
                _TankFrame.color = _DefaultColor;
            }
        }


        private void Start()
        {
            float latestTime = Mathf.Max(0f,_LifeTime - 0.01f);

            float minTime = Mathf.Clamp(_EmergencyTriggerMinTime,0f,latestTime);

            float maxTime = Mathf.Clamp(_EmergencyTriggerMaxTime,minTime,latestTime);

            _EmergencyTriggerTime = Random.Range(minTime, maxTime);
        }


        // ========================================
        // メイン更新
        // ========================================

        private void Update()
        {
            if (_IsGameEnded) return;

            // エネルギー枯渇を最初に判定
            if (_Energy <= 0f)
            {
                _Energy = 0f;

                UpdateTankUI(true);

                GameOver();
                return;
            }

            // Playerが無効になった場合の安全対策
            if (_Player != null && !_Player.isActiveAndEnabled)
            {
                _IsLightMove = false;
            }

            float deltaTime = Time.deltaTime;

            UpdateLifeTime(deltaTime);

            if (_IsGameEnded) return;

            UpdateEmergency(deltaTime);

            UpdateEnergy(deltaTime);

            if (_Energy <= 0f)
            {
                _Energy = 0f;

                UpdateTankUI(true);

                GameOver();
                return;
            }

            UpdateTankUI(false);

            UpdateLifeTimeUI();
        }


        // ========================================
        // 光速移動状態
        // ========================================

        private void SetLightMove(bool value)
        {
            if (_IsGameEnded) return;

            _IsLightMove = value;
        }


        // ========================================
        // 生命エネルギー更新
        // ========================================

        private void UpdateEnergy(float deltaTime)
        {
            float speed;

            if (_IsLightMove)
            {
                // 光速移動中は消費
                speed = -1f / _AirTime;

                if (_IsEmergency)
                {
                    speed *= _EmergencyConsumeMultiplier;
                }
            }
            else
            {
                // 通常状態は回復
                speed = 1f / _FullTime;

                if (_IsEmergency)
                {
                    speed *= _EmergencyRecoveryMultiplier;
                }
            }

            _Energy = Mathf.Clamp01(
                _Energy + speed * deltaTime
            );
        }


        // ========================================
        // タスク達成
        // ========================================

        public void CompleteTask()
        {
            if (_IsGameEnded) return;

            _TaskCount++;

            if (_TaskCount >= _TaskRequired)
            {
                _TaskCount = 0;

                RecoverEnergy(_TaskRecovery);
            }
        }


        // ========================================
        // エネルギー回復
        // ========================================

        public void RecoverEnergy(float amount)
        {
            if (_IsGameEnded) return;

            _Energy = Mathf.Clamp01(
                _Energy + Mathf.Max(0f, amount)
            );
        }


        // ========================================
        // 余命タイマー更新
        // ========================================

        private void UpdateLifeTime(float deltaTime)
        {
            float timeSpeed = _IsLightMove
                ? 1f / _LightMoveTimeScale
                : 1f;

            float elapsed = deltaTime * timeSpeed;

            _RemainingLifeTime -= elapsed;
            _ElapsedLifeTime += elapsed;

            if (_RemainingLifeTime <= 0f)
            {
                _RemainingLifeTime = 0f;

                LifeTimeOver();
                return;
            }

            // タイマーから経過時間を受け取り、
            // 決定済みの時刻に到達したら容態急変
            if (!_EmergencyTriggered &&
                _ElapsedLifeTime >= _EmergencyTriggerTime)
            {
                StartEmergency();
            }
        }


        // ========================================
        // タイマー表示
        // ========================================

        private void UpdateLifeTimeUI()
        {
            if (_LifeTimeText == null) return;

            int totalCentiseconds = Mathf.CeilToInt(
                _RemainingLifeTime * 100f
            );

            int minutes = totalCentiseconds / 6000;
            int seconds = (totalCentiseconds / 100) % 60;
            int centiseconds = totalCentiseconds % 100;

            _LifeTimeText.text =
                $"{minutes:00}:{seconds:00}.{centiseconds:00}";

            // 残り時間が少なくなったら赤点滅
            if (_RemainingLifeTime <= _TimerBlinkThreshold)
            {
                float blinkInterval = Mathf.Max(0.01f,_TimerBlinkInterval);

                int blink = Mathf.FloorToInt(Time.unscaledTime / blinkInterval);

                _LifeTimeText.color =
    blink % 2 == 0
    ? _TimerNormalColor
    : _TimerWarningColor;
            }
            else
            {
                _LifeTimeText.color = _TimerNormalColor;
            }
        }


        // ========================================
        // 容態急変開始
        // ========================================

        private void StartEmergency()
        {
            if (_IsEmergency || _IsGameEnded) return;

            _EmergencyTriggered = true;
            _IsEmergency = true;
            _EmergencyProgress = 0f;

            _OnEmergencyStarted?.Invoke();
        }


        // ========================================
        // 容態急変更新
        // ========================================

        private void UpdateEmergency(float deltaTime)
        {
            if (!_IsEmergency) return;

            float duration = _IsLightMove
                ? _EmergencyLightMoveDuration
                : _EmergencyNormalDuration;

            duration = Mathf.Max(0.01f, duration);

            _EmergencyProgress += deltaTime / duration;

            if (_EmergencyProgress >= 1f)
            {
                EndEmergency();
            }
        }


        // ========================================
        // 容態急変終了
        // ========================================

        private void EndEmergency()
        {
            if (!_IsEmergency) return;

            _IsEmergency = false;
            _EmergencyProgress = 0f;

            _OnEmergencyEnded?.Invoke();
        }


        // ========================================
        // タンクUI更新
        // ========================================

        private void UpdateTankUI(bool instant)
        {
            if (instant)
            {
                _DisplayEnergy = _Energy;
            }
            else
            {
                _DisplayEnergy = Mathf.MoveTowards(
                    _DisplayEnergy,
                    _Energy,
                    _UISmoothSpeed * Time.deltaTime
                );
            }

            if (_TankFill != null)
            {
                _TankFill.fillAmount = _DisplayEnergy;
            }

            UpdateTankImage(instant);
        }


        // ========================================
        // タンク画像変更
        // ========================================

        private void UpdateTankImage(bool instant)
        {
            if (_TankFill == null || _TankLevels == null) return;

            int newLevel = -1;
            float highestThreshold = -1f;

            for (int i = 0; i < _TankLevels.Count; i++)
            {
                TankLevel level = _TankLevels[i];

                if (level == null || level.Image == null) continue;

                if (_DisplayEnergy >= level.MinEnergy &&
                    level.MinEnergy >= highestThreshold)
                {
                    highestThreshold = level.MinEnergy;
                    newLevel = i;
                }
            }

            if (newLevel == -1 || newLevel == _CurrentLevel)
            {
                _PreviousDisplayEnergy = _DisplayEnergy;
                return;
            }

            _TankFill.sprite = _TankLevels[newLevel].Image;

            if (!instant && _CurrentLevel != -1)
            {
                if (_DisplayEnergy < _PreviousDisplayEnergy)
                {
                    PlayShake();
                }
                else if (_DisplayEnergy > _PreviousDisplayEnergy)
                {
                    PlayRecoveryEffect();
                }
            }

            _CurrentLevel = newLevel;
            _PreviousDisplayEnergy = _DisplayEnergy;
        }


        // ========================================
        // タンク揺れ
        // ========================================

        private void PlayShake()
        {
            if (_TankTransform == null) return;

            if (_ShakeCoroutine != null)
            {
                StopCoroutine(_ShakeCoroutine);
            }

            _ShakeCoroutine = StartCoroutine(ShakeTank());
        }


        private IEnumerator ShakeTank()
        {
            float elapsed = 0f;

            while (elapsed < _ShakeDuration)
            {
                Vector2 randomOffset =
                    Random.insideUnitCircle * _ShakeStrength;

                _TankTransform.anchoredPosition =
                    _DefaultPosition + randomOffset;

                elapsed += Time.deltaTime;

                yield return null;
            }

            _TankTransform.anchoredPosition = _DefaultPosition;
            _ShakeCoroutine = null;
        }


        // ========================================
        // 回復エフェクト
        // ========================================

        private void PlayRecoveryEffect()
        {
            if (_RecoveryEffect != null)
            {
                _RecoveryEffect.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );

                _RecoveryEffect.Play();
            }

            if (_TankFrame == null) return;

            if (_FlashCoroutine != null)
            {
                StopCoroutine(_FlashCoroutine);
            }

            _FlashCoroutine = StartCoroutine(FlashTank());
        }


        private IEnumerator FlashTank()
        {
            float elapsed = 0f;

            _TankFrame.color = _FlashColor;

            while (elapsed < _FlashDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    elapsed / _FlashDuration
                );

                _TankFrame.color = Color.Lerp(
                    _FlashColor,
                    _DefaultColor,
                    t
                );

                yield return null;
            }

            _TankFrame.color = _DefaultColor;
            _FlashCoroutine = null;
        }


        // ========================================
        // 生命エネルギー枯渇
        // ========================================

        private void GameOver()
        {
            if (_IsGameEnded) return;

            _IsGameEnded = true;

            EndEmergency();

            _OnEnergyDepleted?.Invoke();

            Debug.Log("生命エネルギー枯渇：GameOver");
        }


        // ========================================
        // 余命終了
        // ========================================

        private void LifeTimeOver()
        {
            if (_IsGameEnded) return;

            _IsGameEnded = true;

            EndEmergency();

            UpdateLifeTimeUI();

            _OnLifeTimeExpired?.Invoke();

            Debug.Log("余命終了：GameEnd");
        }
    }
}
