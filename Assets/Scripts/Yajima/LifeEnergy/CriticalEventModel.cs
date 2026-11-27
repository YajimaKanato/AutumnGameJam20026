using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Yajima
{
    public class CriticalEventModel
    {
        readonly ReactiveProperty<bool> _isCritical = new(false);
        readonly Queue<float> _eventTimings = new();
        readonly int _eventCount;
        readonly float _duration;
        float _timeLimit;
        float _activeUntil;

        public ReadOnlyReactiveProperty<bool> IsCritical => _isCritical;

        public CriticalEventModel(int eventCount, float duration)
        {
            _eventCount = Mathf.Max(0, eventCount);
            _duration = Mathf.Max(0f, duration);
        }

        public void StartSchedule(float timeLimit)
        {
            _eventTimings.Clear();
            _timeLimit = Mathf.Max(0f, timeLimit);
            _activeUntil = 0f;
            _isCritical.Value = false;
            if (_eventCount == 0 || _duration <= 0f || _timeLimit <= 0f) return;

            float interval = _timeLimit / _eventCount;
            for (int i = 0; i < _eventCount; i++)
                _eventTimings.Enqueue(Random.Range(interval * i, interval * (i + 1)));

            UpdateRemainingTime(_timeLimit);
        }

        public void UpdateRemainingTime(float remainingTime)
        {
            float elapsed = _timeLimit - Mathf.Clamp(remainingTime, 0f, _timeLimit);
            while (_eventTimings.Count > 0 && _eventTimings.Peek() <= elapsed)
                _activeUntil = Mathf.Max(_activeUntil, _eventTimings.Dequeue() + _duration);

            _isCritical.Value = remainingTime > 0f && elapsed < _activeUntil;
        }
    }
}
