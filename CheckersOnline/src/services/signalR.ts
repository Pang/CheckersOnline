import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;
let startPromise: Promise<void> | null = null;

export function getConnection(): signalR.HubConnection {
    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7173/checkersHub", {
                transport: signalR.HttpTransportType.WebSockets,
                skipNegotiation: true
            })
            .withAutomaticReconnect()
            .build();
    }
    return connection;
}

// Always returns the same promise while connecting
export function startConnection(): Promise<void> {
    if (!startPromise) {
        const conn = getConnection();
        startPromise = conn.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => {
                console.error("SignalR failed to connect:", err);
                startPromise = null; // allow retry
                throw err;
            });
    }
    return startPromise;
}

export async function stopConnection(): Promise<void> {
    const conn = getConnection();
    if (conn.state === "Connected") {
        await conn.stop();
        startPromise = null;
        console.log("SignalR disconnected");
    }
}