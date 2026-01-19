// API Response Models
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}

// Auth Models
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  userID: number;
  username: string;
  fullName: string;
  email: string;
  roles: string[];
  token: string;
}

export interface UserInfo {
  userID: number;
  username: string;
  fullName: string;
  email: string;
  roles: string[];
  token: string;
}

// Brand Models
export interface BrandDto {
  brandID: number;
  brandName: string;
  brandCode: string;
  isActive?: boolean;
  createdOn?: Date;
  lastUpdatedOn?: Date;
}

// Car Class Models
export interface CarClassDto {
  classID: number;
  className: string;
  classCode: string;
  displayOrder?: number;
  isActive?: boolean;
  createdOn?: Date;
  lastUpdatedOn?: Date;
}

// Car Model Image
export interface CarModelImageDto {
  imageID: number;
  modelID: number;
  imagePath: string;
  imageName: string;
  imageSize: number;
  isDefault: boolean;
  sortOrder: number;
  createdOn?: Date;
  lastUpdatedOn?: Date;
}

// Car Model DTO
export interface CarModelDto {
  modelID: number;
  brandID: number;
  classID: number;
  brandName?: string;
  className?: string;
  modelName: string;
  modelCode: string;
  description: string;
  features: string;
  price: number;
  dateOfManufacturing: Date;
  isActive: boolean;
  sortOrder: number;
  createdBy?: number;
  createdOn?: Date;
  lastUpdatedBy?: number;
  lastUpdatedOn?: Date;
  images: CarModelImageDto[];
}

export interface CreateCarModelDto {
  brandID: number;
  classID: number;
  modelName: string;
  modelCode: string;
  description: string;
  features: string;
  price: number;
  dateOfManufacturing: Date;
  isActive: boolean;
  sortOrder: number;
}

export interface UpdateCarModelDto {
  modelID: number;
  brandID: number;
  classID: number;
  modelName: string;
  modelCode: string;
  description: string;
  features: string;
  price: number;
  dateOfManufacturing: Date;
  isActive: boolean;
  sortOrder: number;
}

// Commission Models
export interface BrandCommissionDetailDto {
  brandID: number;
  brandName: string;
  fixedCommission: number;
  numberOfCarsSold: number;
  totalAmount: number;
  commission: number;
}

export interface ClassCommissionDetailDto {
  classID: number;
  className: string;
  commissionPercentage: number;
  numberOfCarsSold: number;
  totalAmount: number;
  commission: number;
}

export interface CommissionReportDto {
  commissionReportID: number;
  salesmanID: number;
  salesmanName: string;
  saleMonth?: number;
  saleYear?: number;
  month?: number;
  year?: number;
  totalSalesAmount: number;
  totalFixedCommission?: number;
  totalClassCommission?: number;
  fixedCommission?: number;
  percentageCommission?: number;
  bonusCommission: number;
  totalCommission: number;
  brandCommissions: BrandCommissionDetailDto[];
  classCommissions: ClassCommissionDetailDto[];
  calculatedOn?: Date;
  createdOn?: Date;
}

export interface CommissionCalculationDto {
  commissionID: number;
  salesmanID: number;
  saleMonth: number;
  saleYear: number;
  brandID: number;
  classID: number;
  totalCarsSold: number;
  totalSalesAmount: number;
  fixedCommission: number;
  percentageCommission: number;
  bonusCommission: number;
  totalCommission: number;
  calculatedOn: Date;
  createdOn: Date;
}

// Menu Models
export interface MenuDto {
  menuID: number;
  menuName: string;
  menuTitle: string;
  menuUrl: string | null;
  parentMenuID: number | null;
  iconClass: string | null;
  sortOrder: number;
  isActive: boolean;
  createdOn?: Date;
  lastUpdatedOn?: Date;
}

export interface UserMenuResponseDto {
  success: boolean;
  message: string;
  data: MenuDto[];
  errors: string[];
}
