import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, Router, NavigationEnd } from '@angular/router';
import { MenuService, MenuItem } from '../../core/services/menu.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationsComponent } from '../components/notifications/notifications.component';
import { Observable } from 'rxjs';
import { UserInfo } from '../../core/models/api.models';
import { Location } from '@angular/common';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, NotificationsComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent implements OnInit {
  menuItems: MenuItem[] = [];
  expandedMenus: Set<number> = new Set();
  currentUser$!: Observable<UserInfo | null>;
  menuLoading$!: Observable<boolean>;
  menuError$!: Observable<string | null>;
  pageTitle: string = 'Dashboard';
  canGoBack: boolean = false;

  constructor(
    private menuService: MenuService,
    private authService: AuthService,
    private router: Router,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.currentUser$ = this.authService.currentUser$;
    this.menuLoading$ = this.menuService.loading$;
    this.menuError$ = this.menuService.error$;
    this.menuService.menu$.subscribe(menu => {
      this.menuItems = menu;
    });
    
    // Track route changes to update page title and back button
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.updatePageTitle(event.urlAfterRedirects);
        this.updateBackButtonVisibility(event.urlAfterRedirects);
      }
    });
  }

  updatePageTitle(url: string): void {
    const path = url.split('/')[1] || 'Dashboard';
    const titleMap: { [key: string]: string } = {
      'dashboard': 'Dashboard',
      'car-models': 'Car Models',
      'commission-report': 'Commission Report',
      'login': 'Login',
      'unauthorized': 'Unauthorized'
    };
    this.pageTitle = titleMap[path] || 'Dashboard';
  }

  updateBackButtonVisibility(url: string): void {
    // Show back button on sub-pages, not on dashboard
    this.canGoBack = url !== '/dashboard' && url !== '/';
  }

  toggleMenu(menuId: number): void {
    if (this.expandedMenus.has(menuId)) {
      this.expandedMenus.delete(menuId);
    } else {
      this.expandedMenus.add(menuId);
    }
  }

  isMenuExpanded(menuId: number): boolean {
    return this.expandedMenus.has(menuId);
  }

  goBack(): void {
    this.location.back();
  }

  hasChildren(menu: MenuItem): boolean {
    return !!(menu.children && menu.children.length > 0);
  }

  refreshMenu(): void {
    this.menuService.refreshMenu();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
