using System.Collections;
using PawnAndRun.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PawnAndRun.Game
{
    /// <summary>
    /// Bridges GameBoardModel to the 게임화면 UI: board rendering, move buttons, timer, score/best
    /// readouts, capture/blocked feedback, game-over overlay, and the shared settings modal
    /// (pauses the timer while open).
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private const string BestScoreKey = "pawnrun_best_score";
        private const float CaptureDelaySeconds = 0.12f; // mirrors web-prototype CAPTURE_DELAY_MS

        private static readonly Color PopupColor = new Color(0.71f, 0.47f, 0.18f);
        private static readonly Color BlockedZoneColor = new Color(0.86f, 0.22f, 0.18f, 0.55f);
        private static readonly Color CaptureZoneColor = new Color(0.93f, 0.67f, 0.24f, 0.45f);

        [Header("Board")]
        [SerializeField] private Image[] cellPieces; // 35 entries, index = (TopRow-row)*Cols+col
        [SerializeField] private Image[] frontZoneOverlays; // 5 entries, index = col, front-row (just ahead of player) only
        [SerializeField] private RectTransform boardFrameRect;
        [SerializeField] private RectTransform fxLayer;
        [SerializeField] private Sprite playerPawnSprite;
        [SerializeField] private Sprite enemyPawnSprite;

        [Header("Stat bar")]
        [SerializeField] private TMP_Text scoreValueText;
        [SerializeField] private TMP_Text bestValueText;
        [SerializeField] private TMP_Text timerText;

        [Header("Controls")]
        [SerializeField] private Button forwardButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button gearButton;

        [Header("Control art (per-direction normal/disabled state sprites)")]
        [SerializeField] private Sprite forwardNormalSprite;
        [SerializeField] private Sprite forwardDisabledSprite;
        [SerializeField] private Sprite leftNormalSprite;
        [SerializeField] private Sprite leftDisabledSprite;
        [SerializeField] private Sprite rightNormalSprite;
        [SerializeField] private Sprite rightDisabledSprite;

        [Header("Game over")]
        [SerializeField] private GameObject gameOverOverlay;
        [SerializeField] private TMP_Text reasonText;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private Button restartButton;

        [Header("Settings")]
        [SerializeField] private SettingsModalController settingsModal;

        [Header("Audio")]
        [SerializeField] private SfxPlayer sfxPlayer;

        private readonly GameBoardModel _model = new GameBoardModel();
        private Coroutine _timerCoroutine;
        private bool _isPaused;
        private bool _isAnimating;

        private void Awake()
        {
            if (forwardButton != null) forwardButton.onClick.AddListener(() => TryMove(MoveAction.Forward));
            if (leftButton != null) leftButton.onClick.AddListener(() => TryMove(MoveAction.Left));
            if (rightButton != null) rightButton.onClick.AddListener(() => TryMove(MoveAction.Right));
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (exitButton != null) exitButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
            if (gearButton != null) gearButton.onClick.AddListener(() => settingsModal?.Open());
            if (settingsModal != null)
            {
                settingsModal.Opened += PauseGame;
                settingsModal.Closed += ResumeGame;
                settingsModal.SfxVolumeChanged += v => { if (sfxPlayer != null) sfxPlayer.MasterVolume = v; };
            }
        }

        private void Start()
        {
            if (sfxPlayer != null && settingsModal != null) sfxPlayer.MasterVolume = settingsModal.SfxVolume;
            StartNewGame();
        }

        private void OnRestartClicked()
        {
            sfxPlayer?.PlayStart();
            StartNewGame();
        }

        private void StartNewGame()
        {
            if (gameOverOverlay != null) gameOverOverlay.SetActive(false);
            _isPaused = false;
            _isAnimating = false;
            _model.Setup(PlayerPrefs.GetInt(BestScoreKey, 0));
            RenderBoard();
            UpdateZoneHighlight();
            UpdateStatTexts();
            UpdateButtonStates();

            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            _timerCoroutine = StartCoroutine(TimerLoop());
        }

        private void TryMove(MoveAction action)
        {
            if (_isPaused || _model.IsGameOver || _isAnimating) return;

            var l = _model.GetLegalMoves();
            bool illegal = (action == MoveAction.Forward && !l.CanForward)
                         || (action == MoveAction.Left && !l.CanLeft)
                         || (action == MoveAction.Right && !l.CanRight);
            if (illegal)
            {
                sfxPlayer?.Blocked();
                StartCoroutine(ShakeBoard());
                return;
            }

            if (action == MoveAction.Forward)
            {
                sfxPlayer?.Move();
                ResolveMove(action);
            }
            else
            {
                int targetCol = action == MoveAction.Left ? _model.PlayerCol - 1 : _model.PlayerCol + 1;
                sfxPlayer?.Capture();
                PlayCaptureFx(targetCol);
                StartCoroutine(CaptureThenResolve(action));
            }
        }

        private IEnumerator CaptureThenResolve(MoveAction action)
        {
            _isAnimating = true;
            yield return new WaitForSeconds(CaptureDelaySeconds);
            _isAnimating = false;
            ResolveMove(action);
        }

        private void ResolveMove(MoveAction action)
        {
            if (!_model.TryAct(action)) return;

            RenderBoard();
            UpdateZoneHighlight();
            UpdateStatTexts();
            UpdateButtonStates();

            if (_model.IsGameOver) EndGame(timedOut: false);
        }

        private IEnumerator TimerLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (_isPaused) continue;
                if (_model.IsGameOver) yield break;

                bool timeUp = _model.TickTimer();
                UpdateTimerText();
                if (timeUp)
                {
                    EndGame(timedOut: true);
                    yield break;
                }
                if (_model.TimeLeft <= 10) sfxPlayer?.Tick();
            }
        }

        private void EndGame(bool timedOut)
        {
            bool newBest = _model.Score > PlayerPrefs.GetInt(BestScoreKey, 0);
            if (newBest)
            {
                PlayerPrefs.SetInt(BestScoreKey, _model.Score);
                PlayerPrefs.Save();
            }
            UpdateStatTexts();
            UpdateButtonStates();
            if (finalScoreText != null) finalScoreText.text = _model.Score.ToString();
            if (reasonText != null) reasonText.text = timedOut ? "시간 종료!" : "전진 불가!";
            if (gameOverOverlay != null) gameOverOverlay.SetActive(true);

            if (sfxPlayer != null)
            {
                if (newBest) sfxPlayer.NewBest();
                else sfxPlayer.GameOver();
            }
        }

        private void PauseGame() => _isPaused = true;
        private void ResumeGame() => _isPaused = false;

        private void UpdateButtonStates()
        {
            var l = _model.GetLegalMoves();
            bool over = _model.IsGameOver;
            bool canForward = !over && l.CanForward;
            bool canLeft = !over && l.CanLeft;
            bool canRight = !over && l.CanRight;

            if (forwardButton != null)
            {
                forwardButton.interactable = canForward;
                SetButtonSprite(forwardButton, canForward ? forwardNormalSprite : forwardDisabledSprite);
            }
            if (leftButton != null)
            {
                leftButton.interactable = canLeft;
                SetButtonSprite(leftButton, canLeft ? leftNormalSprite : leftDisabledSprite);
            }
            if (rightButton != null)
            {
                rightButton.interactable = canRight;
                SetButtonSprite(rightButton, canRight ? rightNormalSprite : rightDisabledSprite);
            }
        }

        private static void SetButtonSprite(Button button, Sprite sprite)
        {
            if (sprite == null) return;
            if (button.targetGraphic is Image img) img.sprite = sprite;
        }

        private void UpdateStatTexts()
        {
            if (scoreValueText != null) scoreValueText.text = _model.Score.ToString();
            if (bestValueText != null) bestValueText.text = _model.Best.ToString();
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            if (timerText == null) return;
            int m = _model.TimeLeft / 60;
            int s = _model.TimeLeft % 60;
            timerText.text = $"{m:00}:{s:00}";
            timerText.color = _model.TimeLeft <= 10 ? new Color(0.83f, 0.22f, 0.16f) : new Color(0.14f, 0.09f, 0.06f);
        }

        private static int GetCellIndex(int row, int col) => (GameBoardModel.TopRow - row) * GameBoardModel.Cols + col;

        private void RenderBoard()
        {
            if (cellPieces == null) return;
            for (int row = GameBoardModel.PlayerRow; row <= GameBoardModel.TopRow; row++)
            {
                for (int col = 0; col < GameBoardModel.Cols; col++)
                {
                    int idx = GetCellIndex(row, col);
                    if (idx < 0 || idx >= cellPieces.Length || cellPieces[idx] == null) continue;
                    var img = cellPieces[idx];
                    img.rectTransform.localScale = Vector3.one;

                    if (row == GameBoardModel.PlayerRow && col == _model.PlayerCol)
                    {
                        img.enabled = true;
                        img.color = Color.white;
                        if (playerPawnSprite != null) img.sprite = playerPawnSprite;
                    }
                    else if (row != GameBoardModel.PlayerRow && _model.HasEnemyAt(row, col))
                    {
                        img.enabled = true;
                        img.color = Color.white;
                        if (enemyPawnSprite != null) img.sprite = enemyPawnSprite;
                    }
                    else
                    {
                        img.enabled = false;
                    }
                }
            }
        }

        // Highlights the front-row cells (the row directly ahead of the player) so a move's
        // consequence is visible before it's taken, and - since nothing re-renders after
        // game over - doubles as the "why did I die" indicator: the blocked front cell and
        // any diagonal with no escape stay lit in red on the final frame.
        private void UpdateZoneHighlight()
        {
            if (frontZoneOverlays == null) return;
            var l = _model.GetLegalMoves();

            for (int col = 0; col < GameBoardModel.Cols; col++)
            {
                var overlay = frontZoneOverlays[col];
                if (overlay == null) continue;

                if (col == _model.PlayerCol)
                {
                    if (l.Front)
                    {
                        overlay.enabled = true;
                        overlay.color = BlockedZoneColor;
                    }
                    else
                    {
                        overlay.enabled = false;
                    }
                }
                else if (col == _model.PlayerCol - 1 || col == _model.PlayerCol + 1)
                {
                    bool hasEnemy = _model.HasEnemyAt(GameBoardModel.FrontRow, col);
                    if (hasEnemy)
                    {
                        overlay.enabled = true;
                        overlay.color = CaptureZoneColor;
                    }
                    else if (_model.IsGameOver && l.Over)
                    {
                        overlay.enabled = true;
                        overlay.color = BlockedZoneColor;
                    }
                    else
                    {
                        overlay.enabled = false;
                    }
                }
                else
                {
                    overlay.enabled = false;
                }
            }
        }

        // ---------- Juice: capture pop, score popup, illegal-move shake ----------

        private void PlayCaptureFx(int targetCol)
        {
            int idx = GetCellIndex(GameBoardModel.FrontRow, targetCol);
            if (cellPieces == null || idx < 0 || idx >= cellPieces.Length || cellPieces[idx] == null) return;
            var img = cellPieces[idx];
            if (img.enabled) StartCoroutine(PopFade(img));
            SpawnScorePopup(img.rectTransform);
        }

        private IEnumerator PopFade(Image img)
        {
            var rt = img.rectTransform;
            Color baseColor = img.color;
            float t = 0f;
            while (t < CaptureDelaySeconds)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / CaptureDelaySeconds);
                rt.localScale = Vector3.one * (1f + k * 0.5f);
                img.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - k);
                yield return null;
            }
            rt.localScale = Vector3.one;
        }

        private void SpawnScorePopup(RectTransform cellRt)
        {
            if (cellRt == null || fxLayer == null) return;

            var go = new GameObject("ScorePopup", typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(fxLayer, false);
            rt.position = cellRt.position;
            rt.sizeDelta = new Vector2(200, 70);

            var txt = go.AddComponent<TextMeshProUGUI>();
            txt.text = "+10";
            txt.fontSize = 42;
            txt.fontStyle = FontStyles.Bold;
            txt.color = PopupColor;
            txt.alignment = TextAlignmentOptions.Center;
            txt.raycastTarget = false;

            StartCoroutine(FloatAndFade(rt, txt));
        }

        private IEnumerator FloatAndFade(RectTransform rt, TextMeshProUGUI txt)
        {
            const float dur = 0.6f;
            Vector2 start = rt.anchoredPosition;
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float k = t / dur;
                rt.anchoredPosition = start + Vector2.up * (32f * k);
                var c = txt.color;
                c.a = 1f - k;
                txt.color = c;
                yield return null;
            }
            Destroy(rt.gameObject);
        }

        private IEnumerator ShakeBoard()
        {
            if (boardFrameRect == null) yield break;
            Vector2 basePos = boardFrameRect.anchoredPosition;
            float[] offsets = { -10f, 10f, -7f, 7f, 0f };
            foreach (var o in offsets)
            {
                boardFrameRect.anchoredPosition = basePos + new Vector2(o, 0f);
                yield return new WaitForSeconds(0.045f);
            }
            boardFrameRect.anchoredPosition = basePos;
        }
    }
}
