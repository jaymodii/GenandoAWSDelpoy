export enum ClinicalProcessStatus {
    initial = 1,
    clinicalPath = 2,
    prescribeTest = 3,
    collectSample = 4,
    shipSample = 5,
    receiveSample = 6,
    sampleAnalysis = 7,
    sendLabResults = 8,
    publishReport = 9,
    complete = 10,
}