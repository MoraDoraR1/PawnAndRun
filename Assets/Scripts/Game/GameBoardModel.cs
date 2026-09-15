using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawnAndRun.Game
{
    public enum MoveAction
    {
        Forward,
        Left,
        Right
    }

    public struct LegalMoves
    {
        public bool Front;
        public bool CanForward;
        public bool CanLeft;
        public bool CanRight;
        public bool Over;
    }

    /// <summary>
    /// Pure game-state port of web-prototype/index.html's board logic (capture / forward / score / timer).
    /// Rows run 1 (player row) .. 7 (top spawn row); columns are 0..4. No UnityEngine.UI dependency,
    /// so it can be driven and unit-tested independently of the view.
    /// </summary>
    public class GameBoardModel
    {
        public const int Cols = 5;
        public const int TopRow = 7;
        public const int PlayerRow = 1;
        public const int FrontRow = 2;
        public const int TimeLimitSeconds = 180;
        public const int GraceTurns = 4;
        public const int ScorePerCapture = 10;

        private readonly System.Random _rng = new System.Random();
        private bool[][] _grid;

        public int PlayerCol { get; private set; }
        public int Score { get; private set; }
        public int Turn { get; private set; }
        public int TimeLeft { get; private set; }
        public int Best { get; private set; }
        public bool IsGameOver { get; private set; }

        public void Setup(int bestScore)
        {
            PlayerCol = 2;
            Score = 0;
            Turn = 0;
            TimeLeft = TimeLimitSeconds;
            Best = bestScore;
            IsGameOver = false;

            _grid = new bool[TopRow + 1][];
            _grid[PlayerRow] = new bool[Cols];
            for (int r = FrontRow; r <= TopRow; r++)
            {
                _grid[r] = GenRowPattern(0);
            }
            EnsureSafeFront();
        }

        public bool HasEnemyAt(int row, int col) => _grid[row][col];

        private static int LevelOf(int score) => Mathf.Min(6, score / 20);

        // Each row gets 1-3 independently random columns (never 0). Higher levels skew
        // the count toward 2-3 for more pressure. Mirrors web-prototype pickCount().
        private int PickCount(int level)
        {
            float w1 = Mathf.Max(0.25f, 0.55f - level * 0.04f);
            float w3 = Mathf.Min(0.30f, 0.10f + level * 0.03f);
            float r = (float)_rng.NextDouble();
            if (r < w1) return 1;
            if (r < 1f - w3) return 2;
            return 3;
        }

        private bool[] GenRowPattern(int level)
        {
            int k = PickCount(level);
            var cols = new[] { 0, 1, 2, 3, 4 };
            for (int i = 0; i < k; i++)
            {
                int j = i + _rng.Next(cols.Length - i);
                (cols[i], cols[j]) = (cols[j], cols[i]);
            }
            var taken = new bool[Cols];
            for (int i = 0; i < k; i++) taken[cols[i]] = true;
            return taken;
        }

        // If the front is blocked with no diagonal escape, grant one instead of silently
        // deleting the blocker - the player is never left with zero legal moves.
        // Mirrors web-prototype ensureSafeFront().
        private void EnsureSafeFront()
        {
            if (!_grid[FrontRow][PlayerCol]) return;
            bool canLeft = PlayerCol > 0 && _grid[FrontRow][PlayerCol - 1];
            bool canRight = PlayerCol < Cols - 1 && _grid[FrontRow][PlayerCol + 1];
            if (canLeft || canRight) return;

            var options = new List<int>();
            if (PlayerCol > 0) options.Add(PlayerCol - 1);
            if (PlayerCol < Cols - 1) options.Add(PlayerCol + 1);
            int pick = options[_rng.Next(options.Count)];

            int filled = 0;
            for (int c = 0; c < Cols; c++) if (_grid[FrontRow][c]) filled++;
            if (filled >= 3)
            {
                var removable = new List<int>();
                for (int c = 0; c < Cols; c++)
                {
                    if (c != PlayerCol && c != pick && _grid[FrontRow][c]) removable.Add(c);
                }
                if (removable.Count > 0)
                {
                    _grid[FrontRow][removable[_rng.Next(removable.Count)]] = false;
                }
            }
            _grid[FrontRow][pick] = true;
        }

        public LegalMoves GetLegalMoves()
        {
            bool front = _grid[FrontRow][PlayerCol];
            bool canForward = !front;
            bool canLeft = PlayerCol > 0 && _grid[FrontRow][PlayerCol - 1];
            bool canRight = PlayerCol < Cols - 1 && _grid[FrontRow][PlayerCol + 1];
            bool over = front && !canLeft && !canRight;
            return new LegalMoves { Front = front, CanForward = canForward, CanLeft = canLeft, CanRight = canRight, Over = over };
        }

        private void ShiftAndSpawn()
        {
            for (int r = PlayerRow; r < TopRow; r++) _grid[r] = _grid[r + 1];
            _grid[TopRow] = GenRowPattern(LevelOf(Score));
        }

        /// <summary>
        /// Applies a move if legal. Returns false without side effects if illegal.
        /// </summary>
        public bool TryAct(MoveAction action)
        {
            if (IsGameOver) return false;
            var l = GetLegalMoves();
            bool illegal = (action == MoveAction.Forward && !l.CanForward)
                         || (action == MoveAction.Left && !l.CanLeft)
                         || (action == MoveAction.Right && !l.CanRight);
            if (illegal) return false;

            if (action == MoveAction.Left)
            {
                Score += ScorePerCapture;
                _grid[FrontRow][PlayerCol - 1] = false;
                PlayerCol -= 1;
            }
            else if (action == MoveAction.Right)
            {
                Score += ScorePerCapture;
                _grid[FrontRow][PlayerCol + 1] = false;
                PlayerCol += 1;
            }

            Turn += 1;
            ShiftAndSpawn();
            if (Turn <= GraceTurns) EnsureSafeFront();

            if (GetLegalMoves().Over)
            {
                IsGameOver = true;
                if (Score > Best) Best = Score;
            }
            return true;
        }

        /// <summary>
        /// Advances the timer by one second. Returns true the instant time runs out (i.e. the
        /// caller should treat this as game-over-by-time). The time limit is a scoring cap, not
        /// a win condition - both endings look the same to the caller.
        /// </summary>
        public bool TickTimer()
        {
            if (IsGameOver) return false;
            TimeLeft -= 1;
            if (TimeLeft <= 0)
            {
                TimeLeft = 0;
                IsGameOver = true;
                if (Score > Best) Best = Score;
                return true;
            }
            return false;
        }
    }
}
