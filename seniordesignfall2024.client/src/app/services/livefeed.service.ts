import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private baseUrl = 'http://192.168.1.167:5556'; 

  constructor(private http: HttpClient) {}

  startVideoStream(): Observable<any> {
    return this.http.post(`${this.baseUrl}/start-video`, {});
  }
}