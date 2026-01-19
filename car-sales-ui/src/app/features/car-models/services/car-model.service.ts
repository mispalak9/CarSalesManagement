import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, forkJoin } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { CarModelDto, BrandDto, CarClassDto, ApiResponse, CarModelImageDto } from '../../../core/models/api.models';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CarModelService {
  private carModelsSubject = new BehaviorSubject<CarModelDto[]>([]);
  public carModels$ = this.carModelsSubject.asObservable();

  private brandsSubject = new BehaviorSubject<BrandDto[]>([]);
  public brands$ = this.brandsSubject.asObservable();

  private classesSubject = new BehaviorSubject<CarClassDto[]>([]);
  public classes$ = this.classesSubject.asObservable();

  constructor(private apiService: ApiService) {
    this.initializeData();
  }

  private initializeData(): void {
    forkJoin({
      brands: this.apiService.getBrands(),
      classes: this.apiService.getCarClasses(),
      models: this.apiService.getCarModels()
    }).subscribe({
      next: (result) => {
        if (result.brands.success) {
          this.brandsSubject.next(result.brands.data);
        }
        if (result.classes.success) {
          this.classesSubject.next(result.classes.data);
        }
        if (result.models.success) {
          this.mapAndUpdateModels(result.models.data);
        }
      },
      error: (error) => {
        console.error('Error initializing car model data:', error);
      }
    });
  }

  loadCarModels(): void {
    this.apiService.getCarModels().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.mapAndUpdateModels(response.data);
        }
      },
      error: (error) => {
        console.error('Error loading car models:', error);
      }
    });
  }

  private mapAndUpdateModels(models: CarModelDto[]): void {
    const brands = this.brandsSubject.value;
    const classes = this.classesSubject.value;

    const mapped = models.map((model) => ({
      ...model,
      brandName: model.brandName || brands.find(b => b.brandID === model.brandID)?.brandName || 'Unknown Brand',
      className: model.className || classes.find(c => c.classID === model.classID)?.className || 'Unknown Class',
      images: (model.images || []).map(img => ({
        ...img,
        imageUrl: this.buildImageUrl(img.imagePath)
      }))
    }));

    this.carModelsSubject.next(mapped);
  }

  getCarModelById(id: number): Observable<ApiResponse<CarModelDto>> {
    return this.apiService.getCarModelById(id);
  }

  createCarModel(data: CarModelDto): Observable<ApiResponse<CarModelDto>> {
    return this.apiService.createCarModel(data).pipe(
      tap(() => this.loadCarModels())
    );
  }

  updateCarModel(id: number, data: CarModelDto): Observable<ApiResponse<CarModelDto>> {
    return this.apiService.updateCarModel(id, data).pipe(
      tap(() => this.loadCarModels())
    );
  }

  deleteCarModel(id: number): Observable<ApiResponse<any>> {
    return this.apiService.deleteCarModel(id).pipe(
      tap(() => this.loadCarModels())
    );
  }

  uploadImage(modelId: number, file: File): Observable<ApiResponse<CarModelImageDto>> {
    return this.apiService.uploadCarModelImage(modelId, file);
  }

  setDefaultImage(modelId: number, imageId: number): Observable<ApiResponse<any>> {
    return this.apiService.setDefaultImage(modelId, imageId);
  }

  deleteImage(modelId: number, imageId: number): Observable<ApiResponse<any>> {
    return this.apiService.deleteCarModelImage(modelId, imageId);
  }

  private buildImageUrl(imagePath: string): string {
    if (!imagePath) return '';
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) return imagePath;

    const base = environment.apiUrl.replace(/\/$/, '').replace(/\/api\/?$/, '');
    const cleanPath = imagePath.replace(/^\/+/, '');
    return `${base}/${cleanPath}`;
  }

  getCarModels(): CarModelDto[] {
    return this.carModelsSubject.value;
  }

  getBrands(): BrandDto[] {
    return this.brandsSubject.value;
  }

  getClasses(): CarClassDto[] {
    return this.classesSubject.value;
  }
}
