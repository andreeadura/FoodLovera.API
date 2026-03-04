export interface AdminState {
  cities: any[];        // temporar any, apoi facem modele
  restaurants: any[];
  users: any[];

  isLoading: boolean;
  error: unknown | null;
}

export const initialAdminState: AdminState = {
  cities: [],
  restaurants: [],
  users: [],
  isLoading: false,
  error: null,
};