import { Pipe, PipeTransform } from '@angular/core';
import { PatientConsultationStatusType } from '../constants/shared/patient-consultation-status-type';

@Pipe({
    name: 'patientDashboardStatus'
})

export class PatientDashboardStatusPipe implements PipeTransform {
    transform(value: number): string {
        switch (value) {
            case PatientConsultationStatusType.New:
                return 'New';
            
            case PatientConsultationStatusType.OnGoing:
                return 'On Going';

            case PatientConsultationStatusType.Done:
                return 'Done';
        }
        return '';
    }
}