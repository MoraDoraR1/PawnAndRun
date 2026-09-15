using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PawnAndRun.UI
{
    /// <summary>
    /// 랭킹화면: currently populated with dummy rows at edit time (see build script);
    /// only wires the close button back to the main menu until a real leaderboard exists.
    /// </summary>
    public class RankingScreenController : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(GoToMain);
                closeButton.onClick.AddListener(GoToMain);
            }
        }

        private void GoToMain()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
