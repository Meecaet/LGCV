import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../Services/admin.service';
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';
import { UserViewModel } from '../../Models/Admin/User-model';
import { ActivatedRoute, Route, Router } from '../../../../node_modules/@angular/router';
import { RoleAdministratioViewModel } from '../../Models/Admin/RoleAdministration-model';
import { SelectItemViewModel } from '../../Models/Admin/SelectItem-model'
import { ValidatorService } from '../../validator.services';
import { Location } from '../../../../node_modules/@angular/common';

@Component({
  selector: 'app-role-edit',
  templateUrl: './role-edit.component.html',
  styleUrls: ['./role-edit.component.css']
})
export class RoleEditComponent implements OnInit {

  role: RoleAdministratioViewModel;
  usersManager: Array<SelectItemViewModel>;

  constructor(
    private validator: ValidatorService,
    private adminService: AdminService,
    private route: ActivatedRoute,
    private location: Location,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const roleId = params['id'];
      this.loadDetails(roleId);
      this.loadUsers();
    });
  }

  IsValid(value, errorEmpty) {
    errorEmpty.presse = true;
    this.validator.ValidateEmpty(value, errorEmpty);
  }

  Edit(): void {
    this.adminService.EditRol(this.role).subscribe(
      (data: RoleAdministratioViewModel) => {
        console.log(data);
        this.location.replaceState('/');
        this.router.navigate(['role/admin']);
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

  loadDetails(roleId: string) {
    this.adminService.GetDetail(roleId).subscribe(
      (data: RoleAdministratioViewModel) => {
        this.role = data;
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

  loadUsers() {
    this.adminService.GetUsers().subscribe(
      (data: Array<SelectItemViewModel>) => {
        this.usersManager = data;
        console.log(this.usersManager);
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

}
