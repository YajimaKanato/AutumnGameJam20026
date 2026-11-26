using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kato
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; set; }

        private void Awake()
        {
            #region シングルトン化
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }

        public void PopWindow(GameObject ui)
        {
            ui.gameObject.SetActive(true);
        }

        public void CloseWindow(GameObject ui)
        {
            ui.gameObject.SetActive(false);
        }

        public void UpdateGauge(Image gauge,float currentAmount)
        {
            gauge.fillAmount = currentAmount;
        }

        public void UpdateTimer(TextMeshProUGUI timerText,float currentTime)
        {
            timerText.text = "00:" + currentTime.ToString("n2");
        }

        /// <summary>
        /// スコアのUI反映
        /// </summary>
        /// <param name="scoreText">現在のスコアを反映するテキスト</param>
        /// <param name="currentScore">現在のスコア</param>
        public void UpdateScore(TextMeshProUGUI scoreText, int currentScore)
        {
            scoreText.text = currentScore + "点";
        }
    }
}
