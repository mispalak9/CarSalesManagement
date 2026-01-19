import { Injectable } from '@angular/core';
import { NotificationService } from './notification.service';

export interface ErrorDetails {
  status?: number;
  message: string;
  errors?: string[];
  timestamp?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {
  constructor(private notificationService: NotificationService) {}

  handleError(error: any): void {
    const errorDetails = this.parseError(error);
    this.logError(errorDetails);
    this.notifyError(errorDetails);
  }

  private parseError(error: any): ErrorDetails {
    let errorDetails: ErrorDetails = {
      message: 'An unexpected error occurred',
      timestamp: new Date()
    };

    if (error.error && typeof error.error === 'object') {
      // API Error Response
      errorDetails.status = error.status;
      errorDetails.message = error.error.message || error.message;
      errorDetails.errors = error.error.errors;
    } else if (error.message) {
      // Standard Error Object
      errorDetails.message = error.message;
      errorDetails.status = error.status;
    } else if (typeof error === 'string') {
      // String Error
      errorDetails.message = error;
    }

    return errorDetails;
  }

  private logError(errorDetails: ErrorDetails): void {
    console.error('Application Error:', errorDetails);
  }

  private notifyError(errorDetails: ErrorDetails): void {
    let message = errorDetails.message;

    if (errorDetails.errors && errorDetails.errors.length > 0) {
      message = errorDetails.errors.join(', ');
    }

    // Handle specific HTTP status codes
    if (errorDetails.status === 401) {
      message = 'Unauthorized. Please login again.';
    } else if (errorDetails.status === 403) {
      message = 'You do not have permission to perform this action.';
    } else if (errorDetails.status === 404) {
      message = 'Resource not found.';
    } else if (errorDetails.status === 500) {
      message = 'Server error. Please try again later.';
    }

    this.notificationService.error(message);
  }

  getErrorMessage(error: any): string {
    const errorDetails = this.parseError(error);
    if (errorDetails.errors && errorDetails.errors.length > 0) {
      return errorDetails.errors.join(', ');
    }
    return errorDetails.message;
  }
}
