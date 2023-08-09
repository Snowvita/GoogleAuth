import { Component } from '@angular/core';
import { SocialAuthService  } from '@abacritt/angularx-social-login';
import { SocialUser } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';
import { HttpClient,HttpHeaders } from '@angular/common/http';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title(title: any) {
    throw new Error('Method not implemented.');
  }
  user: SocialUser | null;
  hasApiAccess = false;

  constructor(private authService: SocialAuthService, private http: HttpClient) 
  { 
	this.user = null;
	this.authService.authState.subscribe((user: SocialUser) => {
	  console.log(user);
	  if (user) {
		this.http.post<any>('http://localhost:5117/user/authenticate', { idToken: user.idToken }).subscribe((authToken: any) => {
            console.log(authToken);

			let reqHeader = new HttpHeaders({ 
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + authToken.authToken
				});
			this.http.get<any>('http://localhost:5117/user', { headers: reqHeader }).subscribe((data: any) => {
			this.hasApiAccess = true;
			})		
        })		  
	  }
	  this.user = user;
	});
  }

  signInWithGoogle(): void {
    this.authService.signIn(GoogleLoginProvider.PROVIDER_ID).then((x: any) => console.log(x));
  }


  signOut(): void {
    this.authService.signOut();
	this.hasApiAccess = false;
  }  
}