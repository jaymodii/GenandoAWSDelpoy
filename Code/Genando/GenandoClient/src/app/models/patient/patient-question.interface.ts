import { QuestionPublishStatus } from "src/app/constants/shared/question-publish-status";

export interface IPatientQuestion {
    id: number, 
    question: string,
    status: QuestionPublishStatus 
}