using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PawnAndRun.UI
{
    /// <summary>
    /// 랭킹화면: currently populated with dummy rows at edit time (see build script);
    /// wires the back button to the main menu and toggles the All/My-records tab visuals
    /// until a real leaderboard exists.
    /// </summary>
    public class RankingScreenController : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button allTabButton;
        [SerializeField] private Button myTabButton;
        [SerializeField] private Sprite tabActiveSprite;
        [SerializeField] private Sprite tabInactiveSprite;
        [SerializeField] private GameObject allRowsContainer;
        [SerializeField] private GameObject myRankPanel;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(GoToMain);
                closeButton.onClick.AddListener(GoToMain);
            }
            if (allTabButton != null)
            {
                allTabButton.onClick.RemoveListener(ShowAll);
                allTabButton.onClick.AddListener(ShowAll);
            }
            if (myTabButton != null)
            {
                myTabButton.onClick.RemoveListener(ShowMine);
                myTabButton.onClick.AddListener(ShowMine);
            }
        }

        private void GoToMain()
        {
            SceneManager.LoadScene("MainScene");
        }

        private void ShowAll()
        {
            SetTab(showAll: true);
        }

        private void ShowMine()
        {
            SetTab(showAll: false);
        }

        private void SetTab(bool showAll)
        {
            if (allTabButton != null) allTabButton.image.sprite = showAll ? tabActiveSprite : tabInactiveSprite;
            if (myTabButton != null) myTabButton.image.sprite = showAll ? tabInactiveSprite : tabActiveSprite;
            if (allRowsContainer != null) allRowsContainer.SetActive(showAll);
            if (myRankPanel != null) myRankPanel.SetActive(true);
        }
    }
}
