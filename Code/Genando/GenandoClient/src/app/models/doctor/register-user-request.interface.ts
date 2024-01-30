export interface IUserDetailsFormRequest {
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string,
    dateOfBirth: Date | null,
    gender: number | null,
    address : string,
    isPatient : boolean
}
  