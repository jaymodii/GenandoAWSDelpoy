export interface Error {
  error: ErrorResponse;
}

export interface ErrorResponse {
  message: string;
  success: boolean;
  errors: string[];
  statusCode: number;
}
