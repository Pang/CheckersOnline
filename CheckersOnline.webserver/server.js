const { WebSocketServer } = require("ws");

const wss = new WebSocketServer({ port: 8080 });

let players = [];
let gameState = null;

wss.on("connection", (ws) => {
  if (players.length >= 2) {
    ws.close();
    return;
  }

  players.push(ws);
  const playerIndex = players.length - 1;

  ws.send(JSON.stringify({ type: "init", playerIndex }));

  ws.on("message", (msg) => {
    const data = JSON.parse(msg.toString());
    console.log(data);
    if (data.type === "state") {
      gameState = data.state;

      players.forEach((p) => {
        if (p.readyState === p.OPEN) {
          p.send(JSON.stringify({ type: "state", state: gameState }));
        }
      });
    }
  });

  ws.on("close", () => {
    players = players.filter((p) => p !== ws);
    gameState = null;
  });
});

console.log("WebSocket server running on ws://localhost:8080");