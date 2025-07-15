import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class UrgentMessageService {

  public messages: string[] = [];

  constructor() { }

  public addMessage(msg: string) {
    this.messages.push(msg);
  }

  public getAllMessages() {
    return this.messages;
  }

}
