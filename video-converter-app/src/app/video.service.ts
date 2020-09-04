import { Injectable } from '@angular/core';
import { Observable, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Video } from './models/video';
import { OutputFile } from './models/outputFile';

@Injectable({
  providedIn: 'root',
})
export class VideoService {
  // private videoURL = 'http://localhost:3000/videos';
  private videoURL = 'http://localhost:5010/api/videos';

  constructor(private http: HttpClient) {}

  getVideos(): Observable<Video[]> {
    return timer(0, 15000).pipe(
      switchMap(() => {
        return this.http.get<Video[]>(this.videoURL);
      })
    );
  }

  getVideoById(id): Observable<Video> {
    return this.http.get<Video>(`${this.videoURL}/${id}`);
  }

  convertToFormat(formData): Observable<Video> {
    return this.http.post<Video>(this.videoURL, formData);
  }

  convertToFormatById(body): Observable<Video> {
    return this.http.post<Video>(`${this.videoURL}/convert`, body);
  }

  generateHLS(hslData): Observable<Video> {
    return this.http.post<Video>(`${this.videoURL}/hls`, hslData);
  }

  getThumbnails(thumbnailsData): Observable<Video> {
    return this.http.post<Video>(`${this.videoURL}/thumbnails`, thumbnailsData);
  }

  downloadFile(filename): Observable<any> {
    return this.http.get(
      `${this.videoURL}/download/${encodeURIComponent(filename)}`,
      { responseType: 'blob' }
    );
  }
}
