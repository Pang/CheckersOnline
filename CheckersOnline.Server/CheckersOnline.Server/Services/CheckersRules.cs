using CheckersOnline.Server.Models;

namespace CheckersOnline.Server.Services
{
    public class CheckersRules
    {
        public bool IsValidMove(GameState state, Move move)
        {
            // diagonal movement, jumps, kings, forced captures, etc.
            return true;
        }
    }
}
