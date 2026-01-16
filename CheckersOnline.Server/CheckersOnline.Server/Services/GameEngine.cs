using CheckersOnline.Server.Models;
using System.Collections.Concurrent;

namespace CheckersOnline.Server.Services
{
    public class GameEngine
    {
        private readonly CheckersRules _rules;

        private readonly ConcurrentDictionary<string, PieceColor> _players = new();
        public GameState currentGame = new GameState();
        public List<string> currentGroupPlayers = new List<string>();

        public GameEngine(CheckersRules rules)
        {
            _rules = rules;
        }

        public void StartNewGame(GameState state)
        {
            _players.Clear();
            
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    state.Board[row][col] = null;
                }
            }

            // Place Black pieces (top 3 rows)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (IsDarkSquare(row, col))
                    {
                        state.Board[row][col] = new Piece(PieceColor.Black, PieceType.Man);
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
                        state.Board[row][col] = new Piece(PieceColor.White, PieceType.Man);
                    }
                }
            }

            SetPlayersAndTurn(state);
        }

        public ConcurrentDictionary<string, PieceColor> SetPlayersAndTurn(GameState state)
        {
            _players[currentGroupPlayers[0]] = PieceColor.Black;
            _players[currentGroupPlayers[1]] = PieceColor.White;

            state.CurrentTurn = PieceColor.Black;

            return _players;
        }

        // Check move valid && Apply game logic for move
        public void TryApplyMove(GameState state, Move move)
        {
            TurnInfo turnInfo = _rules.IsValidMove(state, move);
            if (!turnInfo.IsLegalMove) return;

            Piece piece = state.Board[move.FromRow][move.FromCol];

            state.Board[move.ToRow][move.ToCol] = new Piece(piece.Color, piece.Type);
            state.Board[move.FromRow][move.FromCol] = null;

            if (_rules.CheckIsPromotedToKing(state, move) || 
                !turnInfo.TookEnemyPiece || 
                !_rules.PlayerHasAnyCapture(state.Board, piece.Color))
            {
                SwitchTurn(state);
            }
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

        private bool HasPieces(Piece?[][] board, PieceColor color)
        {
            foreach (var row in board)
            {
                foreach (var piece in row)
                {
                    if (piece != null && piece.Color == color)
                        return true;
                }
            }

            return false;
        }
    }
}
