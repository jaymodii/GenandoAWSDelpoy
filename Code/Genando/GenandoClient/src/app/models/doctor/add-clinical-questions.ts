export interface AddClinicalQuestions {
  questionId:number;
  question: string;
  typeOfQuestion: number;
  options: string | null;
  isQuestionMandatory: boolean;
}
