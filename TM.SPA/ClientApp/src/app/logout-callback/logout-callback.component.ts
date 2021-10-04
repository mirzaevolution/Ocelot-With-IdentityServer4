import { Component, OnInit } from '@angular/core';
import { User, UserManager } from 'oidc-client';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
@Component({
  selector: 'app-logout-callback',
  templateUrl: './logout-callback.component.html',
})
export class LogoutCallbackComponent implements OnInit {
  ngOnInit(): void {
    new UserManager({ response_mode: "query" })
      .signoutRedirectCallback().then(function () {
        localStorage.setItem(environment.authKeys.isAuthenticated, "0");
        localStorage.removeItem(environment.authKeys.accessToken);
        localStorage.removeItem(environment.authKeys.currentUser);
        window.location.href = "/";
      })
    }

}
