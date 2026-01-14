import { useEffect, useRef, useState } from "react";

// --- Types ---
type Player = "WHITE" | "BLACK";
type Piece = { player: Player; king: boolean } | null;
type Board = Piece[][]; // 8x8

type WSMessage =
  | { type: "state"; board: Board; turn: Player }
  | { type: "move"; from: [number, number]; to: [number, number] }
  | { type: "join"; player: Player };

// --- Helpers ---
const SIZE = 8;

function createInitialBoard(): Board {
  const b: Board = Array.from({ length: SIZE }, () => Array(SIZE).fill(null));
  for (let r = 0; r < 3; r++) {
    for (let c = 0; c < SIZE; c++) if ((r + c) % 2 === 1) b[r][c] = { player: "BLACK", king: false };
  }
  for (let r = 5; r < 8; r++) {
    for (let c = 0; c < SIZE; c++) if ((r + c) % 2 === 1) b[r][c] = { player: "WHITE", king: false };
  }
  return b;
}

function cloneBoard(b: Board): Board {
  return b.map((row) => row.map((p) => (p ? { ...p } : null)));
}

export default function App() {
  const [board, setBoard] = useState<Board>(() => createInitialBoard());
  const [turn, setTurn] = useState<Player>("WHITE");
  const [me, setMe] = useState<Player | null>(null);
  const [selected, setSelected] = useState<[number, number] | null>(null);
  const wsRef = useRef<WebSocket | null>(null);

  // Connect WebSocket
  useEffect(() => {
    const ws = new WebSocket("ws://localhost:8080");
    wsRef.current = ws;

    ws.onmessage = (ev) => {
      console.log(ev);
      const msg: WSMessage = JSON.parse(ev.data);
      if (msg.type === "join") setMe(msg.player);
      if (msg.type === "state") {
        setBoard(msg.board);
        setTurn(msg.turn);
      }
    };

    ws.onclose = () => console.log("WS closed");
    return () => ws.close();
  }, []);

  function sendMove(from: [number, number], to: [number, number]) {
    wsRef.current?.send(JSON.stringify({ type: "move", from, to }));
  }

  function handleCellClick(r: number, c: number) {
    const piece = board[r][c];
    if (!selected) {
      if (piece && piece.player === me && turn === me) setSelected([r, c]);
      return;
    }
    const [sr, sc] = selected;
    setSelected(null);
    if (sr === r && sc === c) return;
    sendMove([sr, sc], [r, c]);
  }

  return (
  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '16px', minHeight: '100vh', backgroundColor: '#272727ff', padding: '16px' }}>
    <h1 style={{ fontSize: '24px', fontWeight: 'bold' }}>Online Checkers</h1>
    <div style={{ fontSize: '14px' }}>You are: <b>{me ?? "…"}</b> | Turn: <b>{turn}</b></div>
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(8, 64px)', border: '4px solid #444' }}>
    {board.map((row, r) =>
      row.map((cell, c) => {
        const dark = (r + c) % 2 === 1;
        const isSel = selected?.[0] === r && selected?.[1] === c;
        return (
          <div className="gameBoard" key={`${r}-${c}`}
              onClick={() => dark && handleCellClick(r, c)}
              style={{
                backgroundColor: dark ? '#276127ff' : '#89d389ff',
                boxSizing: 'border-box',
                border: isSel ? '4px solid WHITE' : 'none', 
              }}>

          {cell && (
            <div className="playerChecker" style={{backgroundColor: cell.player === 'WHITE' ? '#fff' : '#000',}} >
              {cell.king ? 'K' : ''}
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
