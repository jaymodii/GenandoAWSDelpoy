import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { MessageConstant } from 'src/app/constants/message-constant';
import { profileImage } from 'src/app/constants/profile-image/profile-image';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { ClinicalProcessStatus } from 'src/app/constants/shared/clinical-process-status';
import { GenderType } from 'src/app/constants/shared/gender-type';
import {
  DropdownClinicalPrecessStep,
  LinksTitleConstant,
} from 'src/app/constants/system-constant';
import { DoctorPatient } from 'src/app/models/doctor-patient';
import { IPatientDetailInterface } from 'src/app/models/doctor/patient-detail.interface';
import { DropdownItem } from 'src/app/models/drop-down-item';
import { PageListRequest } from 'src/app/models/page-list-request';
import { PatientLabResult } from 'src/app/models/patient-lab-result';
import { PatientTestDetail } from 'src/app/models/patient-test-detail';
import { PrescribeTest } from 'src/app/models/prescribe-test';
import { IResponse } from 'src/app/models/shared/response';
import { TestExplanation } from 'src/app/models/test-explanation';
import { AuthService } from 'src/app/services/auth.service';
import { DoctorService } from 'src/app/services/doctor.service';
import { UserService } from 'src/app/services/doctor/user.service';
import { DialogBoxComponent } from 'src/app/shared/components/dialog-box/dialog-box.component';
import { Dropdown } from 'src/app/shared/models/dropdown';
import { MessageService } from 'src/app/shared/services/message.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  data: DoctorPatient[] = [];
  pageSizeOption: number[] = [10, 20, 30];
  pageSize: FormControl = new FormControl(10);
  totalRecords: number = 0;
  page: number = 1;
  dropDownListArray: Dropdown[][] = [];
  genderType: number = GenderType.Male;
  sendLabResults: number = ClinicalProcessStatus.sendLabResults;

  quickLinks: { text: string; route: string }[] = [
    {
      text: LinksTitleConstant.createNewPatient,
      route: RoutingPathConstant.qlPatientRegister,
    },
    {
      text: LinksTitleConstant.createQuestions,
      route: RoutingPathConstant.addClinicalQuestionsUrl,
    },
  ];
  otherLinks: { text: string; route: string }[] = [
    {
      text: LinksTitleConstant.contactUs,
      route: RoutingPathConstant.qlContactUs,
    },
    {
      text: LinksTitleConstant.testExplaination,
      route: RoutingPathConstant.qlTestExplaination,
    },
  ];
  pageListRequest: PageListRequest = {
    pageIndex: 1,
    pageSize: 10,
  };
  @ViewChild('resultDialogBox') resultDialogBox!: TemplateRef<any>;
  @ViewChild('prescribeDialogBox') prescribeDialogBox!: TemplateRef<any>;

  @ViewChild('dialogBox') dialogBox!: TemplateRef<any>;
  @ViewChild('patientDetailDialogBox')
  patientDetailDialogBox!: TemplateRef<any>;
  patientDetail!: IPatientDetailInterface;
  avatarUrl!: string;
  patientLabResult!: PatientLabResult;
  patientTestDetail!: PatientTestDetail;
  testDropdown: DropdownItem[] = [];
  testDeatilForm: FormGroup = new FormGroup({
    testDetailControl: new FormControl(),
  });
  selectedTests: number[] = [];
  clinicalProcessId: number = 0;
  prescribeTest: PrescribeTest = {
    clinicalProcessId: 0,
    testIds: [],
  };
  selectedOption: any;
  prescribeTestForm: FormGroup = new FormGroup({
    testOptions: new FormControl(),
  });
  constructor(
    private doctorService: DoctorService,
    private router: Router,
    private modalService: NgbModal,
    private userService: UserService,
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.userService
      .getPatientForDashboard(
        parseInt(this.authService.getUserId()!),
        this.pageListRequest
      )
      .subscribe((data: any) => {
        this.totalRecords = data.data.totalRecords;
        this.data = data.data.records;
        
        this.data.forEach((dataItem) => {
          const currentStep = dataItem.nextStep;

          if (currentStep === DropdownClinicalPrecessStep.initial) {
            dataItem.nextStep =
              DropdownClinicalPrecessStep.addClinicalQuestions;
          } else if (currentStep === DropdownClinicalPrecessStep.clinicalPath) {
            dataItem.nextStep = DropdownClinicalPrecessStep.waitForPatient;
          } else if (
            currentStep === DropdownClinicalPrecessStep.sampleAnalysis ||
            currentStep === DropdownClinicalPrecessStep.receiveSample ||
            currentStep === DropdownClinicalPrecessStep.sendLabResults
          ) {
            dataItem.nextStep = DropdownClinicalPrecessStep.waitForLab;
          }
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
    this.selectedOption = event.dropDown;
    const patientId = event.dataItem.patientId;
    this.clinicalProcessId = event.dataItem.clinicalProcessId;
    switch (this.selectedOption.value) {
      case 1:
        this.router.navigate([RoutingPathConstant.contactUsUrl]);
        break;
      case 2:
        this.openDialogPrescribeTest();
        break;
      case 3:
        this.collectSampleStatusUpdate(patientId);
        break;
      case 4:
        this.shipSampleStatusUpdate(patientId);
        break;
      case 5:
        this.openResultDialog(this.selectedOption.value);
        break;
      case 6:
        this.router.navigate([RoutingPathConstant.reqMoreDoctUrl, patientId]);
        break;
      case 7:
        this.publishResult();
        break;
      case 8:
        this.openResultDialog(this.selectedOption.value);
        break;
      case 9:
        this.router.navigate([
          RoutingPathConstant.seeClinicalPathUrl,
          patientId,
        ]);
        break;
      case 10:
        this.router.navigate([
          RoutingPathConstant.seeRequestedInfoUrl,
          patientId,
        ]);
        break;
      case 11:
        this.router.navigate([
          RoutingPathConstant.addClinicalQuestions,
          patientId,
        ]);
        break;
    }
  }

  updateDropdowns(): void {
    this.data.forEach((dataItem) => {
      const dropDownList: Dropdown[] = [
        { value: 11, text: DropdownClinicalPrecessStep.addClinicalQuestions },
        { value: 9, text: DropdownClinicalPrecessStep.seeClinicalPath },
        { value: 2, text: DropdownClinicalPrecessStep.prescribeTest },
        { value: 3, text: DropdownClinicalPrecessStep.collectSample },
        { value: 4, text: DropdownClinicalPrecessStep.shipSample },
        { value: 5, text: DropdownClinicalPrecessStep.seeLabResult },
        { value: 7, text: DropdownClinicalPrecessStep.publishReport },
        { value: 8, text: DropdownClinicalPrecessStep.Report },
        { value: 6, text: DropdownClinicalPrecessStep.reqMoreInfo },
        { value: 10, text: DropdownClinicalPrecessStep.seeRequestedInfo },
        { value: 1, text: DropdownClinicalPrecessStep.contactGenando },
      ];
      const currentStep = dataItem.nextStep;

      dropDownList.forEach((item) => {
        switch (item.value) {
          case 11: // Add Clinical Questions
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.addClinicalQuestions;
            break;
          case 1: // Contact Genando
            item.isDisabled = false;
            break;
          case 2: // Prescribe Test
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.prescribeTest;
            break;
          case 3: // Collect Sample
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.collectSample;
            break;
          case 4: // Ship Sample
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.shipSample;
            break;
          case 5: // See Results
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.publishReport;
            break;
          case 6: // Request More Info
            item.isDisabled =
              currentStep === DropdownClinicalPrecessStep.initial ||
              currentStep ===
                DropdownClinicalPrecessStep.addClinicalQuestions ||
              currentStep === DropdownClinicalPrecessStep.prescribeTest ||
              currentStep === DropdownClinicalPrecessStep.complete;
            break;
          case 7: // Publish Result
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.publishReport;
            break;
          case 8: // Report
            item.isDisabled =
              currentStep !== DropdownClinicalPrecessStep.complete;
            break;
          case 9: // See Clinical Path
            item.isDisabled =
              currentStep === DropdownClinicalPrecessStep.waitForPatient ||
              currentStep === DropdownClinicalPrecessStep.addClinicalQuestions;
            break;
          case 10: // See Requested Info
            item.isDisabled =
              currentStep == DropdownClinicalPrecessStep.addClinicalQuestions;
            break;
          default:
            item.isDisabled = true; // Disable other items by default
        }
      });
      this.dropDownListArray.push(dropDownList);
    });
  }

  downloadResult(clinicalProcessTestId: number, fileName: string) {
    this.doctorService.downloadLabResult(clinicalProcessTestId).subscribe({
      next: (response: ArrayBuffer) => {
        const blob = new Blob([response], { type: 'application/pdf' });
        const blobUrl = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = blobUrl;
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(blobUrl);
      },
      error: (error) => {
        console.error(MessageConstant.errorDownloadFile, error);
      },
    });
  }

  openResultDialog(selectedOptionValue: number) {
    this.selectedOption = selectedOptionValue;
    this.doctorService.getLabResult(this.clinicalProcessId).subscribe({
      next: (data: any) => {
        this.patientLabResult = data.data;
        if (this.patientLabResult && this.patientLabResult.avatar === '') {
          this.patientLabResult.avatar =
            this.patientLabResult.gender == GenderType.Male
              ? profileImage.maleProfile
              : profileImage.gridFemaleProfile;
        }
      },
    });
    const modalOptions: NgbModalOptions = {
      size: 'lg',
      windowClass: 'custom-modal-class',
    };

    const modalRef = this.modalService.open(DialogBoxComponent, modalOptions);

    if (selectedOptionValue !== this.sendLabResults) {
      modalRef.componentInstance.modalTitle =
        DropdownClinicalPrecessStep.seeLabResult;
    } else {
      modalRef.componentInstance.modalTitle =
        DropdownClinicalPrecessStep.Report;
    }

    modalRef.componentInstance.modalContent = this.resultDialogBox;

    if (selectedOptionValue !== this.sendLabResults) {
      modalRef.componentInstance.modalBtnLabel3 =
        DropdownClinicalPrecessStep.publishReport;
    }
    modalRef.componentInstance.clickBtn3.subscribe(() => {
      this.publishResult();
      modalRef.close();
    });
  }

  publishResult() {
    this.router.navigate([
      RoutingPathConstant.sentResultUrl + '/' + this.clinicalProcessId,
    ]);
  }

  openDialogPrescribeTest() {
    this.doctorService.getPatientTestDetails(this.clinicalProcessId).subscribe({
      next: (data: IResponse<PatientTestDetail>) => {
        this.patientTestDetail = data.data;

        if (this.testDropdown.length === 0) {
          let i = 1;
          for (let item of this.patientTestDetail.testDetails) {
            const dropDownItem: DropdownItem = {
              value: i,
              viewValue: item.title,
            };
            this.testDropdown.push(dropDownItem);
            i++;
          }
        }

        if (this.patientTestDetail.avatar === '') {
          this.patientTestDetail.avatar =
            this.patientTestDetail.gender == GenderType.Male
              ? profileImage.maleProfile
              : profileImage.gridFemaleProfile;
        }

        const modalOptions: NgbModalOptions = {
          size: 'lg',
          windowClass: 'custom-modal-class',
        };

        const modalRef = this.modalService.open(
          DialogBoxComponent,
          modalOptions
        );

        modalRef.componentInstance.modalTitle =
          DropdownClinicalPrecessStep.prescribeTest;
        modalRef.componentInstance.modalContent = this.prescribeDialogBox;
        modalRef.componentInstance.modalBtnLabel3 =
          DropdownClinicalPrecessStep.prescribeTest;
        modalRef.componentInstance.clickBtn3.subscribe(() => {
          if (this.selectedTests.length > 0) {
            this.prescribeTest.clinicalProcessId = this.clinicalProcessId;
            this.prescribeTest.testIds = this.selectedTests;

            this.doctorService.prescribeTest(this.prescribeTest).subscribe({
              next: (res: IResponse<null>) => {
                this.loadData();
              },
            });
            modalRef.close();
          } else {
            this.messageService.error(MessageConstant.selectTestFirst);
          }
        });
      },
    });
  }

  selectTest(selectedValues: string) {
    const selectedValueArray = selectedValues.split(',');
    this.selectedTests = [];
    for (let item of selectedValueArray) {
      const test = this.patientTestDetail.testDetails.find(
        (x) => x.title === item
      );
      if (test) {
        this.selectedTests.push(test.id);
      }
    }
  }

  openDialogPatientDetail(id: number) {
    this.userService.getPatientDetails(id).subscribe((res: any) => {
      this.patientDetail = res;
      if (this.patientDetail.avatar == '') {
        this.avatarUrl =
          this.patientDetail.gender == GenderType.Male
            ? profileImage.maleProfile
            : profileImage.gridFemaleProfile;
      } else {
        this.avatarUrl = this.patientDetail.avatar;
      }

      //open modal for patient details
      const modalOptions: NgbModalOptions = {
        size: 'lg',
        windowClass: 'custom-modal-class',
      };
      const modalRef = this.modalService.open(DialogBoxComponent, modalOptions);
      modalRef.componentInstance.modalTitle =
        MessageConstant.patientDetailModalTitle;
      modalRef.componentInstance.modalContent = this.patientDetailDialogBox;
      modalRef.componentInstance.clickBtn3.subscribe(() => {
        modalRef.close();
      });
    });
  }

  collectSampleStatusUpdate(patientId: number) {
    this.doctorService.markAsCollectSample(patientId).subscribe(() => {
      this.loadData();
    });
  }

  shipSampleStatusUpdate(patientId: number) {
    this.doctorService.markAsShipSample(patientId).subscribe(() => {
      this.loadData();
    });
  }

  getFullExternalLink(link: string): string {
    if (link.startsWith('http://') || link.startsWith('https://')) {
      return link;
    } else {
      return 'http://' + link;
    }
  }
}
