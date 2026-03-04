export interface SessionsState {
  sessionId: string | null;
  joinCode: string | null;
  sessionName: string | null;

  isLoading: boolean;
  error: unknown | null;
}

export const initialSessionsState: SessionsState = {
  sessionId: null,
  joinCode: null,
  sessionName: null,
  isLoading: false,
  error: null,
};