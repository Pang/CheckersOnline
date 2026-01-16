import { useEffect, useRef, useState } from "react";
import { getConnection, startConnection } from "./services/signalR";
import type { GameState, PieceColor } from "./models/GameState";

export default function App() {
  const [gameState, setGameState] = useState<GameState>();
  // const [board, setBoard] = useState<Board>(() => createInitialBoard());
  // const [turn, setTurn] = useState<PieceColor>("WHITE");
  const [me, setMe] = useState<PieceColor | null>(null);
  const [selected, setSelected] = useState<[number, number] | null>(null);
  const wsRef = useRef<WebSocket | null>(null);

  // connect signalr
  useEffect(() => {
      const conn = getConnection();

      const GameStartedHandler = (gameState: GameState) => {
        console.log(gameState.board);
        setGameState(gameState);
      };

      conn.on("GameStarted", GameStartedHandler);

      conn.on("GameUpdated", (game) => {
        console.log("Game updated:", game);
      });

      // Start connection safely
      startConnection().catch(console.error);

      return () => {
        conn.off("GameStarted", GameStartedHandler);
      };
  }, []);

  // Connect WebSocket
  // useEffect(() => {
  //   const ws = new WebSocket("ws://localhost:8080");
  //   wsRef.current = ws;

  //   ws.onmessage = (ev) => {
  //     console.log(ev);
  //     const msg: WSMessage = JSON.parse(ev.data);
  //     if (msg.type === "join") setMe(msg.player);
  //     if (msg.type === "state") {
  //       setBoard(msg.board);
  //       setTurn(msg.turn);
  //     }
  //   };

  //   ws.onclose = () => console.log("WS closed");
  //   return () => ws.close();
  // }, []);

  function sendMove(from: [number, number], to: [number, number]) {
    wsRef.current?.send(JSON.stringify({ type: "move", from, to }));
  }

  function handleCellClick(r: number, c: number) {
    const piece = gameState?.board[r][c];
    console.log(piece);
    if (!selected) {
      if (piece && piece.color === me && gameState?.currentTurn === me) setSelected([r, c]);
      return;
    }
    const [sr, sc] = selected;
    setSelected(null);
    if (sr === r && sc === c) return;
    sendMove([sr, sc], [r, c]);
  }

  return (
  <div className="mainContainer">
    <h1>Online Checkers</h1>
    <div style={{ fontSize: '14px' }}>You are: <b>{me ?? "…"}</b> | Turn: <b>{gameState?.currentTurn}</b></div>
    <div className="gameBoard">
      {gameState?.board.map((row, r) =>
        row.map((cell, c) => {
          const dark = (r + c) % 2 === 1;
          const isSel = selected?.[0] === r && selected?.[1] === c;
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
