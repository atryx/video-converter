import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Video } from '../models/video';
import { VIDEOS } from '../mock-videos';

@Component({
  selector: 'app-videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css'],
})
export class VideosComponent implements OnInit {
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
  dataSource = new MatTableDataSource<Video>(VIDEOS);

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor() {}

  ngOnInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  onSelect(video: Video): void {
    console.log(video);
  }
}
