import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Observable, switchMap, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImagingService {

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) { }

  getJpeg(): Observable<SafeUrl> {
    return this.http.get("/api/Imaging/Jpeg", { responseType: 'blob', headers: { Accept: 'image/jpeg' } })
      .pipe(
        switchMap(blob => {
          const reader = new FileReader();
          reader.readAsDataURL(blob);
          return new Observable<string>(observer => {
            reader.onloadend = () => {
              observer.next(reader.result as string);
              observer.complete();
            };
          });
        }),
        map(dataUrl => this.sanitizer.bypassSecurityTrustUrl(dataUrl))
      )
  }
}
  // stillCap(imageUrl:string): Observable<Blob>{
  //   return this.httpClient.get(`/api/Imaging/Jpeg`, {responseType: 'blob','headers':new HttpHeaders({'Accept':'image/jpeg'})});
  // }