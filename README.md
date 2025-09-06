# Youtube Downloader
<img src="res/icon.png" width="200"/>
Easy asynchronous youtube MP3 downloader

## Usage
This application acts as a wrapper over [yt-dlp](https://github.com/yt-dlp/yt-dlp). It lets you download the audio of multiple videos and playlists at the same time in a MP3 format.  
The output MP3 files are generated on a folder on the executing path of the application called `out`.  
Their names are the same that they had on youtube.  
If you open the application normally, you will have access to an interactive console. If you pass CL args, you will only execute one operation.  
**Note**: [yt-dlp](https://github.com/yt-dlp/yt-dlp) needs [ffmpeg](https://ffmpeg.org/) to work. You may have to install it as well.  

## CLI
Pass it the `-h` flag to get CLI help.  
Only one operation may be executed at a time.  
Example:
```ytdl -v https://www.youtube.com/watch?v=z9MovmqnDFA```  
Downloads the audio of a video

## Installation
This application is only available for Windows, because of the way it downloads the `yt-dlp` executable.  
Download an executable from the [Releases](https://github.com/siljamdev/ytdl/releases/latest).

## License
This software is licensed under the [MIT License](https://github.com/siljamdev/ytdl/blob/main/LICENSE).

## Internal operation
Uses [AshLib](https://github.com/siljamdev/AshLib) for its [FormatStrings](https://github.com/siljamdev/AshLib/blob/main/documentation/formatstrings.md).  
It supports NO_COLOR environment variable(because of AshLib). Read more [here](https://no-color.org/).