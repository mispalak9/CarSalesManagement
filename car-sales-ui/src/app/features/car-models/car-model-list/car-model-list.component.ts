import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CarModelService } from '../services/car-model.service';
import { CarModelDto } from '../../../core/models/api.models';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-car-model-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './car-model-list.component.html',
  styleUrl: './car-model-list.component.css'
})
export class CarModelListComponent implements OnInit {
  carModels: CarModelDto[] = [];
  filteredModels: CarModelDto[] = [];
  searchForm!: FormGroup;
  isLoading = false;
  sortBy = 'dateOfManufacturing';
  sortOrder = 'desc';

  constructor(
    private carModelService: CarModelService,
    private formBuilder: FormBuilder,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadCarModels();
  }

  private initializeForm(): void {
    this.searchForm = this.formBuilder.group({
      searchTerm: [''],
      sortBy: [this.sortBy],
      sortOrder: [this.sortOrder]
    });

    this.searchForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }

  private loadCarModels(): void {
    this.isLoading = true;
    this.carModelService.carModels$.subscribe({
      next: (models) => {
        this.carModels = models;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading car models:', error);
        this.notificationService.error('Failed to load car models');
        this.isLoading = false;
      }
    });
  }

  private applyFilters(): void {
    let filtered = [...this.carModels];
    const { searchTerm, sortBy, sortOrder } = this.searchForm.value;

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(
        model =>
          model.modelName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          model.modelCode.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Sorting
    if (sortBy === 'dateOfManufacturing') {
      filtered.sort((a, b) => {
        const dateA = new Date(a.dateOfManufacturing).getTime();
        const dateB = new Date(b.dateOfManufacturing).getTime();
        return sortOrder === 'desc' ? dateB - dateA : dateA - dateB;
      });
    } else if (sortBy === 'sortOrder') {
      filtered.sort((a, b) => {
        const orderA = a.sortOrder || 0;
        const orderB = b.sortOrder || 0;
        return sortOrder === 'desc' ? orderB - orderA : orderA - orderB;
      });
    }

    this.filteredModels = filtered;
  }

  deleteCarModel(id: number, modelName: string): void {
    if (confirm(`Are you sure you want to delete ${modelName}?`)) {
      this.carModelService.deleteCarModel(id).subscribe({
        next: () => {
          this.notificationService.success('Car model deleted successfully');
          this.carModelService.loadCarModels();
        },
        error: (error) => {
          console.error('Error deleting car model:', error);
          this.notificationService.error('Failed to delete car model');
        }
      });
    }
  }

  getDefaultImage(model: CarModelDto): string {
    if (model.images && model.images.length > 0) {
      const defaultImage = model.images.find(img => img.isDefault);
      const selected = defaultImage || model.images[0];
      return (selected as any).imageUrl || (selected as any).imagePath || 'assets/placeholder.png';
    }
    return 'assets/placeholder.png';
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
