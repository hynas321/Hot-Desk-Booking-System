import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userKey = 'user';
  private userSubject = new BehaviorSubject<User | null>(
    this.getUserFromSession()
  );

  user$ = this.userSubject.asObservable();

  private getUserFromSession(): User | null {
    const user = sessionStorage.getItem(this.userKey);
    return user ? JSON.parse(user) : null;
  }

  getUser(): User | null {
    return this.userSubject.getValue();
  }

  updateUser(updatedUser: User): void {
    this.userSubject.next(updatedUser);
    sessionStorage.setItem(this.userKey, JSON.stringify(updatedUser));
  }

  clearUser(): void {
    this.userSubject.next(null);
    sessionStorage.removeItem(this.userKey);
  }
}
