const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 8080 });

const SIZE = 8;

// --- Types ---
// Piece = { player: 'WHITE' | 'BLACK', king: boolean } | null

// Initial board setup
function createInitialBoard() {
  const board = Array.from({ length: SIZE }, () => Array(SIZE).fill(null));
  for (let r = 0; r < 3; r++) {
    for (let c = 0; c < SIZE; c++) {
      if ((r + c) % 2 === 1) board[r][c] = { player: 'BLACK', king: false };
    }
  }
  for (let r = 5; r < 8; r++) {
    for (let c = 0; c < SIZE; c++) {
      if ((r + c) % 2 === 1) board[r][c] = { player: 'WHITE', king: false };
    }
  }
  return board;
}

let board = createInitialBoard();
let turn = 'WHITE'; // WHITE starts
let connectedPlayer = [];

function broadcastBoard() {
  const message = JSON.stringify({ type: 'state', board, turn });
  wss.clients.forEach(client => {
    if (client.readyState === WebSocket.OPEN) client.send(message);
  });
}

wss.on('connection', ws => {
  if (connectedPlayer.length >= 2) {
    ws.send(JSON.stringify({ type: 'error', message: 'Game full' }));
    ws.close();
    return;
  }

  let player = 'WHITE';
  if (connectedPlayer.length > 0 && connectedPlayer[0].player == "WHITE") {
    player = "BLACK";
  }

  connectedPlayer.push(ws);
  ws.player = player;

  ws.send(JSON.stringify({ type: 'join', player }));
  ws.send(JSON.stringify({ type: 'state', board, turn }));

  ws.on('message', data => {
    let msg;
    try {
      msg = JSON.parse(data);
    } catch (e) {
      console.error('Invalid message', data);
      return;
    }

    if (msg.type === 'move') {
      const [fromR, fromC] = msg.from;
      const [toR, toC] = msg.to;
      const piece = board[fromR][fromC];

      if (!piece || piece.player !== ws.player) return; // Not your piece
      if (ws.player !== turn) return; // Not your turn

      // Very basic move (no capture validation yet)
      board[toR][toC] = piece;
      board[fromR][fromC] = null;

      // Promote to king
      if (piece.player === 'WHITE' && toR === 0) piece.king = true;
      if (piece.player === 'BLACK' && toR === SIZE - 1) piece.king = true;

      turn = turn === 'WHITE' ? 'BLACK' : 'WHITE';

      broadcastBoard();
    }
  });

  ws.on('close', () => {
    connectedPlayer = connectedPlayer.filter(p => p !== ws);
    // reset board when someone leaves
    board = createInitialBoard();
    turn = 'WHITE';
    broadcastBoard();
  });
});

console.log('Checkers WebSocket server running on ws://localhost:8080');
