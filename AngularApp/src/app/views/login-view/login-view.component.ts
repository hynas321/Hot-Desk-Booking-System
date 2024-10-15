import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SigninComponent } from '../../components/sign-in/sign-in.component';
import { ApiService } from '../../services/api.service';
import { TokenOutput, UserInfoOutput } from '../../api/models';
import { UserService } from '../../services/user.service';
import { TokenService } from '../../services/token.service';
import { AlertService } from '../../services/alert.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login-view',
  templateUrl: './login-view.component.html',
  styleUrls: ['./login-view.component.scss'],
  standalone: true,
  imports: [SigninComponent],
})
export class LoginViewComponent {
  constructor(
    private apiService: ApiService,
    private router: Router,
    private tokenService: TokenService,
    private userService: UserService,
    private alertService: AlertService
  ) {}

  async handleFormSubmit(credentials: { username: string; password: string }) {
    const { username, password } = credentials;

    this.apiService.logIn(username, password).subscribe({
      next: (tokenOutput: TokenOutput) => {
        if (tokenOutput) {
          this.tokenService.setToken(tokenOutput.token);

          this.apiService
            .getUserInfo()
            .subscribe((userInfo: UserInfoOutput) => {
              this.userService.updateUser({
                username: userInfo.username,
                isAdmin: userInfo.isAdmin,
                bookedDesk: userInfo.bookedDesk,
                bookedDeskLocation: userInfo.bookedDeskLocation,
              });
              this.alertService.showAlert(
                `You have successfully logged in`,
                'success'
              );
              this.router.navigate(['/locations']);
            });
        }
      },
      error: (error: HttpErrorResponse) => {
        switch (error.status) {
          case 401:
            this.alertService.showAlert('Invalid credentials', 'error');
            break;
          case 404:
            this.alertService.showAlert('User not found', 'error');
            break;
          case 400:
            this.alertService.showAlert('Invalid request', 'error');
            break;
          default:
            this.alertService.showAlert(
              'An unexpected error occurred. Please try again later',
              'error'
            );
            break;
        }
      },
    });
  }
}
