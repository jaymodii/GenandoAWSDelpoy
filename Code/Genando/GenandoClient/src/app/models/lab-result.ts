import { TestResult } from './test-result';

export interface LabResult {
  clinicalProcessId: number;
  externalLink: string;
  testResults: TestResult[];
}
