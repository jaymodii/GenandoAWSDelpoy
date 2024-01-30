import { TestResult } from './test-result';

export interface LabPatientInfo {
  id: number;
  age: number;
  dob: Date;
  gender: string;
  externalLink: string;
  testTitle: string;
  testResults: TestResult;
}
