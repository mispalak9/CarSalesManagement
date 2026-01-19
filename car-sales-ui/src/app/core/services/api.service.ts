import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  LoginRequest,
  LoginResponse,
  BrandDto,
  CarClassDto,
  CarModelDto,
  CreateCarModelDto,
  UpdateCarModelDto,
  CarModelImageDto,
  CommissionReportDto,
  MenuDto,
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Auth Endpoints
  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/Auth/login`,
      request
    );
  }

  getUser(userId: number): Observable<ApiResponse<LoginResponse>> {
    return this.http.get<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/Auth/user/${userId}`
    );
  }

  // Car Model Endpoints
  getCarModels(): Observable<ApiResponse<CarModelDto[]>> {
    return this.http.get<ApiResponse<CarModelDto[]>>(
      `${this.apiUrl}/CarModels`
    );
  }

  getCarModelById(id: number): Observable<ApiResponse<CarModelDto>> {
    return this.http.get<ApiResponse<CarModelDto>>(
      `${this.apiUrl}/CarModels/${id}`
    );
  }

  createCarModel(model: CreateCarModelDto): Observable<ApiResponse<CarModelDto>> {
    return this.http.post<ApiResponse<CarModelDto>>(
      `${this.apiUrl}/CarModels`,
      model
    );
  }

  updateCarModel(id: number, model: UpdateCarModelDto): Observable<ApiResponse<CarModelDto>> {
    return this.http.put<ApiResponse<CarModelDto>>(
      `${this.apiUrl}/CarModels/${id}`,
      model
    );
  }

  deleteCarModel(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(
      `${this.apiUrl}/CarModels/${id}`
    );
  }

  // Car Model Images
  getCarModelImages(modelId: number): Observable<ApiResponse<CarModelImageDto[]>> {
    return this.http.get<ApiResponse<CarModelImageDto[]>>(
      `${this.apiUrl}/CarModels/${modelId}/images`
    );
  }

  uploadCarModelImage(modelId: number, file: File): Observable<ApiResponse<CarModelImageDto>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<CarModelImageDto>>(
      `${this.apiUrl}/CarModels/${modelId}/images`,
      formData
    );
  }

  setDefaultImage(modelId: number, imageId: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(
      `${this.apiUrl}/CarModels/${modelId}/images/${imageId}/set-default`,
      {}
    );
  }

  deleteCarModelImage(modelId: number, imageId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(
      `${this.apiUrl}/CarModels/${modelId}/images/${imageId}`
    );
  }

  // Brands
  getBrands(): Observable<ApiResponse<BrandDto[]>> {
    return this.http.get<ApiResponse<BrandDto[]>>(
      `${this.apiUrl}/CarModels/brands`
    );
  }

  // Car Classes
  getCarClasses(): Observable<ApiResponse<CarClassDto[]>> {
    return this.http.get<ApiResponse<CarClassDto[]>>(
      `${this.apiUrl}/CarModels/classes`
    );
  }

  // Commission Report
  getSalesmanCommissionReport(
    salesmanId: number,
    month: number,
    year: number
  ): Observable<ApiResponse<CommissionReportDto>> {
    return this.http.get<ApiResponse<CommissionReportDto>>(
      `${this.apiUrl}/CommissionReport/salesman/${salesmanId}`,
      {
        params: {
          month: month.toString(),
          year: year.toString(),
        },
      }
    );
  }

  getAllSalesmenCommissionReport(
    month: number,
    year: number
  ): Observable<ApiResponse<CommissionReportDto[]>> {
    return this.http.get<ApiResponse<CommissionReportDto[]>>(
      `${this.apiUrl}/CommissionReport/all-salesmen`,
      {
        params: {
          month: month.toString(),
          year: year.toString(),
        },
      }
    );
  }

  // Menu
  getUserMenu(userId: number): Observable<ApiResponse<MenuDto[]>> {
    return this.http.get<ApiResponse<MenuDto[]>>(
      `${this.apiUrl}/Menu/user/${userId}`
    );
  }

  getRoleMenu(roleId: number): Observable<ApiResponse<MenuDto[]>> {
    return this.http.get<ApiResponse<MenuDto[]>>(
      `${this.apiUrl}/Menu/role/${roleId}`
    );
  }
}
