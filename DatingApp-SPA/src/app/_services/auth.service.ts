import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl=environment.apiUrl;
private currentUserSource=new ReplaySubject<User>(1);
currentUser$=this.currentUserSource.asObservable();

constructor(private http:HttpClient) { }

login(model:any){
  return this.http.post<User>(this.baseUrl+'auth/login',model)
  .pipe(
    map((response:User)=>{
      const user=response;
      if(user){
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
        
    })
  )
}

setCurrentUser(user:User){
  this.currentUserSource.next(user);
}

register(model:any){
  return this.http.post<User>(this.baseUrl+'auth/register',model).pipe(
    map((user:User)=>{
      if (user) {
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
    })
  );
}

logout(){
  localStorage.removeItem('user');
  this.currentUserSource.next(null!);
}
}
