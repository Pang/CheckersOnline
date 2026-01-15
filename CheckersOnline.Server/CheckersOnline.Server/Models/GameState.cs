namespace CheckersOnline.Server.Models
{
    public class GameState
    {
        public Piece?[,] Board { get; set; } = new Piece[8, 8];
        public PieceColor CurrentTurn { get; set; }

        public bool IsValidMove(Move move)
        {
            // TODO: enforce checkers rules
            return true;
        }

        public void ApplyMove(Move move)
        {
            Board[move.ToRow, move.ToCol] = Board[move.FromRow, move.FromCol];
            //Board[move.FromRow, move.FromCol] = 0;
        }
    }
}
