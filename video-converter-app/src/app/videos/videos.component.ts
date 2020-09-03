import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { VideoService } from '../video.service';
import { Video } from '../models/video';

@Component({
  selector: 'app-videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css'],
})
export class VideosComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  displayedColumns: string[] = [
    'actions',
    'name',
    'duration',
    'bitrate',
    'codec',
    'resolution',
    'availableResolutions',
    'thumbnails',
    'delete',
  ];
  dataSource = new MatTableDataSource<Video>();
  isLoading = true;

  constructor(public dialog: MatDialog, private videoService: VideoService) {}

  ngOnInit(): void {
    this.dataSource.paginator = this.paginator;
    this.getVideos();
  }

  getVideos(): void {
    this.videoService.getVideos().subscribe(
      (videos) => {
        this.isLoading = false;
        this.dataSource.data = videos;
      },
      (error) => {
        console.log(error);
        this.isLoading = false;
      }
    );
  }

  onSelect(id: number): void {
    console.log('on select clicked', id);
  }

  openDialog(id: number) {
    const dialogRef = this.dialog.open(DialogContentExampleDialog);

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.event) console.log('delete video');
      else console.log('do nothing');
    });
  }
}

@Component({
  selector: 'dialog-content-example-dialog',
  templateUrl: 'dialog-content-example-dialog.html',
})
export class DialogContentExampleDialog {
  constructor(public dialogRef: MatDialogRef<DialogContentExampleDialog>) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
  onConfirmClick(): void {
    this.dialogRef.close({ event: true });
  }
}
