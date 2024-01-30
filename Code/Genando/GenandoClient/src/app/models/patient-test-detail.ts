import { TestExplanation } from './test-explanation';

export interface PatientTestDetail {
  testDetails: TestExplanation[];
  patientName: string;
  email: string;
  dob: Date;
  avatar: string;
  gender: number;
  prescribedTestId: number[];
}
