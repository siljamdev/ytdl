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
			
			try{
				ch = new ConsoleHandler(false);
			}catch(InvalidOperationException e){}
			
			vid(args[1], true);
			return 0;
			
			case "-P":
			case "--PLS":
			if(args.Length != 2){
				Console.Error.WriteLine(new FormatString("Expected playlist url", error));
				return 1;
			}
			
			try{
				ch = new ConsoleHandler(false);
			}catch(InvalidOperationException e){}
			
			pls(args[1], true);
			return 0;
			
			case "-U":
			case "--UPDATE":
			
			try{
				ch = new ConsoleHandler(false);
			}catch(InvalidOperationException e){}
			
			downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", "yt-dlp.exe").Wait();
			return 0;
			
			default:
			Console.Error.WriteLine(new FormatString("Unknown option: " + args[0], error));
			return 1;
		}
	}
	
	static void cliHelp(){
		Console.WriteLine(new FormatString("Youtube Downloader CLI help", info));
		Console.WriteLine("This application creates a folder in the executing directory called 'out', that is where all downloaded videos will go");
		Console.WriteLine("mp3 format is always used, and it needs yt-dlp.exe to work");
		Console.WriteLine("");
		Console.WriteLine(new FormatString("Options", info));
		Console.WriteLine(" --help");
		Console.WriteLine(" -h");
		Console.WriteLine(new FormatString("\tDisplays help", info));
		Console.WriteLine("");
		Console.WriteLine(" --vid <video url>");
		Console.WriteLine(" -v <video url>");
		Console.WriteLine(new FormatString("\tProcesses a single video", info));
		Console.WriteLine("");
		Console.WriteLine(" --pls <playlist url>");
		Console.WriteLine(" -p <playlist url>");
		Console.WriteLine(new FormatString("\tProcesses a playlist (multiple videos)", info));
		Console.WriteLine("");
		Console.WriteLine(" --update");
		Console.WriteLine(" -u");
		Console.WriteLine(new FormatString("\tUpdates yt-dlp", info));
	}
}