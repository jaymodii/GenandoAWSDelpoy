// import { Action } from "@ngrx/store";
// import { IPatientPageRequest } from "../models/doctor/patient-page-request.interface";

// export enum FilterActionType {
//   ADD_ITEM = '[ARTICLE] Add ARTICLE',
// }

// export class AddFilterAction implements Action {

//   readonly type = FilterActionType.ADD_ITEM;
//   constructor(public payload: IPatientPageRequest) {}

// }

// export type FilterAction = AddFilterAction;

import { Action } from '@ngrx/store';
import { IPatientPageRequest } from 'src/app/models/doctor/patient-page-request.interface';

export enum FilterActionType {
  ADD_ITEM = '[FILTER] Add FILTER',
  REMOVE_ITEM = '[FILTER] Remove FILTER',
}

export class AddFilterAction implements Action {

  readonly type = FilterActionType.ADD_ITEM;
  constructor(public payload: IPatientPageRequest) {}

}

export class RemoveFilterAction implements Action {

  readonly type = FilterActionType.REMOVE_ITEM;
  constructor(public payload: IPatientPageRequest) {}

}

export type FilterAction = AddFilterAction | RemoveFilterAction ;
