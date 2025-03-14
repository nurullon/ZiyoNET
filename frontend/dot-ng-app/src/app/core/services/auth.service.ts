import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { LoginResponse } from '../models/auth/login.response';
import { environment } from '../../../environments/environment';
import { API_ROUTES } from '../constants/api-routes';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  baseUrl = environment.baseUrl;

  constructor(private api: ApiService, private router: Router) {}

  async login(userName: string, password: string): Promise<boolean> {
    const data = { userName, password };
    const result = await this.api.post<LoginResponse>(API_ROUTES.AUTH.LOGIN, data);
    
    if (result.success && result.data) {
      localStorage.setItem('authToken', result.data.token);

      this.router.navigate(['/dashboard']);
      return true;
    }
    return false;
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('authToken');
  }

  loginGoogle(): Promise<void> {
    return new Promise((_resolve, reject) => {
      const width = 500;
      const height = 600;
      const left = (window.screen.width - width) / 2;
      const top = (window.screen.height - height) / 2;
  
      const popup = window.open(
        `${this.baseUrl}/api/auth/google-login`,
        'GoogleLogin',
        `width=${width},height=${height},top=${top},left=${left}`
      );
  
      if (!popup) {
        reject('Popup blocked');
        return;
      }
      
      const listener = (event: MessageEvent) => {
        console.log('Received event:', event);
        
        if (event.origin !== this.baseUrl) return;
        
        if (event.data?.success && event.data?.token) {
          console.log('Token received:', event.data.token);
          
          localStorage.setItem('authToken', event.data.token);

          this.router.navigate(['/dashboard']);
          
        } else {
          console.error('No token received:', event.data);
          reject('No token received');
        }
      };
      window.addEventListener('message', listener);
  
    });
  }

  loginFacebook(): Promise<void> {
    return new Promise((_resolve, reject) => {
      const width = 500;
      const height = 600;
      const left = (window.screen.width - width) / 2;
      const top = (window.screen.height - height) / 2;
  
      const popup = window.open(
        `${this.baseUrl}/api/auth/facebook-login`,
        'FacebookLogin',
        `width=${width},height=${height},top=${top},left=${left}`
      );
  
      if (!popup) {
        reject('Popup blocked');
        return;
      }
      
      const listener = (event: MessageEvent) => {
        console.log('Received event:', event);
        
        if (event.origin !== this.baseUrl) return;
        
        if (event.data?.success && event.data?.token) {
          console.log('Token received:', event.data.token);
          
          localStorage.setItem('authToken', event.data.token);
          this.router.navigate(['/dashboard']);
          
        } else {
          console.error('No token received:', event.data);
          reject('No token received');
        }
      };
      window.addEventListener('message', listener);
  
    });
  }
}