export interface GameState {
  board: (Piece | null)[][];
  currentTurn: PieceColor;
}

export interface Piece {
  color: PieceColor;
  type: PieceType;
}

export type PieceColor = "Black" | "White";
export type PieceType = "Man" | "King";
