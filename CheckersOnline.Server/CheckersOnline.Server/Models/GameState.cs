namespace CheckersOnline.Server.Models
{
    public class GameState
    {
        public Piece?[][] Board { get; set; } = CreateEmptyBoard();
        public PieceColor CurrentTurn { get; set; }

        private static Piece?[][] CreateEmptyBoard()
        {
            var board = new Piece?[8][];
            for (int i = 0; i < 8; i++)
                board[i] = new Piece?[8];
            return board;
        }
    }
}
