import { Component, OnInit } from '@angular/core';
import { User, UserManager, UserManagerSettings } from 'oidc-client'
import { environment } from '../../environments/environment';
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  private _userManager: UserManager;

  ngOnInit(): void {
    this._userManager = new UserManager({
      authority: environment.oidc.baseAddress,
      client_id: environment.oidc.clientId,
      redirect_uri: environment.oidc.redirectUri,
      post_logout_redirect_uri: environment.oidc.postLogoutRedirectUri,
      scope: environment.oidc.scopes,
      response_type: environment.oidc.responseType,

    });
  }
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
  login(): void {
    
    this._userManager.signinRedirect();
  }
  logout(): void {
    this._userManager.signoutRedirect();
  }
}
