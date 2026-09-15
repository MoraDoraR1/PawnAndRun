using System;
using UnityEngine;
using UnityEngine.UI;

namespace PawnAndRun.UI
{
    /// <summary>
    /// Shared 배경음/효과음 settings modal, reused by the main menu and the in-game pause modal.
    /// </summary>
    public class SettingsModalController : MonoBehaviour
    {
        private const string BgmVolumeKey = "pawnrun_bgm_volume";
        private const string SfxVolumeKey = "pawnrun_sfx_volume";

        [SerializeField] private GameObject root;
        [SerializeField] private Button closeButton;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        public float BgmVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 1f;

        /// <summary>Raised right after the modal is shown/hidden - e.g. so a game screen can pause/resume.</summary>
        public event Action Opened;
        public event Action Closed;

        /// <summary>Raised whenever the SFX slider changes, so live sound playback can pick it up immediately.</summary>
        public event Action<float> SfxVolumeChanged;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Wires up listeners against the currently assigned references. Safe to call again
        /// (e.g. right after assigning serialized fields from an editor-time construction script,
        /// since Awake may already have run against null references at that point).
        /// </summary>
        public void Initialize()
        {
            BgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

            if (bgmSlider != null)
            {
                bgmSlider.SetValueWithoutNotify(BgmVolume);
                bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);
                bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(SfxVolume);
                sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
                sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
            }
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
                closeButton.onClick.AddListener(Close);
            }
        }

        public void Open()
        {
            if (root != null)
            {
                root.SetActive(true);
            }
            Opened?.Invoke();
        }

        public void Close()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
            Closed?.Invoke();
        }

        public bool IsOpen => root != null && root.activeSelf;

        private void OnBgmSliderChanged(float value)
        {
            BgmVolume = value;
            PlayerPrefs.SetFloat(BgmVolumeKey, value);
        }

        private void OnSfxSliderChanged(float value)
        {
            SfxVolume = value;
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
            SfxVolumeChanged?.Invoke(value);
        }
    }
}
