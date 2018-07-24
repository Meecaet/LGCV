import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../Services/admin.service';
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';
import { UserViewModel } from '../../Models/Admin/User-model';
import { ActivatedRoute, Route, Router } from '../../../../node_modules/@angular/router';
import { RoleAdministratioViewModel } from '../../Models/Admin/RoleAdministration-model';
import { ValidatorService } from '../../validator.services';
import { Location } from '../../../../node_modules/@angular/common';

@Component({
  selector: 'app-role-edit',
  templateUrl: './role-edit.component.html',
  styleUrls: ['./role-edit.component.css']
})
export class RoleEditComponent implements OnInit {

  role: RoleAdministratioViewModel;
  applicationUsers: Array<UserViewModel>;
  selectedUserId: string;

  constructor(
    private validator: ValidatorService,
    private adminService: AdminService,
    private route: ActivatedRoute,
    private location: Location,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const roleId = params['id'];
      this.loadUsers();
      this.loadDetails(roleId);
    });
  }

  isValid(value, errorEmpty) {
    errorEmpty.presse = true;
    this.validator.ValidateEmpty(value, errorEmpty);
  }

  editRole(): void {
    this.adminService.EditRol(this.role).subscribe(
      (data: RoleAdministratioViewModel) => {
        this.location.replaceState('/');
        this.router.navigate(['role/admin']);
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

  addUser(): void {
    this.adminService.AddUserRole(this.role.role.id, this.selectedUserId).subscribe(
      (data: Array<UserViewModel>) => {
        this.role.users = data;
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

  deleteUser(userId: string): void {
    this.adminService.DeleteUserRol(this.role.role.id, userId).subscribe(
      (data: Array<UserViewModel>) => {
        this.role.users = data;
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
      (data: Array<UserViewModel>) => {
        let _default = new UserViewModel();
        _default.userName = "---Selecionez un utilisateur---";
        _default.id = "-1";
        this.selectedUserId = _default.id;
        data.unshift(_default);
        this.applicationUsers = data;
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

}
