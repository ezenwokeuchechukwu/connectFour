using System;

namespace ConnectFour
{
    public class GameState
    {
        public enum Piece { Empty, Player1, Player2 }
        public enum WinState { None, Player1_Wins, Player2_Wins, Tie }

        private readonly Piece[] board = new Piece[42]; // 6 rows × 7 columns

        public int PlayerTurn { get; private set; } = 1; // 1 = Player1, 2 = Player2
        public int CurrentTurn { get; private set; } = 0;

        public GameState()
        {
            ResetBoard();
        }

        public void ResetBoard()
        {
            Array.Fill(board, Piece.Empty);
            PlayerTurn = 1;
            CurrentTurn = 0;
        }

        /// <summary>
        /// Plays a piece in the specified column.
        /// Returns the row where the piece lands.
        /// </summary>
        public int PlayPiece(int column)
        {
            if (column < 0 || column > 6)
                throw new ArgumentException("Invalid column");

            for (int row = 5; row >= 0; row--)
            {
                int index = row * 7 + column;

                if (board[index] == Piece.Empty)
                {
                    board[index] = PlayerTurn == 1
                        ? Piece.Player1
                        : Piece.Player2;

                    CurrentTurn++;
                    SwitchTurn();
                    return row;
                }
            }

            throw new ArgumentException("Column is full");
        }

        private void SwitchTurn()
        {
            PlayerTurn = PlayerTurn == 1 ? 2 : 1;
        }

        /// <summary>
        /// Checks the board for a win or tie.
        /// </summary>
        public WinState CheckForWin()
        {
            int[] directions = { 1, 7, 8, 6 }; // right, down, diag-right, diag-left

            for (int i = 0; i < 42; i++)
            {
                if (board[i] == Piece.Empty)
                    continue;

                foreach (int dir in directions)
                {
                    if (HasFourInARow(i, dir))
                    {
                        return board[i] == Piece.Player1
                            ? WinState.Player1_Wins
                            : WinState.Player2_Wins;
                    }
                }
            }

            return CurrentTurn >= 42 ? WinState.Tie : WinState.None;
        }

        private bool HasFourInARow(int start, int direction)
        {
            Piece piece = board[start];
            int startCol = start % 7;

            for (int i = 1; i < 4; i++)
            {
                int idx = start + direction * i;

                if (idx < 0 || idx >= 42)
                    return false;

                int idxCol = idx % 7;

                // Prevent wrapping across rows
                if ((direction == 1 || direction == 6 || direction == 8) &&
                    Math.Abs(idxCol - startCol) > 3)
                    return false;

                if (board[idx] != piece)
                    return false;
            }

            return true;
        }

        // Optional helper (not required by Board.razor)
        public Piece GetPieceAt(int index) => board[index];
    }
}
