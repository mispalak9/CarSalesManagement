import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { LoginRequest, LoginResponse, UserInfo } from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private apiService: ApiService) {
    this.loadUserFromStorage();
  }

  login(username: string, password: string): Observable<boolean> {
    const request: LoginRequest = { username, password };
    return this.apiService.login(request).pipe(
      map(response => {
        if (response.success && response.data) {
          const userInfo: UserInfo = response.data;
          this.setCurrentUser(userInfo);
          return true;
        }
        return false;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('authToken');
    this.currentUserSubject.next(null);
    this.isLoggedInSubject.next(false);
  }

  setCurrentUser(user: UserInfo): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    localStorage.setItem('authToken', user.token);
    this.currentUserSubject.next(user);
    this.isLoggedInSubject.next(true);
  }

  private loadUserFromStorage(): void {
    const userJson = localStorage.getItem('currentUser');
    if (userJson) {
      try {
        const user: UserInfo = JSON.parse(userJson);
        this.currentUserSubject.next(user);
        this.isLoggedInSubject.next(true);
      } catch (error) {
        console.error('Error parsing stored user', error);
        this.logout();
      }
    }
  }

  getCurrentUser(): UserInfo | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  isLoggedIn(): boolean {
    return this.isLoggedInSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user ? user.roles.includes(role) : false;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.some(role => user.roles.includes(role)) : false;
  }
}
