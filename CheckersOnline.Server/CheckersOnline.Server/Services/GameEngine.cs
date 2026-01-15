using CheckersOnline.Server.Models;

namespace CheckersOnline.Server.Services
{
    public class GameEngine
    {
        private readonly CheckersRules _rules;

        public GameEngine(CheckersRules rules)
        {
            _rules = rules;
        }

        public void StartNewGame(GameState state)
        {
            // Clear board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    state.Board[row, col] = null;
                }
            }

            // Place Black pieces (top 3 rows)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (IsDarkSquare(row, col))
                    {
                        state.Board[row, col] = new Piece
                        {
                            Color = PieceColor.Black,
                            Type = PieceType.Man
                        };
                    }
                }
            }

            // Place White pieces (bottom 3 rows)
            for (int row = 5; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (IsDarkSquare(row, col))
                    {
                        state.Board[row, col] = new Piece
                        {
                            Color = PieceColor.White,
                            Type = PieceType.Man
                        };
                    }
                }
            }

            // Black moves first
            state.CurrentTurn = PieceColor.Black;
        }

        // Check move valid && Apply game logic for move
        public bool TryApplyMove(GameState state, Move move)
        {
            if (!_rules.IsValidMove(state, move))
                return false;

            return true;
        }

        // Check for win condition - else, Switch turns
        public void CheckWinOrSwitchTurn(GameState state, Move move)
        {
            if (!HasPieces(state.Board, PieceColor.Black))
            {
                // White wins
            }
            else if (!HasPieces(state.Board, PieceColor.White))
            {
                // Black wins
            }
            else
            {
                SwitchTurn(state);
            }
        }

        private void SwitchTurn(GameState state)
        {
            if (state.CurrentTurn == PieceColor.Black)
            {
                state.CurrentTurn = PieceColor.White;
            }
            else
            {
                state.CurrentTurn = PieceColor.Black;
            }
        }

        private bool IsDarkSquare(int row, int col)
        {
            return (row + col) % 2 != 0;
        }

        private bool HasPieces(Piece?[,] board, PieceColor color)
        {
            foreach (var piece in board)
            {
                if (piece != null && piece.Color == color)
                    return true;
            }

            return false;
        }
    }
}
