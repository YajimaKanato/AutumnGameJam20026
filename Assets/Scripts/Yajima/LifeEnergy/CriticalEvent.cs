using UnityEngine;

namespace Yajima
{
    [CreateAssetMenu(fileName = "CriticalEvent", menuName = "TeamGemini/CriticalEvent")]
    public class CriticalEvent : MvpModelFactory<CriticalEventModel>
    {
        [SerializeField, Min(0)] int _eventCount = 5;
        [SerializeField, Min(0f)] float _eventTimeRange = 0.5f;

        public override CriticalEventModel CreateModel()
        {
            return new CriticalEventModel(_eventCount, _eventTimeRange);
        }
    }
}
