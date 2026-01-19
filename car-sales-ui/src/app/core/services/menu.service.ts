import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';
import { MenuDto } from '../models/api.models';

export interface MenuItem extends MenuDto {
  children?: MenuItem[];
}

@Injectable({
  providedIn: 'root'
})
export class MenuService {
  private menuSubject = new BehaviorSubject<MenuItem[]>([]);
  public menu$ = this.menuSubject.asObservable();
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();
  private errorSubject = new BehaviorSubject<string | null>(null);
  public error$ = this.errorSubject.asObservable();

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {
    this.initializeMenu();
  }

  private initializeMenu(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.loadUserMenu(user.userID);
    }

    this.authService.currentUser$.subscribe(user => {
      if (user) {
        this.loadUserMenu(user.userID);
      } else {
        this.menuSubject.next([]);
      }
    });
  }

  private loadUserMenu(userId: number): void {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);
    this.apiService.getUserMenu(userId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const menuItems = this.buildMenuTree(response.data);
          this.menuSubject.next(menuItems);
        } else {
          this.errorSubject.next(response.message || 'Failed to load menu');
        }
      },
      error: (error) => {
        console.error('Error loading menu:', error);
        this.errorSubject.next('Unable to load menu. Please try again.');
        this.loadingSubject.next(false);
      },
      complete: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  private buildMenuTree(menuItems: MenuDto[]): MenuItem[] {
    const menuMap = new Map<number, MenuItem>();
    const rootItems: MenuItem[] = [];

    // Create MenuItem objects
    menuItems.forEach(item => {
      menuMap.set(item.menuID, { ...item, children: [] });
    });

    // Build parent-child relationships
    menuMap.forEach((menuItem, id) => {
      if (menuItem.parentMenuID === null || menuItem.parentMenuID === undefined) {
        rootItems.push(menuItem);
      } else {
        const parent = menuMap.get(menuItem.parentMenuID);
        if (parent) {
          parent.children = parent.children || [];
          parent.children.push(menuItem);
        }
      }
    });

    // Sort by displayOrder
    return this.sortMenuItems(rootItems);
  }

  private sortMenuItems(items: MenuItem[]): MenuItem[] {
    return items
      .sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0))
      .map(item => ({
        ...item,
        children: item.children ? this.sortMenuItems(item.children) : []
      }));
  }

  getMenu(): MenuItem[] {
    return this.menuSubject.value;
  }

  refreshMenu(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.loadUserMenu(user.userID);
    }
  }
}
