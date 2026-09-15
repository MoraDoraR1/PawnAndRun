using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PawnAndRun.UI
{
    /// <summary>
    /// Wires up the 메인화면 (main menu) buttons: game start, ranking, and the gear/settings button.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button rankButton;
        [SerializeField] private Button gearButton;
        [SerializeField] private SettingsModalController settingsModal;

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
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartClicked);
                startButton.onClick.AddListener(OnStartClicked);
            }
            if (rankButton != null)
            {
                rankButton.onClick.RemoveListener(OnRankClicked);
                rankButton.onClick.AddListener(OnRankClicked);
            }
            if (gearButton != null)
            {
                gearButton.onClick.RemoveListener(OnGearClicked);
                gearButton.onClick.AddListener(OnGearClicked);
            }
        }

        private void OnStartClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void OnRankClicked()
        {
            SceneManager.LoadScene("RankingScene");
        }

        private void OnGearClicked()
        {
            if (settingsModal != null)
            {
                settingsModal.Open();
            }
        }
    }
}
