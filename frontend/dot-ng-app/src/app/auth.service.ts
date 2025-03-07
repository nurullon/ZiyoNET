import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from '../environments/constants';

declare const google: any;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  googleSignup() {
    // google.accounts.id.initialize({
    //   client_id: '182502122629-cpoc28q61ks90j8f4ppsq9jum0vb1ona.apps.googleusercontent.com',
    //   callback: (response: any) => this.handleCredentialResponse(response),
    // });

    var test = google.getAuthResponse().id_token
    console.log(test)
    this.http.post(`${API_BASE_URL}/api/auth/google-signup`, { idToken: test })

    // google.accounts.id.initialize({
    //   client_id: '182502122629-cpoc28q61ks90j8f4ppsq9jum0vb1ona.apps.googleusercontent.com',
    //   callback: (response: any) => this.handleCredentialResponse(response),
    // });

    // google.accounts.id.prompt();
  }

  // private handleCredentialResponse(response: any) {
  //   const idToken = response.credential;
  //   console.log('Google ID Token:', idToken); // Проверяем токен

  //   this.http.post(`${API_BASE_URL}/api/auth/google-signup`, { idToken })
  //     .subscribe((res: any) => {
  //       console.log('Server Response:', res);
  //       localStorage.setItem('token', res.token);
  //     }, error => {
  //       console.error('Sign Up Error:', error);
  //     });
  // }

//   googleLogin() {
//     google.accounts.id.initialize({
//       client_id: '182502122629-clsfqchg8j9jdp7smucmcgeckcmj6mal.apps.googleusercontent.com',
//       callback: (response: any) => this.handleCredentialResponse(response),
//     });

//     google.accounts.id.prompt();
//   }

//   private handleCredentialResponse(response: any) {
//     console.log('Google ID Token:', response.credential);  // Проверь токен

//     this.http.post(`${API_BASE_URL}/api/auth/google-login`, { idToken: response.credential })
//       .subscribe((res: any) => {
//         console.log('Server Response:', res);
//         localStorage.setItem('token', res.token);
//       }, error => {
//         console.error('Login Error:', error);
//       });
// }

}