import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { User } from '../../models/User';
import { TokenService } from '../../services/token.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.scss'],
  standalone: true,
  imports: [CommonModule],
})
export class TopBarComponent {
  user$: Observable<User | null>;

  constructor(
    private router: Router,
    protected userService: UserService,
    private tokenService: TokenService,
    private alertService: AlertService
  ) {
    this.user$ = this.userService.user$;
  }

  handleButtonClick() {
    this.userService.clearUser();
    this.tokenService.clearToken();
    this.alertService.showAlert('You have logged out', 'success');
    this.router.navigate(['/']);
  }
}
