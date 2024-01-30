import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IUserListingResponse } from 'src/app/models/doctor/user-listing-response.interface';
import { IResponse } from 'src/app/models/shared/response';
import { UserService } from 'src/app/services/doctor/user.service';
import { ConfirmationDialogService } from 'src/app/shared/services/confirmation-dialog.service';

@Component({
  selector: 'app-lab',
  templateUrl: './lab.component.html',
  styleUrls: ['./lab.component.scss'],
})
export class LabComponent implements OnInit {
  public labUser!: IUserListingResponse;

  constructor(
    private userService: UserService,
    private router: Router,
    private confirmDialogService: ConfirmationDialogService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadLabDetails();
  }

  public loadLabDetails(): void {
    this.userService.loadLabDetails().subscribe({
      next: (data: IResponse<IUserListingResponse>) => {
        this.labUser = data.data;
      },
    });
  }

  public navigateToLab(id: number): void {
    this.router.navigate(['.', id], { relativeTo: this.route });
  }

  public navigateToRegister(): void {
    this.router.navigate(['register'], { relativeTo: this.route });
  }

  public deleteUser(user: IUserListingResponse): void {
    this.confirmDialogService
      .confirm(
        'Remove Lab User?',
        `Are you sure you want to remove ${user.name}?`
      )
      .then((confirmed) => {
        if (!confirmed) return;

        this.userService.deleteUser(user.id).subscribe({
          next: () => {
            this.loadLabDetails();
          },
        });
      })
      .catch(() => console.log(''));
  }
}
