import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';


@Injectable({
  providedIn: 'root',
})

export class ApiService {
  private baseUrl = environment.baseUrl;

  constructor(private http: HttpClient) {}

  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.handleRequest(() => firstValueFrom(this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`)));
  }

  async post<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.handleRequest(() => firstValueFrom(this.http.post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, data)));
  }

  async put<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.handleRequest(() => firstValueFrom(this.http.put<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, data)));
  }

  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.handleRequest(() => firstValueFrom(this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`)));
  }

  private async handleRequest<T>(requestFn: () => Promise<ApiResponse<T>>): Promise<ApiResponse<T>> {
    try {
      const response = await requestFn();
      if (!response.success) {
        throw response.error;
      }
      return response;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  private handleError(error: any) {
    if (error instanceof HttpErrorResponse) {
      return error.error?.error || { code: 'unknown_error', message: 'An unexpected error occurred' };
    }
    return { code: 'unknown_error', message: 'An unexpected error occurred' };
  }
}