import {
  AdminCity,
  AdminRestaurant,
  AdminUser,
} from '../../services/admin-api.service';

export interface AdminState {
  cities: AdminCity[];
  restaurants: AdminRestaurant[];
  users: AdminUser[];
  isLoading: boolean;
  error: string | null;
}

export const initialAdminState: AdminState = {
  cities: [],
  restaurants: [],
  users: [],
  isLoading: false,
  error: null,
};