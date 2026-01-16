using CheckersOnline.Server.Models;

namespace CheckersOnline.Server.Services
{
    public class CheckersRules
    {
        public bool IsValidMove(GameState state, Move move)
        {
            Piece piece = state.Board[move.FromRow][move.FromCol];
            if (piece.Color != state.CurrentTurn) return false;

            // Stop piece from going backwards if not king
            if (piece.Type == PieceType.Man)
            {
                if (piece.Color == PieceColor.Black && move.FromRow > move.ToRow) return false;
                if (piece.Color == PieceColor.White && move.FromRow < move.ToRow) return false;
            }

            // stop player moving more than 1 square
            if (!IsSingleDiagonalMove(move))
            {
                // check if theres an enemy piece in middle, else false.
                if (!HasEnemyBetween(state.Board, move, piece.Color)) return false;
            }


            // diagonal movement, jumps, kings, forced captures, etc.
            return true;
        }

        private bool IsSingleDiagonalMove(Move move)
        {
            int rowDiff = Math.Abs(move.ToRow - move.FromRow);
            int colDiff = Math.Abs(move.ToCol - move.FromCol);

            return rowDiff == 1 && colDiff == 1;
        }

        private bool HasEnemyBetween(Piece?[][] board, Move move, PieceColor movingColor)
        {
            int rowDiff = Math.Abs(move.ToRow - move.FromRow);
            int colDiff = Math.Abs(move.ToCol - move.FromCol);

            // Must be a jump
            if (rowDiff != 2 || colDiff != 2)
                return false;

            int midRow = (move.FromRow + move.ToRow) / 2;
            int midCol = (move.FromCol + move.ToCol) / 2;

            var middlePiece = board[midRow][midCol];

            return middlePiece != null && middlePiece.Color != movingColor;
        }
    }
}
