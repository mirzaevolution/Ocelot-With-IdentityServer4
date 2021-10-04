// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiUrl: "https://localhost:44332",
  oidc: {
    baseAddress: "https://localhost:44331",
    clientId: "TM.SPA",
    scopes: "openid profile email Roles TM.Api:read",
    redirectUri: "https://localhost:44342/login-callback",
    responseType: "code",
    postLogoutRedirectUri: "https://localhost:44342/logout-callback"
  },
  authKeys: {
    isAuthenticated: "is_authenticated",
    accessToken: "access_token",
    currentUser: "current_user"
  }
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
