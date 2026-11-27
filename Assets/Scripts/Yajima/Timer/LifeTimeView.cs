using TMPro;
using UnityEngine;

public class LifeTimeView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timerText;

    public void UpdateTimer(float time)
    {
        _timerText.text = "00:" + time.ToString("00.00");
    }
}
