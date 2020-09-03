import { Injectable } from '@angular/core';
import { Observable, of, interval, timer } from 'rxjs';
import { flatMap, switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Video } from './models/video';
import { VIDEOS } from './mock-videos';

@Injectable({
  providedIn: 'root',
})
export class VideoService {
  // private videoURL = 'http://localhost:3000/videos';
  private videoURL = 'http://localhost:3000/videos';

  constructor(private http: HttpClient) {}

  getVideos(): Observable<Video[]> {
    return timer(0, 15000).pipe(
      switchMap(() => {
        return this.http.get<Video[]>(this.videoURL);
      })
    );
  }

  convertToFormat(formData): Observable<Video> {
    return this.http.post<Video>(this.videoURL, formData);
  }

  getVideoById(id): Observable<Video> {
    return this.http.get<Video>(`${this.videoURL}/${id}`);
  }
}
