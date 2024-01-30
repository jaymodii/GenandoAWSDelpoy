// import { Action } from "@ngrx/store";
// import { IPatientPageRequest } from "../models/doctor/patient-page-request.interface";
// import { FilterAction, FilterActionType } from "./filter.action";

// const initialState: Array<IPatientPageRequest> = [];

// export function FilterReducer(
//    state: Array<IPatientPageRequest> = initialState,
//    action: Action
// ) {
//    switch (action.type) {
//      case FilterActionType.ADD_ITEM:
//        return [...state, (action as FilterAction).payload];
//      default:
//        return state;
//    }
// }


import { Action } from "@ngrx/store/public_api";
import { IPatientPageRequest } from "src/app/models/doctor/patient-page-request.interface";
import { FilterAction, FilterActionType } from "./filter.action";

const initialState: IPatientPageRequest[] = [];

export function FilterReducer(
  state: Array<IPatientPageRequest> = initialState,
  action: Action
) {
  switch (action.type) {
    case FilterActionType.ADD_ITEM:
      state = [];
      return [...state, (action as FilterAction).payload];
    case FilterActionType.REMOVE_ITEM:
      return [...state].filter(obj => {return obj !== (action as FilterAction).payload});
    default:
      return state;
  }
}
