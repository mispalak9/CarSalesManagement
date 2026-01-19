import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil, finalize } from 'rxjs';
import { CommissionReportService } from './services/commission-report.service';
import { CommissionReportDto, BrandCommissionDetailDto, ClassCommissionDetailDto } from '../../core/models/api.models';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-commission-report',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './commission-report.component.html',
  styleUrl: './commission-report.component.css'
})
export class CommissionReportComponent implements OnInit, OnDestroy {
  reports: CommissionReportDto[] = [];
  filteredReports: CommissionReportDto[] = [];
  filterForm!: FormGroup;
  isLoading = false;
  singleLoading = false;
  selectedReport: CommissionReportDto | null = null;
  singleReport: CommissionReportDto | null = null;
  readonly maxYear = new Date().getFullYear();
  private destroy$ = new Subject<void>();

  constructor(
    private commissionReportService: CommissionReportService,
    private formBuilder: FormBuilder,
    private notificationService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.commissionReportService.commissionReports$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (reports: CommissionReportDto[]) => {
          this.reports = this.normalizeReports(reports);
          this.applyFilters();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading reports:', error);
          this.notificationService.error('Failed to load commission reports');
          this.isLoading = false;
        }
      });

    this.refreshReports();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    const now = new Date();
    this.filterForm = this.formBuilder.group({
      month: [now.getMonth() + 1, [Validators.required, Validators.min(1), Validators.max(12)]],
      year: [now.getFullYear(), [Validators.required, Validators.min(2000), Validators.max(now.getFullYear())]],
      singleSalesmanId: [null],
      salesmanFilter: [''],
      minCommission: [''],
      maxCommission: ['']
    });

    this.filterForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.applyFilters());
  }

  private normalizeReports(reports: CommissionReportDto[]): CommissionReportDto[] {
    return (reports || []).map(report => this.mapBackendReport(report));
  }

  private applyFilters(): void {
    let filtered = [...this.reports];
    const { salesmanFilter, minCommission, maxCommission } = this.filterForm.value;

    if (salesmanFilter) {
      filtered = filtered.filter(report =>
        report.salesmanName.toLowerCase().includes(salesmanFilter.toLowerCase())
      );
    }

    if (minCommission) {
      filtered = filtered.filter(report => report.totalCommission >= parseFloat(minCommission));
    }

    if (maxCommission) {
      filtered = filtered.filter(report => report.totalCommission <= parseFloat(maxCommission));
    }

    filtered.sort((a, b) => b.totalCommission - a.totalCommission);
    this.filteredReports = filtered;
  }

  selectReport(report: CommissionReportDto): void {
    this.selectedReport = this.selectedReport?.commissionReportID === report.commissionReportID
      ? null
      : report;
  }

  formatCurrency(value: number | null | undefined): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(value ?? 0);
  }

  formatPercentage(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  refreshReports(): void {
    const { month, year } = this.filterForm.value;
    if (!month || !year) {
      this.notificationService.error('Please provide both month and year');
      return;
    }

    this.isLoading = true;
    this.commissionReportService
      .loadAllCommissionReports(month, year)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: () => {
          this.notificationService.success('Commission reports refreshed');
        },
        error: () => {
          this.notificationService.error('Failed to load commission reports');
        }
      });
  }

  loadSalesmanReport(): void {
    const { singleSalesmanId, month, year } = this.filterForm.value;
    if (!singleSalesmanId || !month || !year) {
      this.notificationService.warning('Please enter salesman ID, month, and year');
      return;
    }

    this.singleLoading = true;
    this.singleReport = null;
    this.commissionReportService
      .getSalesmanCommissionReport(singleSalesmanId, month, year)
      .pipe(
        finalize(() => {
          this.singleLoading = false;
          this.cdr.detectChanges();
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.singleReport = this.mapBackendReport(response.data);
            this.notificationService.success('Salesman report loaded');
          } else {
            this.notificationService.error(response.message || 'Failed to load report');
          }
        },
        error: () => {
          this.notificationService.error('Failed to load salesman report');
        }
      });
  }

  exportToCSV(): void {
    const headers = [
      'Salesman Name',
      'Month',
      'Year',
      'Total Sales Amount',
      'Fixed Commission',
      'Class Commission',
      'Bonus Commission',
      'Total Commission'
    ];

    const rows = this.filteredReports.map(report => [
      report.salesmanName,
      report.month,
      report.year,
      this.formatCurrency(report.totalSalesAmount),
      this.formatCurrency(report.totalFixedCommission),
      this.formatCurrency(report.totalClassCommission),
      this.formatCurrency(report.bonusCommission),
      this.formatCurrency(report.totalCommission)
    ]);

    let csv = headers.join(',') + '\n';
    rows.forEach(row => {
      csv += row.map(cell => `"${cell}"`).join(',') + '\n';
    });

    const element = document.createElement('a');
    element.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv));
    element.setAttribute('download', `commission_report_${new Date().getTime()}.csv`);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);

    this.notificationService.success('Report exported successfully');
  }

  private mapBackendReport(report: any): CommissionReportDto {
    const month = report.month ?? report.saleMonth;
    const year = report.year ?? report.saleYear;

    const brandDetails: any[] = report.brandDetails || [];

    // Aggregate totals from brand details
    const totals = brandDetails.reduce(
      (acc, brand) => {
        (brand.classDetails || []).forEach((cls: any) => {
          acc.totalSalesAmount += Number(cls.totalSalesAmount || 0);
          acc.totalFixedCommission += Number(cls.fixedCommission || 0);
          acc.totalPercentageCommission += Number(cls.percentageCommission || 0);
          acc.totalBonusCommission += Number(cls.bonusCommission || 0);
        });
        return acc;
      },
      { totalSalesAmount: 0, totalFixedCommission: 0, totalPercentageCommission: 0, totalBonusCommission: 0 }
    );

    const totalSalesAmount = report.totalSalesAmount ?? totals.totalSalesAmount;
    const totalFixedCommission = report.totalFixedCommission ?? totals.totalFixedCommission;
    const totalClassCommission = report.totalClassCommission ?? report.totalPercentageCommission ?? totals.totalPercentageCommission;
    const bonusCommission = report.bonusCommission ?? report.totalBonusCommission ?? totals.totalBonusCommission;
    const totalCommission = report.totalCommission ?? report.grandTotalCommission ?? (totalFixedCommission + totalClassCommission + bonusCommission);

    // Map brand-wise commissions
    const brandCommissions = brandDetails.map(b => {
      const fixedSum = (b.classDetails || []).reduce((s: number, c: any) => s + Number(c.fixedCommission || 0), 0);
      const cars = (b.classDetails || []).reduce((s: number, c: any) => s + Number(c.totalCarsSold || 0), 0);
      const amount = (b.classDetails || []).reduce((s: number, c: any) => s + Number(c.totalSalesAmount || 0), 0);
      const commission = Number(b.brandTotalCommission || fixedSum);
      return {
        brandID: b.brandID,
        brandName: b.brandName,
        fixedCommission: fixedSum,
        numberOfCarsSold: cars,
        totalAmount: amount,
        commission
      };
    });

    // Map class-wise commissions (aggregate by class across brands)
    const classMap = new Map<number, { classID: number; className: string; totalAmount: number; numberOfCarsSold: number; percentageCommission: number }>();
    brandDetails.forEach(b => {
      (b.classDetails || []).forEach((cls: any) => {
        const current = classMap.get(cls.classID) || { classID: cls.classID, className: cls.className, totalAmount: 0, numberOfCarsSold: 0, percentageCommission: 0 };
        current.totalAmount += Number(cls.totalSalesAmount || 0);
        current.numberOfCarsSold += Number(cls.totalCarsSold || 0);
        current.percentageCommission += Number(cls.percentageCommission || 0);
        classMap.set(cls.classID, current);
      });
    });

    const classCommissions = Array.from(classMap.values()).map(v => {
      const percentage = v.totalAmount > 0 ? (v.percentageCommission / v.totalAmount) * 100 : 0;
      return {
        classID: v.classID,
        className: v.className,
        commissionPercentage: percentage,
        numberOfCarsSold: v.numberOfCarsSold,
        totalAmount: v.totalAmount,
        commission: v.percentageCommission
      };
    });

    return {
      ...report,
      month,
      year,
      totalSalesAmount,
      totalFixedCommission,
      totalClassCommission,
      bonusCommission,
      totalCommission,
      brandCommissions,
      classCommissions
    } as CommissionReportDto;
  }

  getTotalCommission(): number {
    return this.filteredReports.reduce((sum, r) => sum + (r.totalCommission || 0), 0);
  }

  getAverageCommission(): number {
    const total = this.getTotalCommission();
    return this.filteredReports.length > 0 ? total / this.filteredReports.length : 0;
  }

  clearSingleReport(): void {
    this.singleReport = null;
    this.filterForm.patchValue({ singleSalesmanId: null });
  }
}
