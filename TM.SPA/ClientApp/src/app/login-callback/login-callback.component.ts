import { Component, OnInit } from '@angular/core';
import { User, UserManager } from 'oidc-client';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
@Component({
  selector: 'app-login-callback',
  templateUrl: './login-callback.component.html',
})
export class LoginCallbackComponent implements OnInit {
  ngOnInit(): void {
    new UserManager({ response_mode: "query" })
      .signinRedirectCallback().then(function (user: User) {
        if (user) {

          console.log(`Logged in. Object payload: ${JSON.stringify(user)}`);
          console.log(user);
          localStorage.setItem(environment.authKeys.isAuthenticated, "1");
          localStorage.setItem(environment.authKeys.accessToken, user.access_token);
          localStorage.setItem(environment.authKeys.currentUser, user.profile.name);
        }
        window.location.href = "/";
      })
    }

}
