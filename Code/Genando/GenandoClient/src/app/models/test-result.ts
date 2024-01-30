export interface TestResult {
  testResultId: number;
  reportAttachmentTitle: string;
  labNotes: string;
  doctorNotes: string;
  clinicalProcessTestId: number;
  testTitle: string;
  reportAttachmentBytes: ArrayBuffer;
  reportAttachment: File;
}
