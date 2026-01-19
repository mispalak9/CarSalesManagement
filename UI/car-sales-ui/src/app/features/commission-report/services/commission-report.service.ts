import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { CommissionReportDto, ApiResponse } from '../../../core/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class CommissionReportService {
  private commissionReportsSubject = new BehaviorSubject<CommissionReportDto[]>([]);
  public commissionReports$ = this.commissionReportsSubject.asObservable();

  constructor(private apiService: ApiService) {}

  loadAllCommissionReports(month: number, year: number): Observable<ApiResponse<CommissionReportDto[]>> {
    return this.apiService.getAllSalesmenCommissionReport(month, year).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.commissionReportsSubject.next(response.data);
        }
      })
    );
  }

  getSalesmanCommissionReport(salesmanId: number, month: number, year: number): Observable<ApiResponse<CommissionReportDto>> {
    return this.apiService.getSalesmanCommissionReport(salesmanId, month, year);
  }

  updateCommissionReports(reports: CommissionReportDto[]): void {
    this.commissionReportsSubject.next(reports || []);
  }

  getCommissionReports(): CommissionReportDto[] {
    return this.commissionReportsSubject.value;
  }
}
