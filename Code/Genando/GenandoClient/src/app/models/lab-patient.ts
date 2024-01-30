export interface LabPatient {
  clinicalProcessId: number;
  patientId: number;
  test: string;
  notes: string;
  deadline: Date;
  Intrefcode: string;
  isResultUploaded: boolean;
  isSampleRecieve: boolean;
  isResultsPublished: boolean;
}
