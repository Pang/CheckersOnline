using CheckersOnline.Server.Models;
using System.IO.Pipelines;

namespace CheckersOnline.Server.Services
{
    public class CheckersRules
    {
        public TurnInfo IsValidMove(GameState state, Move move)
        {
            TurnInfo turnInfo = new TurnInfo();
            turnInfo.IsLegalMove = true;

            Piece piece = state.Board[move.FromRow][move.FromCol];

            if (piece.Color != state.CurrentTurn) turnInfo.IsLegalMove = false;

            // Stop piece from going backwards if not king
            if (piece.Type == PieceType.Man)
            {
                if (piece.Color == PieceColor.Black && move.FromRow > move.ToRow) turnInfo.IsLegalMove = false;
                if (piece.Color == PieceColor.White && move.FromRow < move.ToRow) turnInfo.IsLegalMove = false;
            }

            if (!turnInfo.IsLegalMove) return turnInfo;

            // stop player moving more than 1 square
            if (!IsSingleDiagonalMove(move))
            {
                // check if theres an enemy piece in middle, else false.
                if (!HasEnemyBetween(state.Board, move, piece.Color))
                {
                    turnInfo.IsLegalMove = false;
                }
                else
                {
                    turnInfo.TookEnemyPiece = true;
                }
            }
            else
            {
                // If available enemy piece hasn't been captured, disallow move.
                if (PlayerHasAnyCapture(state.Board, piece.Color))
                {
                    turnInfo.IsLegalMove = false;
                }
            }

            return turnInfo;
        }

        public bool CheckIsPromotedToKing(GameState state, Move move)
        {
            Piece piece = state.Board[move.ToRow][move.ToCol];
            if (ReachedPromotionRow(piece, move.ToRow))
            {
                piece.Type = PieceType.King;
                return true;
            }
            return false;
        }

        public bool PlayerHasAnyCapture(Piece?[][] board, PieceColor color)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (board[r][c]?.Color == color &&
                        PieceHasCapture(board, r, c))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool PieceHasCapture(Piece?[][] board, int row, int col)
        {
            var piece = board[row][col];
            if (piece == null) return false;

            int[] directions = piece.Type == PieceType.King
                ? new[] { -1, 1 }
                : piece.Color == PieceColor.Black ? new[] { 1 } : new[] { -1 };

            foreach (int dr in directions)
            {
                foreach (int dc in new[] { -1, 1 })
                {
                    int midRow = row + dr;
                    int midCol = col + dc;
                    int landingRow = row + dr * 2;
                    int landingCol = col + dc * 2;

                    if (!IsInsideBoard(landingRow, landingCol))
                        continue;

                    var middle = board[midRow][midCol];
                    var landing = board[landingRow][landingCol];

                    if (middle != null &&
                        middle.Color != piece.Color &&
                        landing == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ReachedPromotionRow(Piece piece, int row)
        {
            return piece.Color switch
            {
                PieceColor.Black => row == 7,
                PieceColor.White => row == 0,
                _ => false
            };
        }

        private bool IsInsideBoard(int r, int c)
        {
            return r >= 0 && r < 8 && c >= 0 && c < 8;
        }

        private bool IsSingleDiagonalMove(Move move)
        {
            int rowDiff = Math.Abs(move.ToRow - move.FromRow);
            int colDiff = Math.Abs(move.ToCol - move.FromCol);

            return rowDiff == 1 && colDiff == 1;
        }

        public bool HasEnemyBetween(Piece?[][] board, Move move, PieceColor movingColor)
        {
            int rowDiff = Math.Abs(move.ToRow - move.FromRow);
            int colDiff = Math.Abs(move.ToCol - move.FromCol);

            if (rowDiff != 2 || colDiff != 2)
                return false;

            int midRow = (move.FromRow + move.ToRow) / 2;
            int midCol = (move.FromCol + move.ToCol) / 2;

            var middlePiece = board[midRow][midCol];

            if (middlePiece != null && middlePiece.Color != movingColor)
            {
                board[midRow][midCol] = null;
                return true;
            }

            return false;
        }
    }
}
