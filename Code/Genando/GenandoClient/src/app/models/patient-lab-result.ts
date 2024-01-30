import { TestResult } from './test-result';

export interface PatientLabResult {
  id: number;
  clinicalProcessId: number;
  firstName: string;
  lastName: string;
  email: string;
  gender: number;
  dob: Date;
  externalLink: string;
  avatar: string;
  testResults: TestResult[];
}
