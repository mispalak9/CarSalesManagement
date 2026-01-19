import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CarModelService } from '../services/car-model.service';
import { NotificationService } from '../../../core/services/notification.service';
import { BrandDto, CarClassDto } from '../../../core/models/api.models';
import { AlphanumericDirective } from '../../../shared/directives/validators.directive';
import { environment } from '../../../../environments/environment';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-car-model-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, AlphanumericDirective],
  templateUrl: './car-model-form.component.html',
  styleUrl: './car-model-form.component.css'
})
export class CarModelFormComponent implements OnInit, OnDestroy {
  carModelForm!: FormGroup;
  isLoading = false;
  isSaving = false;
  isEditMode = false;
  brands: BrandDto[] = [];
  classes: CarClassDto[] = [];
  uploadedImages: any[] = [];
  modelId: number | null = null;
  auditInfo: { createdBy?: string | number; createdOn?: string; lastUpdatedBy?: string | number; lastUpdatedOn?: string } = {};
  private destroy$ = new Subject<void>();

  constructor(
    private formBuilder: FormBuilder,
    private carModelService: CarModelService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadBrandsAndClasses();
    this.initializeForm();
    // Delay checkEditMode to ensure form is ready
    setTimeout(() => {
      this.checkEditMode();
    }, 0);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadBrandsAndClasses(): void {
    this.carModelService.brands$
      .pipe(takeUntil(this.destroy$))
      .subscribe(brands => {
        this.brands = brands;
      });

    this.carModelService.classes$
      .pipe(takeUntil(this.destroy$))
      .subscribe(classes => {
        this.classes = classes;
      });
  }

  private initializeForm(): void {
    this.carModelForm = this.formBuilder.group({
      brandID: ['', Validators.required],
      classID: ['', Validators.required],
      modelName: ['', [Validators.required, Validators.minLength(3)]],
      modelCode: ['', [Validators.required, Validators.pattern(/^[A-Z0-9]{10}$/)]],
      description: ['', Validators.required],
      features: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0.01)]],
      dateOfManufacturing: ['', Validators.required],
      isActive: [true],
      sortOrder: [0, Validators.min(0)]
    });
  }

  private checkEditMode(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        console.log('Route params changed:', params);
        if (params['id']) {
          this.isEditMode = true;
          this.modelId = +params['id'];
          this.loadCarModel(this.modelId);
        } else {
          this.isEditMode = false;
          this.modelId = null;
          this.resetForm();
        }
      });
  }

  private resetForm(): void {
    this.carModelForm.reset();
    this.uploadedImages = [];
  }

  private loadCarModel(id: number): void {
    this.isLoading = true;
    this.carModelService.getCarModelById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            const model = response.data;
            this.carModelForm.patchValue({
              brandID: model.brandID,
              classID: model.classID,
              modelName: model.modelName,
              modelCode: model.modelCode,
              description: model.description,
              features: model.features,
              price: model.price,
              dateOfManufacturing: new Date(model.dateOfManufacturing).toISOString().split('T')[0],
              isActive: model.isActive,
              sortOrder: model.sortOrder
            });
            this.auditInfo = {
              createdBy: model.createdBy,
              createdOn: model.createdOn ? new Date(model.createdOn).toLocaleString() : '',
              lastUpdatedBy: model.lastUpdatedBy,
              lastUpdatedOn: model.lastUpdatedOn ? new Date(model.lastUpdatedOn).toLocaleString() : ''
            };
            this.uploadedImages = (model.images || []).map((img: any) => ({
              ...img,
              imageUrl: img.imagePath ? this.buildImageUrl(img.imagePath) : ''
            }));
          } else {
            this.notificationService.error('Failed to load car model data');
          }
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading car model:', error);
          this.notificationService.error('Failed to load car model');
          this.isLoading = false;
        }
      });
  }

  private buildImageUrl(imagePath: string): string {
    if (!imagePath) return '';
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) return imagePath;
    const base = environment.apiUrl.replace(/\/$/, '').replace(/\/api\/?$/, '');
    const cleanPath = imagePath.replace(/^\/+/, '');
    return `${base}/${cleanPath}`;
  }

  onModelCodeInput(event: any): void {
    const input = event.target.value;
    const alphanumericValue = input.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    if (input !== alphanumericValue) {
      this.carModelForm.patchValue(
        { modelCode: alphanumericValue.substring(0, 10) },
        { emitEvent: false }
      );
    }
  }

  onImageSelect(event: any): void {
    const files = event.target.files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        if (file.size > 5 * 1024 * 1024) {
          this.notificationService.warning(`${file.name} exceeds 5MB limit`);
          continue;
        }

        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.uploadedImages.push({
            file,
            preview: e.target.result,
            isNew: true
          });
        };
        reader.readAsDataURL(file);
      }
    }
  }

  removeImage(index: number): void {
    const image = this.uploadedImages[index];
    if (image.imageID && this.modelId) {
      this.carModelService.deleteImage(this.modelId, image.imageID)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.uploadedImages.splice(index, 1);
            this.notificationService.success('Image deleted successfully');
          },
          error: (error) => {
            this.notificationService.error('Failed to delete image');
          }
        });
    } else {
      this.uploadedImages.splice(index, 1);
    }
  }

  setDefaultImage(index: number): void {
    const image = this.uploadedImages[index];
    if (image.imageID && this.modelId) {
      this.carModelService.setDefaultImage(this.modelId, image.imageID)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.uploadedImages.forEach((img, idx) => {
              img.isDefault = idx === index;
            });
            this.notificationService.success('Default image updated');
          },
          error: (error) => {
            this.notificationService.error('Failed to set default image');
          }
        });
    }
  }

  onSubmit(): void {
    if (this.carModelForm.invalid) {
      this.carModelForm.markAllAsTouched();
      this.notificationService.warning('Please fill all required fields correctly');
      return;
    }

    this.isSaving = true;
    const formData = { ...this.carModelForm.value };

    if (this.isEditMode && this.modelId) {
      const updateData = {
        ...formData,
        modelID: this.modelId
      };
      this.carModelService.updateCarModel(this.modelId, updateData)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.uploadNewImages();
            this.notificationService.success('Car model updated successfully');
            this.carModelService.loadCarModels();
            this.router.navigate(['/car-models']);
          },
          error: (error: any) => {
            console.error('Error updating car model:', error);
            this.notificationService.error('Failed to update car model');
            this.isSaving = false;
          }
        });
    } else {
      this.carModelService.createCarModel(formData)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response: any) => {
            if (response.success && response.data) {
              this.modelId = response.data.modelID;
              this.uploadNewImages();
              this.notificationService.success('Car model created successfully');
              this.carModelService.loadCarModels();
              this.router.navigate(['/car-models']);
            }
          },
          error: (error: any) => {
            console.error('Error creating car model:', error);
            this.notificationService.error('Failed to create car model');
            this.isSaving = false;
          }
        });
    }
  }

  private uploadNewImages(): void {
    const newImages = this.uploadedImages.filter(img => img.isNew && img.file);
    if (newImages.length === 0) {
      this.isSaving = false;
      return;
    }

    let uploadedCount = 0;
    newImages.forEach(image => {
      if (this.modelId) {
        this.carModelService.uploadImage(this.modelId, image.file)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              uploadedCount++;
              if (uploadedCount === newImages.length) {
                this.isSaving = false;
              }
            },
            error: (error: any) => {
              this.notificationService.warning('Some images failed to upload');
              this.isSaving = false;
            }
          });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/car-models']);
  }

  get modelCode() {
    return this.carModelForm.get('modelCode');
  }

  get modelName() {
    return this.carModelForm.get('modelName');
  }

  get price() {
    return this.carModelForm.get('price');
  }
}
