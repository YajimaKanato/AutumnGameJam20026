using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kato
{
    [System.Serializable]
    public class ClipData
    {
        public AudioClip clip;
        public ClipType type;
    }

    public enum ClipType
    {
        BGM = 0,
        LoopSE = 1,
        OneShotSE = 2,
    }

    public class AudioManager : MonoBehaviour
    {
        [SerializeField, Header("AudioClip設定")] private Dictionary<string, ClipData> _dict = new Dictionary<string, ClipData>();
        [SerializeField] private AudioSource[] _audioSources;
        [Header("======音量調節用スライダー======")]
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _seSlider;


        public static AudioManager instance { get; set; }

        private void Awake()
        {
            #region シングルトン化
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }

        /// <summary>
        /// IDに基づきClipを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClipData GetClip(string id)
        {
            return _dict.TryGetValue(id, out ClipData data) ? data : null;
        }

        /// <summary>
        /// Audio再生
        /// </summary>
        /// <param name="id"></param>
        public void Play(string id)
        {
            var data = string.IsNullOrEmpty(id) ? null : GetClip(id);
            if (data?.clip == null) return;

            AudioSource source = _audioSources[(int)data.type];
            source.clip = data.clip;

            switch (data.type)
            {
                case ClipType.BGM:
                    source.Play();
                    break;
                case ClipType.LoopSE:
                    source.loop = true;
                    source.Play();
                    break;
                case ClipType.OneShotSE:
                    source.PlayOneShot(data.clip);
                    break;
            }
        }

        public float BgmVolume => _audioSources[(int)ClipType.BGM].volume;
        public float SoundEffectVolume => _audioSources[(int)ClipType.OneShotSE].volume;

        public void SetBgmVolume(float volume)
        {
            _audioSources[(int)ClipType.BGM].volume = volume;
        }

        public void SetSoundEffectVolume(float volume)
        {
            _audioSources[(int)ClipType.LoopSE].volume = volume;
            _audioSources[(int)ClipType.OneShotSE].volume = volume;
        }

        /// <summary>
        /// 音量調節
        /// </summary>
        public void SetVolume()
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                AudioSource source = _audioSources[i];

                switch (i)
                {
                    case (int)ClipType.BGM:
                        source.volume = _bgmSlider.value;
                        break;
                    case (int)ClipType.LoopSE:
                        source.volume = _seSlider.value;
                        break;
                    case (int)ClipType.OneShotSE:
                        source.volume = _seSlider.value;
                        break;
                }
            }
        }
    }
}