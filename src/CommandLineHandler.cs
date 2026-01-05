using AshLib.Formatting;

partial class YTDownloader{
	static int cli(string[] args){
		args[0] = args[0].ToUpper();
		switch(args[0]){
			case "-H":
			case "--HELP":
			cliHelp();
			return 0;
			
			case "-V":
			case "--VID":
			if(args.Length != 2){
				Console.Error.WriteLine(new FormatString("Expected video url", error));
				return 1;
			}
			
			ch = new ConsoleHandler(false);
			
			vid(args[1], true);
			return 0;
			
			case "-A":
			case "-AV":
			case "--AVID":
			if(args.Length != 2){
				Console.Error.WriteLine(new FormatString("Expected video url", error));
				return 1;
			}
			
			ch = new ConsoleHandler(false);
			
			vidAudio(args[1], true);
			return 0;
			
			case "-P":
			case "--PLS":
			if(args.Length != 2){
				Console.Error.WriteLine(new FormatString("Expected playlist url", error));
				return 1;
			}
			
			ch = new ConsoleHandler(false);
			
			pls(args[1], true);
			return 0;
			
			case "-AP":
			case "--APLS":
			if(args.Length != 2){
				Console.Error.WriteLine(new FormatString("Expected playlist url", error));
				return 1;
			}
			
			ch = new ConsoleHandler(false);
			
			plsAudio(args[1], true);
			return 0;
			
			case "-U":
			case "--UPDATE":
			
			ch = new ConsoleHandler(false);
			
			updateYtdlp();
			return 0;
			
			case "-D":
			case "--DOWNLOAD":
			
			ch = new ConsoleHandler(false);
			
			downloadYtdlp(true);
			return 0;
			
			default:
			Console.Error.WriteLine(new FormatString("Unknown flag: " + args[0], error));
			return 1;
		}
	}
	
	static void cliHelp(){
		Console.WriteLine(new FormatString("Youtube Downloader CLI help", info));
		Console.WriteLine("This application creates a folder in the executing directory called 'out', that is where all downloaded videos will go");
		Console.WriteLine("mp4/mp3 format is always used, and it needs yt-dlp to work");
		Console.WriteLine("");
		Console.WriteLine(new FormatString("Flags", info));
		Console.WriteLine(" --help");
		Console.WriteLine(" -h");
		Console.WriteLine(new FormatString("\tDisplay help", info));
		Console.WriteLine("");
		Console.WriteLine(" --vid <video url>");
		Console.WriteLine(" -v <video url>");
		Console.WriteLine(new FormatString("\tDownload a single video as mp4", info));
		Console.WriteLine("");
		Console.WriteLine(" --avid <video url>");
		Console.WriteLine(" -av <video url>");
		Console.WriteLine(" -a <video url>");
		Console.WriteLine(new FormatString("\tDownload a single video as mp3", info));
		Console.WriteLine("");
		Console.WriteLine(" --pls <playlist url>");
		Console.WriteLine(" -p <playlist url>");
		Console.WriteLine(new FormatString("\tDownload a playlist (multiple videos) as mp4", info));
		Console.WriteLine("");
		Console.WriteLine(" --apls <playlist url>");
		Console.WriteLine(" -ap <playlist url>");
		Console.WriteLine(new FormatString("\tDownload a playlist (multiple videos) as mp3", info));
		Console.WriteLine("");
		Console.WriteLine(" --update");
		Console.WriteLine(" -u");
		Console.WriteLine(new FormatString("\tUpdate existing yt-dlp", info));
		Console.WriteLine("");
		Console.WriteLine(" --download");
		Console.WriteLine(" -d");
		Console.WriteLine(new FormatString("\tDownload yt-dlp", info));
	}
}