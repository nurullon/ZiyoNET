import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { C } from '@angular/cdk/keycodes';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatDividerModule
  ]
})
export class LoginComponent {
  title = 'angular-material-tab-router';
  loginForm: FormGroup;

  constructor(private fb: FormBuilder,  private authService: AuthService) {
    
    this.loginForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  async onSubmit() {
    if (this.loginForm.valid) {
      const { userName, password } = this.loginForm.value;
      var res = await this.authService.login(userName, password);
      console.log("Token: ", res);
    }
  }

  loginWithGoogle() {
    this.authService.loginGoogle();
  }

  loginWithFacebook() {
    this.authService.loginFacebook()
      .then(() => {
        console.log('Login successful!');
        
      })
      .catch(error => console.error('Login failed:', error));
  }
}