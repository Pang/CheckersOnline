namespace CheckersOnline.Server.Models
{
    public class TurnInfo
    {
        public bool IsLegalMove { get; set; }
        public bool TookEnemyPiece { get; set; }
        public bool BecameKing { get; set; }
    }
}
