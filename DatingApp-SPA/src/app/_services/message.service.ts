import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/Group';
import { Message } from '../_models/Message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginatedResult, getPaginationHeaders } from './PaginationHelpers';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
baseUrl=environment.apiUrl;
hubUrl=environment.hubUrl;
  private hubConnection!: HubConnection;
  private messageThreadSource=new BehaviorSubject<Message[]>([]);
  messageThread$=this.messageThreadSource.asObservable();

  constructor(private http:HttpClient,private busyService:BusyService) { }

  createHubConnection(user:User,otherUsername:string){
    this.busyService.busy();
    this.hubConnection=new HubConnectionBuilder()
    .withUrl(this.hubUrl+'message?user='+otherUsername,{
      accessTokenFactory:()=>user.token}).withAutomaticReconnect().build();

    this.hubConnection.start()
    .catch(err=>{console.log(err);
    }).finally(()=>this.busyService.idle());

    this.hubConnection.on("ReceiveMessageThread",messages=>{
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on("NewMessage",message=>{
      this.messageThread$.pipe(take(1)).subscribe(messages=>{
        this.messageThreadSource.next([...messages,message]);
      })
    });

    this.hubConnection.on("UpdatedGroup",(grp:Group)=>{
      if (grp.connections.some(x=>x.username===otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(msgs=>{
          msgs.forEach(msg=>{
            if(!msg.dateRead)
              msg.dateRead=new Date(Date.now());
          })
          this.messageThreadSource.next([...msgs]);
        })
      }
    })
  }

  stopHubConnection(){
    if(this.hubConnection)
    {
      this.messageThreadSource.next([]);
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber: number,pageSize: number,container: any){
    let params=getPaginationHeaders(pageNumber,pageSize);

    params=params.append('Container',container);

    return getPaginatedResult<Message[]>(this.baseUrl+'messages',params,this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username);
  }

  async sendMessage(username:string,content:string){
    return this.hubConnection.invoke("SendMessage",{recipientUsername:username,content})
    .catch(err=>{console.log(err);
    });
  }

  deleteMessage(id:number){
    return this.http.delete(this.baseUrl+'messages/'+id);
  }
}
