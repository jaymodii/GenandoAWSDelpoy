export interface LabUploadData {
    clinicalProcessId: number;
    reportAttachmentTitle: string;
    notes: string;
    externalLink: string;
    reportAttachment?: File;
}
