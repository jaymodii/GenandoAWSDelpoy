import { GenderType } from "src/app/constants/shared/gender-type";
import { PatientConsultationStatusType } from "src/app/constants/shared/patient-consultation-status-type";

interface IUserBaseModel {
    id: number;
    name: string;
    email: string;
    gender: GenderType;
    dob: Date;
}

export interface IUserListingResponse extends IUserBaseModel{
    phoneNumber: string;
    status: PatientConsultationStatusType;
    avatar: string | null;
}

export interface IPatientTestInfoResponse extends IUserBaseModel {
    testTitle: string;
    referenceCode: string;
}
