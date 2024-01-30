import { GenderType } from "src/app/constants/shared/gender-type";
import { IPageRequest } from "../page-request.interface";
import { PatientConsultationStatusType } from "src/app/constants/shared/patient-consultation-status-type";

export interface IPatientPageRequest {
    pageRequest?: IPageRequest | null,
    gender?: GenderType | null,
    status?: PatientConsultationStatusType | null
}