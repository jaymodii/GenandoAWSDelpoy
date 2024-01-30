export interface IPageInfoResponse<T> {

  totalRecords: number;
  content: T[];
  totalPage: number;
  pageNumber: number;
  pageSize: number;

}
