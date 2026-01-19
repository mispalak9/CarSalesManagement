import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { LayoutComponent } from './shared/layout/layout.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'car-models',
        loadComponent: () => import('./features/car-models/car-model-list/car-model-list.component').then(m => m.CarModelListComponent)
      },
      {
        path: 'car-models/create',
        loadComponent: () => import('./features/car-models/car-model-form/car-model-form.component').then(m => m.CarModelFormComponent)
      },
      {
        path: 'car-models/:id/edit',
        loadComponent: () => import('./features/car-models/car-model-form/car-model-form.component').then(m => m.CarModelFormComponent)
      },
      {
        path: 'commission-report',
        loadComponent: () => import('./features/commission-report/commission-report.component').then(m => m.CommissionReportComponent)
      },
      {
        path: 'unauthorized',
        loadComponent: () => import('./features/auth/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
