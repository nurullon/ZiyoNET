import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { UserFilterRequest } from "../models/users/user.filter.request";
import { UserResponse } from "../models/users/user.response";
import { API_ROUTES } from "../constants/api-routes";
import { ListResponse } from "../models/list.response";
import { UserRequest } from "../models/users/user.request";
import { RoleResponse } from "../models/roles/role.response";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private apiService: ApiService) {}

  async getRoles(): Promise<RoleResponse[]> {
    const result = await this.apiService.get<RoleResponse[]>(API_ROUTES.USERS.API_ROLES);
    if (result.success && result.data) {
      return result.data;
    }
    return [];
  }

  async getUsers(filter: UserFilterRequest): Promise<ListResponse<UserResponse>> {
    const queryString = this.toQueryString(filter);
    const url = `${API_ROUTES.USERS.API_USERS}?${queryString}`;
    
    var result = await this.apiService.get<ListResponse<UserResponse>>(url);
    
    if (result.success && result.data) {
      return result.data;
    }

    return {
      data: [],
      totalCount: 0,
      pageNumber: 0,
      pageSize: 0,
      isFirst: false,
      isLast: false
    };
  }


  async createUser(userFormData: FormData): Promise<boolean> {
    const result = await this.apiService.post(API_ROUTES.USERS.API_USERS, userFormData);
    return result.success;
  }

  async updateUser(id: string, userFormData: FormData): Promise<boolean> {
    const result = await this.apiService.put(`${API_ROUTES.USERS.API_USERS}/${id}`, userFormData);
    return result.success;
  }

  async deleteUser(id: string): Promise<boolean> {
    const url = `${API_ROUTES.USERS.API_USERS}/${id}`;
    const result = await this.apiService.delete(url);
    return result.success;
  }

  private toQueryString(filter: UserFilterRequest): string {
    const queryParams = new URLSearchParams();
    for (const key in filter) {
      if (filter.hasOwnProperty(key)) {
        const pascalCaseKey = key.charAt(0).toUpperCase() + key.slice(1);
        queryParams.append(pascalCaseKey, (filter as any)[key]);
      }
    }
    return queryParams.toString();
  }
}
