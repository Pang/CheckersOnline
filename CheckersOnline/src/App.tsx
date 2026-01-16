import { useEffect, useRef, useState } from "react";
import { getConnection, startConnection } from "./services/signalR";
import type { GameState, Move, PieceColor } from "./models/GameState";
import type { HubConnection } from "@microsoft/signalr";

export default function App() {
  const [gameState, setGameState] = useState<GameState>();
  const [me, setMe] = useState<PieceColor | null>(null);
  const [selectedPiece, setSelectedPiece] = useState<[number, number] | null>(null);
  const wsConn = useRef<HubConnection | null>(null);

  const GameStartedHandler = (gameState: GameState) => {
    console.log(gameState.board);
    setGameState(gameState);
  };
  const SetColorHandler = (color: PieceColor) => {
    console.log(color);
    setMe(color);
  }
  const SetMoveMadeHandler = (gameState: GameState) => {
    console.log(gameState.board);
    setGameState(gameState);
  }

  // connect signalr
  useEffect(() => {
      wsConn.current = getConnection();
      const conn = wsConn.current;
      conn.on("GameStarted", GameStartedHandler);
      conn.on("SetColor", SetColorHandler);
      conn.on("MoveMade", SetMoveMadeHandler);

      startConnection().catch(console.error);

      return () => {
        conn.off("GameStarted", GameStartedHandler);
        conn.off("SetColor", SetColorHandler);
        conn.off("MoveMade", SetMoveMadeHandler);
      };
  }, []);

  function sendMove(r: number, c: number, sr: number, sc: number) {
    const move: Move = { fromRow: sr, fromCol: sc, toRow: r, toCol: c };
    wsConn.current?.invoke("MakeMove", move);
  }

  function handleCellClick(r: number, c: number) {
    const piece = gameState?.board[r][c];
    console.log(piece);
    if (!selectedPiece) {
      if (piece && piece.color === me && gameState?.currentTurn === me) setSelectedPiece([r, c]);
      return;
    }
    const [sr, sc] = selectedPiece;
    setSelectedPiece(null);
    if (sr === r && sc === c) return;
    sendMove(r, c, sr, sc);
  }

  return (
  <div className="mainContainer">
    <h1>Online Checkers</h1>
    <div style={{ fontSize: '14px' }}>You are: <b>{me ?? "…"}</b> | Turn: <b>{gameState?.currentTurn}</b></div>
    <div className="gameBoard">
      {gameState?.board.map((row, r) =>
        row.map((cell, c) => {
          const dark = (r + c) % 2 === 1;
          const isSel = selectedPiece?.[0] === r && selectedPiece?.[1] === c;
          return (
            <div className="gameBoardSquare" key={`${r}-${c}`}
                onClick={() => dark && handleCellClick(r, c)}
                style={{
                  backgroundColor: dark ? '#276127ff' : '#89d389ff',
                  boxSizing: 'border-box',
                  border: isSel ? '4px solid WHITE' : 'none', 
                }}>

            {cell && (
              <div className="playerChecker" style={{backgroundColor: cell.color === 'White' ? '#fff' : '#000',}} >
                {cell.type == "King" ? 'K' : ''}
              </div>
            )}
          </div>
          );
        })
      )}
    </div>
    <p style={{ fontSize: '12px', color: '#555', maxWidth: '400px', textAlign: 'center' }}>
      Still a WIP
    </p>
  </div>
  );
}
