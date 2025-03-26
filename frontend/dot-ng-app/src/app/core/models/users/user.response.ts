import { RoleResponse } from "../roles/role.response";

export interface UserResponse {
    id: string;
    name: string;
    email: string;
    userName: string;
    role: RoleResponse | null;
}