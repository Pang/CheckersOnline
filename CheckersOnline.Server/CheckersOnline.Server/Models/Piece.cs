namespace CheckersOnline.Server.Models
{
    public class Piece
    {
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
