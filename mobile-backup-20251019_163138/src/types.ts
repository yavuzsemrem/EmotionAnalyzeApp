export interface User {
  id: number;
  nickname: string;
  createdAt: string;
}

export interface Message {
  id: number;
  content: string;
  createdAt: string;
  positiveScore: number;
  negativeScore: number;
  neutralScore: number;
  userId: number;
  user?: User;
}

export interface EmotionScores {
  Pozitif: number;
  Negatif: number;
  Nötr: number;
}

