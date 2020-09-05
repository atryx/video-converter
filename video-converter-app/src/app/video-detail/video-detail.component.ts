import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VideoService } from '../video.service';
import { Video } from '../models/video';
import { OutputFormat } from '../models/outputFormat';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'app-video-detail',
  templateUrl: './video-detail.component.html',
  styleUrls: ['./video-detail.component.css'],
})
export class VideoDetailComponent implements OnInit {
  id;
  selectedValue: string;
  video: Video;
  isLoading = true;
  error: string;

  formats: OutputFormat[] = [
    { value: 'Hd480', viewValue: 'HD 480p' },
    { value: 'Hd720', viewValue: 'HD 720p' },
    { value: 'Hd1080', viewValue: 'HD 1080p' },
  ];
  constructor(
    private _Activatedroute: ActivatedRoute,
    private videoService: VideoService
  ) {}

  ngOnInit(): void {
    this._Activatedroute.paramMap.subscribe((params) => {
      this.id = params.get('id');
      this.getVideo(this.id);
    });
  }

  getVideo(id: number) {
    this.videoService.getVideoById(id).subscribe(
      (video) => {
        this.video = video;
        this.isLoading = false;
        console.log(this.video);
      },
      (error) => {
        this.isLoading = false;
        this.handleError(error);
      }
    );
  }

  generateHLS() {
    this.isLoading = true;
    this.videoService
      .generateHLS({ videoId: this.video.id, outputFormat: this.selectedValue })
      .subscribe(
        (video) => {
          this.video = video;
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
          this.handleError(error);
        }
      );
  }

  getThumbnails() {
    this.isLoading = true;
    this.videoService
      .getThumbnails({ videoId: this.video.id, timestampOfScreenshots: [1, 3] })
      .subscribe(
        (video) => {
          this.video = video;
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
          this.handleError(error);
        }
      );
  }

  convertToOutput() {
    this.isLoading = true;
    this.videoService
      .convertToFormatById({
        videoId: this.video.id,
        outputFormat: this.selectedValue,
      })
      .subscribe(
        (video) => {
          this.video = video;
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
          this.handleError(error);
        }
      );
  }

  download(fileLocation, fileName) {
    this.isLoading = true;
    const fullFilePath = fileLocation + '\\' + fileName;
    this.videoService.downloadFile(fullFilePath).subscribe((response) => {
      console.log(response);
      this.isLoading = false;

      let fileExtension = fileName.split('.').pop();
      let contentType =
        fileExtension == 'png' ? 'image/png' : `video/${fileExtension}`;
      let blob: any = new Blob([response], {
        type: contentType,
      });
      // const url = window.URL.createObjectURL(blob);
      fileSaver.saveAs(blob, fileName);
    }),
      (error) => {
        this.isLoading = false;
        this.handleError(error);
      },
      () => console.info('File downloaded successfully');
  }

  handleError(error) {
    if (error.status == 0 || error.status == 500) {
      this.error = 'Something went wrong. Please contact developer';
    } else {
      this.error = error.error;
    }
  }
}
