import { QuestionPublishStatus } from "src/app/constants/shared/question-publish-status";
import { IPatientQuestion } from "../patient/patient-question.interface";

export interface IPatientMoreQuestionRequest {
    questions: IPatientQuestion[],
    status: QuestionPublishStatus;
    deletedQuestions?: number[],
    questionIds: number[]
}