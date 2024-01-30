import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subscription, debounceTime, distinctUntilChanged, of, switchMap, take, tap } from 'rxjs';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { PatientConsultationStatusType } from 'src/app/constants/shared/patient-consultation-status-type';
import { IPatientPageRequest } from 'src/app/models/doctor/patient-page-request.interface';
import { IUserListingResponse } from 'src/app/models/doctor/user-listing-response.interface';
import { IPageRequest } from 'src/app/models/page-request.interface';
import { IPageInfoResponse } from 'src/app/models/shared/page-info-response.interface';
import { UserService } from 'src/app/services/doctor/user.service';
import { ConfirmationDialogService } from 'src/app/shared/services/confirmation-dialog.service';
import { MessageService } from 'src/app/shared/services/message.service';
import { AddFilterAction } from 'src/app/store-ngrx/filter.action';
import { PreserveFilterModelInterface } from 'src/app/store-ngrx/state.model';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  styleUrls: ['./patient.component.scss'],
})
export class PatientComponent implements OnInit, OnDestroy {
  public patientsRecords!: IPageInfoResponse<IUserListingResponse>;
  public filterForm!: FormGroup;
  public pageRequest!: IPageRequest;
  public pageSize: FormControl = new FormControl(10);
  public filterRequest!: IPatientPageRequest;
  public pageSizeOption: number[] = [10, 20, 30];
  filter$!: Observable<Array<IPatientPageRequest>>;
  seachKey: string = '';
  subscription !: Subscription;

  constructor(
    private userService: UserService,
    private messageService: MessageService,
    private router: Router,
    private confirmDialogService: ConfirmationDialogService,
    private store: Store<PreserveFilterModelInterface>
  ) { }

  public ngOnInit(): void {
    this.filterForm = new FormGroup({
      search: new FormControl(''),
      gender: new FormControl(''),
      status: new FormControl(''),
    });
    this.pageRequest = this.setInitialPageRequest();
    this.filterRequest = {
      pageRequest: this.pageRequest,

    };
    this.filter$ = this.store.select((store) => store.arrayOfState);
    this.store.subscribe((res: any) => {
      if (!res || res['filter'].length < 1) {
        this.filterRequest = {
          pageRequest: this.pageRequest,
        };
      }
      else {
        this.filterRequest = res['filter'][res['filter'].length - 1];
        if (!this.filterRequest.pageRequest) {
          this.filterRequest.pageRequest = this.pageRequest;
        }
        else
          this.seachKey = this.filterRequest.pageRequest.searchKey ?? '';
      }
      this.pageSize.setValue(this.filterRequest.pageRequest?.pageSize);
      this.filterForm.get('search')?.setValue(this.filterRequest.pageRequest?.searchKey ? this.filterRequest.pageRequest?.searchKey : '');
      this.filterForm.get('gender')?.setValue(this.filterRequest.gender ? this.filterRequest.gender : '');
      this.filterForm.get('status')?.setValue(this.filterRequest.status ? this.filterRequest.status : '');
    })
    this.loadPatientsRecords();
    this.setFilterForm();
  }

  private setInitialPageRequest() {
    const pageRequest: IPageRequest = {
      pageNumber: 1,
      pageSize: this.pageSize.value,
    };
    return pageRequest;
  }

  private setFilterForm(): void {
    this.subscription = this.filterForm.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((filter) => {
          this.pageRequest = this.setInitialPageRequest();
          this.pageRequest.searchKey = filter.search;
          this.seachKey = filter.search ?? '';
          const filterRequest: IPatientPageRequest = {
            pageRequest: this.pageRequest,
            gender: parseInt(filter.gender),
            status: parseInt(filter.status),
          };
          this.filterRequest = { ...filterRequest };
          this.userService
            .loadPatients(this.filterRequest)
            .subscribe((res) => (this.patientsRecords = res.data));
          return of(filter);
        })
      )
      .subscribe();
  }

  private loadPatientsRecords(): void {
    this.userService
      .loadPatients(this.filterRequest)
      .subscribe((res) => (this.patientsRecords = res.data));
  }

  public navigateToPatient(id: number): void {
    this.router.navigate([RoutingPathConstant.patientListUrl + '/', id]);
  }

  public deleteUser(patient: any): void {
    this.confirmDialogService
      .confirm(
        'Remove Patient?',
        `Are you sure you want to remove ${patient.name}?`
      )
      .then((confirmed) => {
        if (!confirmed) return;

        this.userService.deleteUser(patient.id).subscribe({
          next: () => {
            this.loadPatientsRecords();
          },
        });
      })
      .catch(() => console.log(''));
  }

  public getStatusClass(status: PatientConsultationStatusType): string {
    let statusClass: string = '';
    switch (status) {
      case PatientConsultationStatusType.New:
        statusClass = 'status-new';
        break;
      case PatientConsultationStatusType.OnGoing:
        statusClass = 'status-on-going';
        break;
      case PatientConsultationStatusType.Done:
        statusClass = 'status-done';
        break;
    }
    return statusClass;
  }

  public pageSizeChange(): void {
    this.pageRequest.pageSize = parseInt(this.pageSize.value);
    this.pageRequest.searchKey = this.seachKey;
    this.filterRequest = { ...this.filterRequest, pageRequest: this.pageRequest };
    this.loadPatientsRecords();
  }

  public onPageChange(val: number): void {
    this.pageRequest.pageNumber = val;
    this.pageRequest.searchKey = this.seachKey;
    this.filterRequest = { ...this.filterRequest, pageRequest: this.pageRequest }
    this.loadPatientsRecords();
  }

  public ngOnDestroy(): void {
    this.messageService.clear();
    this.store.dispatch(
      new AddFilterAction(this.filterRequest)
    );
    this.subscription.unsubscribe();
  }
}