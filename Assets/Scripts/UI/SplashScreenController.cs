using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PawnAndRun.UI
{
    /// <summary>
    /// 시작화면: touching anywhere moves on to the main menu.
    /// </summary>
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField] private Button tapCatcher;

        private void Awake()
        {
            if (tapCatcher != null)
            {
                tapCatcher.onClick.RemoveListener(GoToMain);
                tapCatcher.onClick.AddListener(GoToMain);
            }
        }

        private void GoToMain()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
