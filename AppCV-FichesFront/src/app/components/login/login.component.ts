import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ValidatorService } from "../../validator.services";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
  constructor(private route: Router, private validator: ValidatorService) {}
  Email: string = null;
  PassWord: string = null;
  RememberMe:boolean=false
  ngOnInit() {}
  IsValid(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }
  IsValidPassword(value, errorEmpty, errorRegex) {
    debugger
    this.validator.ValidateEmpty(value, errorEmpty);

    this.validator.ValidadePassword(value, errorRegex);
  }
  Login(): void {
    // this.service.Login(this.UserId, this.AccessKey).subscribe(
    //   x => {
    //     debugger;
    //    let token = x["token"];
    //    if(x.authenticated){
    //      this.MsgError=""
    //     localStorage.setItem('token',token);
    //     this.route.navigate(["/pag1"])
    //    }else{
    //     localStorage.removeItem('token');
    //     this.MsgError="User or Password is wrong!!"
    //    }
    //   },
    //   (error: any) => {
    //     debugger;
    //   }
    // );
  }
}
