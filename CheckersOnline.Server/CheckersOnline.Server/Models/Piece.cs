namespace CheckersOnline.Server.Models
{
    public class Piece
    {
        public Piece()
        { }
        public Piece(PieceColor color, PieceType type)
        {
            Color = color;
            Type = type;
        }

        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }
    }

    public enum PieceColor
    {
        None,
        Black,
        White
    }

    public enum PieceType
    {
        Man,
        King
    }
}
