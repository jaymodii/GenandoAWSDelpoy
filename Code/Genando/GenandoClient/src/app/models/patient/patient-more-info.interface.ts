import { QuestionPublishStatus } from "src/app/constants/shared/question-publish-status";

export interface IPatientMoreInfoResponse {
    id: 1,
    question: string,
    answer: string,
}

export interface IPatientMoreInfoRequest {
    patientMoreInfo: IPatientMoreInfoResponse[],
    status: QuestionPublishStatus
}