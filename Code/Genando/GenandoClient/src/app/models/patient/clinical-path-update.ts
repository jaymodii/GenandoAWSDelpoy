export interface ClinicalPathUpdate {
  id: number;
  question: string;
  typeOfQuestion: number;
  isQuestionMandatory: boolean;
  options: string;
  answer: string;
}
