import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import {
  DropdownClinicalPrecessStep,
  LinksTitleConstant,
  SystemConstant,
} from 'src/app/constants/system-constant';
import { LabPatient } from 'src/app/models/lab-patient';
import { PageListRequest } from 'src/app/models/page-list-request';
import { AuthService } from 'src/app/services/auth.service';
import { LabService } from 'src/app/services/lab.service';
import { Dropdown } from 'src/app/shared/models/dropdown';

@Component({
  selector: 'app-lab-dashboard',
  templateUrl: './lab-dashboard.component.html',
  styleUrls: ['./lab-dashboard.component.scss'],
  providers: [],
})
export class LabDashboardComponent implements OnInit {
  userId: number = parseInt(this.authService.getUserId() || '');
  data: LabPatient[] = [];
  pageSizeOption: number[] = [10, 20, 30];
  pageSize: FormControl = new FormControl(10);
  totalRecords: number = 0;
  page: number = 1;
  dropDownListArray: Dropdown[][] = [];
  quickLinks: { text: string; route: string }[] = [
    { text: LinksTitleConstant.faq, route: RoutingPathConstant.qlLabFaq },
    {
      text: LinksTitleConstant.testExplaination,
      route: RoutingPathConstant.qlLabTestExplaination,
    },
  ];
  pageListRequest: PageListRequest = {
    pageIndex: 1,
    pageSize: 10,
  };

  constructor(
    private labService: LabService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.labService
      .getData(parseInt(this.authService.getUserId()!), this.pageListRequest)
      .subscribe((data: any) => {
        this.totalRecords = data.data.totalRecords;
        this.data = data.data.records.map((record: LabPatient) => {
          return {
            ...record,
            Intrefcode: SystemConstant.testPrefix + record.patientId,
          };
        });
        this.dropDownListArray = [];
        this.updateDropdowns();
      });
  }

  pageSizeChange() {
    this.pageListRequest.pageSize = this.pageSize.value;
    this.loadData();
  }

  onPageChange(newPage: number) {
    this.page = newPage;
    this.pageListRequest.pageIndex = this.page;
    this.loadData();
  }

  dropDownClick(event: any) {
    const clinicalProcessTestId: number = event.dataItem.clinicalProcessTestId;

    const selectedOption = event.dropDown;
    const patientId = event.dataItem.patientId;
    switch (selectedOption.value) {
      case 1:
        this.recieveSampleStatusUpdate(patientId);
        break;
      case 2:
        if (event.dataItem.isResultUploaded) {
          this.router.navigate([
            RoutingPathConstant.labUpdateResultUrl,
            clinicalProcessTestId,
          ]);
        } else {
          this.router.navigate([
            RoutingPathConstant.labUploadResultUrl,
            clinicalProcessTestId,
          ]);
        }
        break;
    }
    this.updateDropdowns();
  }

  updateDropdowns(): void {
    this.data.forEach((dataItem) => {
      const dropDownList: Dropdown[] = [
        {
          value: 1,
          text: DropdownClinicalPrecessStep.receiveSample,
          isDisabled: true,
        },
        {
          value: 2,
          text: DropdownClinicalPrecessStep.uploadResult,
          isDisabled: true,
        },
      ];
      if (dataItem.isResultUploaded) {
        dropDownList[1].text = DropdownClinicalPrecessStep.updateResult;
      }

      const isResultUploaded = dataItem.isResultUploaded;
      const isSampleRecieve = dataItem.isSampleRecieve;
      const isResultsPublished = dataItem.isResultsPublished;
      dropDownList.forEach((item) => {
        switch (item.value) {
          case 1:
            item.isDisabled = !(!isSampleRecieve && !isResultUploaded);
            break;
          case 2:
            if (isSampleRecieve && !isResultsPublished) {
              item.isDisabled = false;
            }
            break;
          default:
            item.isDisabled = true;
        }
      });
      this.dropDownListArray.push(dropDownList);
    });
  }

  recieveSampleStatusUpdate(patientId: number) {
    this.labService.markAsRecieve(patientId).subscribe(() => {
      this.loadData();
    });
  }
}
