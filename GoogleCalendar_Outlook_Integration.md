# Google Calendar & Outlook Calendar Integration Guide

## Google Calendar Integration

1. **Create a project in Google Cloud Console**
   - Go to https://console.cloud.google.com/
   - Enable the "Google Calendar API".
   - Create an OAuth2 Client ID (Web application).
   - Add http://localhost:4200 as an authorized URI.

2. **Get your client_id**

3. **Add the Google API script to your index.html:**
```html
<script src="https://apis.google.com/js/api.js"></script>
```

4. **Sample code for authentication and fetching events:**
```typescript
// MeetingsListComponent (fragment)
declare const gapi: any;
const CLIENT_ID = 'YOUR_CLIENT_ID.apps.googleusercontent.com';
const SCOPES = 'https://www.googleapis.com/auth/calendar.events.readonly';

initGoogleApi(): Promise<void> {
  return new Promise(resolve => {
    gapi.load('client:auth2', () => {
      gapi.client.init({
        clientId: CLIENT_ID,
        scope: SCOPES,
        discoveryDocs: ['https://www.googleapis.com/discovery/v1/apis/calendar/v3/rest'],
      }).then(() => resolve());
    });
  });
}

syncWithGoogle() {
  this.initGoogleApi().then(() => {
    gapi.auth2.getAuthInstance().signIn().then(() => {
      gapi.client.calendar.events.list({
        calendarId: 'primary',
        timeMin: (new Date()).toISOString(),
        showDeleted: false,
        singleEvents: true,
        maxResults: 10,
        orderBy: 'startTime'
      }).then((response: any) => {
        const events = response.result.items;
        // Import events into meetings
      });
    });
  });
}
```

5. **For production, use a backend proxy for token/refresh handling.**

---

## Outlook Calendar (Microsoft) Integration

1. **Register an app at https://portal.azure.com/**
   - Get your client_id.
   - Add http://localhost:4200 as a redirect URI.
   - Set permissions: Calendars.Read, Calendars.ReadWrite

2. **Install MSAL.js:**
```sh
npm install @azure/msal-browser
```

3. **Sample code for authentication:**
```typescript
import { PublicClientApplication } from '@azure/msal-browser';
const msalConfig = {
  auth: {
    clientId: 'YOUR_CLIENT_ID',
    redirectUri: 'http://localhost:4200'
  }
};
const msalInstance = new PublicClientApplication(msalConfig);

async function signInAndGetEvents() {
  const loginResponse = await msalInstance.loginPopup({ scopes: ["Calendars.Read"] });
  const accessToken = loginResponse.accessToken;
  // Use accessToken to call Microsoft Graph API
}
```

4. **Microsoft Graph API for events:**
- Docs: https://learn.microsoft.com/en-us/graph/api/resources/event?view=graph-rest-1.0

---

## Notes
- For production, use backend OAuth2 flow to protect client_secret and refresh tokens.
- For demo, client_id and popup/redirect are sufficient.
- See MeetingsListComponent and official Google/Microsoft docs for more code samples.
