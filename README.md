# Summary

The goal of this application is to provide some basic functionality for video conversion, as well as extracting thumbnails from the video.

# Requirements

The backend server leverages the [Xabe.ffmpeg](https://ffmpeg.xabe.net/) library.
The requirement is that the [ffmpeg](https://ffmpeg.org/download.html) tool must be installed, or the binary files must in the solution of the project

# Technologies used

For the backend server: - .NET Core 3.1 - Microsoft SQL Database
For the frontend application: - Angular 10, scaffolded using @angular-cli

# Workflows

    - Upload a video
    - Convert that video to another format: Hd480, Hd720, Hd1080 using the h264 codec
    - Generate the HLS VOD using the options for formats as above
    - Generate thumbnails of the parent video(at the moment it extracts from second 1 and 3 of the parent video)

# Consideration and limitations

This project servers as demo purposes and some of the things are implemented accordingly:

    - local queue that processes requests based on the order they arrive, since video editing is an expensive operation and thus risking the connection to time out
    - files are saved on disk in detriment of db
    - the FE app calls the server every 15 seconds to get the updated list of videos. Could be improved in the future with websockets
