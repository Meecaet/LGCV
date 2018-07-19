import { Injectable } from "@angular/core";

@Injectable()
export class ValidatorService {
  constructor() {}
  //Valid if the value is empty or not
  ValidateEmpty(value, element) {
    if (value != "" && value != null) {
      element.hidden = true;
    } else {
      element.hidden = false;
    }
  }
  //valid if the value is correct
  ValidateEmail(value, element) {
    if (element == null)
    return

      var regex = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      );
    if (regex.test(value)) {
      element.hidden = true;
    } else {
      element.hidden = false;
    }
  }
  //valid if the value is correct (8 caracters,upcase and lowcase)
  ValidadePassword(value, element) {
    if (element == null)
    return
    var regex = new RegExp(
      /^(?=(?:[^a-z]*[A-z]){2})(?=(?:[^0-9]*[0-9]){2}).{4,}$/
    );
    if (regex.test(value)) {
      element.hidden = true;
    } else {
      element.hidden = false;
    }
  }
}
