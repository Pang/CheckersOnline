export interface GameState {
  board: (Piece | null)[][];
  currentTurn: PieceColor;
}

export interface Piece {
  color: PieceColor;
  type: PieceType;
}

export interface Move {
  fromRow: number;
  fromCol: number;
  toRow: number;
  toCol: number;
}

export type PieceColor = "Black" | "White";
export type PieceType = "Man" | "King";
