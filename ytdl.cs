using System.Diagnostics;
using AshLib;
using AshLib.Formatting;

partial class YTDownloader{
	static CharFormat error = new CharFormat(new Color3("E74856"));
	static CharFormat prompt = new CharFormat(new Color3("13A10E"));
	static CharFormat input = new CharFormat(new Color3("FF9B10"));
	static CharFormat info = new CharFormat(new Color3("11A8CD"));
	
	static Queue<CharFormat> pool = new();
	
	static ConsoleHandler ch;
	
	static FormatString promptVid = build(("Enter video url", prompt), (" > ", CharFormat.ResetAll));
	static FormatString promptPls = build(("Enter playlist url", prompt), (" > ", CharFormat.ResetAll));
	
	static string ytdlpPath;
	
	static int Main(string[] args){
		if(!OperatingSystem.IsWindows()){
			ch.WriteLine("Only Windows is supported", error);
			return 2;
		}
		
        ytdlpPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/yt-dlp.exe";
		
		//Also handles NO_COLOR
		FormatString.usesColors = !Console.IsOutputRedirected && FormatString.usesColors;
		
		initPool();
		
		Directory.CreateDirectory("out");
		
		if(args.Length > 0){
			return cli(args);
		}
		
		try{
			ch = new ConsoleHandler();
		}catch(InvalidOperationException e){
			Console.WriteLine(new FormatString(e.ToString(), error));
			return 3;
		}
		
		if(!File.Exists(ytdlpPath)){
			downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", ytdlpPath);
		}
		
		FormatString promptStr = build(("\nEnter instruction", prompt), (" > ", CharFormat.ResetAll));
		
		ch.WriteLine("Welcome to Youtube Downloader");
		
		while(true){
			string mode = ch.ReadLine(promptStr, input);
			
			switch(mode.Trim().ToUpper()){
				default:
					ch.WriteLine("Unknown instruction: " + mode, error);
				break;
				
				case "EXIT":
				case "X":
					return 0;
				break;
				
				case "VID":
				case "V":
				case "1":{
					string videoUrl = ch.ReadLine(promptVid, input);
					vid(videoUrl);
				}break;
				
				case "PLS":
				case "P":
				case "2":{
					string playlistUrl = ch.ReadLine(promptPls, input);
					pls(playlistUrl);
				}break;
				
				case "RAP":
				case "R":{
					ch.WriteLine("You are now in rapid mode. Type 'exit' to exit", info);
					
					string videoUrl = ch.ReadLine(promptVid, input);
					
					while(videoUrl.Trim().ToUpper() != "X" && videoUrl.Trim().ToUpper() != "EXIT"){
						vid(videoUrl);
						videoUrl = ch.ReadLine(promptVid, input);
					}
				}break;
				
				case "UPDATE":
				case "U":
					downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", "yt-dlp.exe");
				break;
				
				case "HELP":
				case "H":
					help();
				break;
			}
		}
	}
	
	static void initPool(){
		pool.Enqueue(new CharFormat(new Color3("3030FF")));
		pool.Enqueue(new CharFormat(new Color3("FF30FF")));
		pool.Enqueue(new CharFormat(new Color3("30FFFF")));
		pool.Enqueue(new CharFormat(new Color3("FFFF30")));
		pool.Enqueue(new CharFormat(new Color3("FF3090")));
		pool.Enqueue(new CharFormat(new Color3("10FF30")));
		pool.Enqueue(new CharFormat(new Color3("FF6030"))); //Orange
		pool.Enqueue(new CharFormat(new Color3("9933FF")));
		pool.Enqueue(new CharFormat(new Color3("98BF93")));
		pool.Enqueue(new CharFormat(new Color3("FF88DD"))); //Pink
		pool.Enqueue(new CharFormat(new Color3("00FF66")));
		pool.Enqueue(new CharFormat(new Color3("95C7D8"))); //Blue
		pool.Enqueue(new CharFormat(new Color3("B5BA72")));
		pool.Enqueue(new CharFormat(new Color3("E26E58")));
		pool.Enqueue(new CharFormat(new Color3("D88508")));
		pool.Enqueue(new CharFormat(new Color3("7DC1B4"))); //Cambridge blue
		pool.Enqueue(new CharFormat(new Color3("CC33D1")));
	}
	
	static void vid(string videoUrl, bool wait = false){
		if(!File.Exists("yt-dlp.exe")){
			ch.WriteLine("yt-dlp.exe is needed.", error);
			return;
		}
		
		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = ytdlpPath,
			Arguments = "-x --audio-format mp3 --audio-quality 0 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --no-playlist \"" + videoUrl + "\"",
			
			//UseShellExecute = false,
			CreateNoWindow = true,
			
			RedirectStandardOutput = true,
			RedirectStandardError  = true
		};
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("vid", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Video downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(psi, ind, f, wait);
	}
	
	static void pls(string videoUrl, bool wait = false){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp.exe is needed.", error);
			return;
		}
		
		var psi = new ProcessStartInfo{
			FileName = ytdlpPath,
			Arguments = "-x --audio-format mp3 --audio-quality 0 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --yes-playlist -i --user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64)\" \"" + videoUrl + "\"",
			
			//UseShellExecute = false,
			CreateNoWindow = true,
			
			RedirectStandardOutput = true,
			RedirectStandardError  = true
		};
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("pls", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Playlist downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(psi, ind, f, wait);
	}
	
	static void runProcess(ProcessStartInfo psi, FormatString ind, CharFormat? f, bool wait){
		Process process = new Process{StartInfo = psi, EnableRaisingEvents = true};
		process.OutputDataReceived += (sender, e) => {
			if(e.Data != null){
				ch.WriteLine(ind + e.Data);
			}
		};
		
		process.ErrorDataReceived += (sender, e) => {
			if(e.Data != null){
				ch.WriteLine(ind + new FormatString(e.Data, error));
			}
		};
		
		process.Exited += (sender, e) => {
			ch.WriteLine(ind + new FormatString("Exit code: " + process.ExitCode, info));
			pool.Enqueue(f);
			process.Dispose();
		};
		
		process.Start();
		
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		if(wait){
			process.WaitForExit();
		}
	}
	
	static void help(){
		ch.WriteLine("Youtube Downloader help", info);
		ch.WriteLine("This application creates a folder in the executing directory called 'out', that is where all downloaded videos will go");
		ch.WriteLine("mp3 format is always used, and it needs yt-dlp.exe to work");
		ch.WriteLine("");
		ch.WriteLine("Instructions:", info);
		ch.WriteLine(" help", input);
		ch.WriteLine(" h", input);
		ch.WriteLine("\tDisplays help");
		ch.WriteLine("");
		ch.WriteLine(" vid", input);
		ch.WriteLine(" v", input);
		ch.WriteLine(" 1", input);
		ch.WriteLine("\tProcesses a single video");
		ch.WriteLine("");
		ch.WriteLine(" pls", input);
		ch.WriteLine(" p", input);
		ch.WriteLine(" 2", input);
		ch.WriteLine("\tProcesses a playlist (multiple videos)");
		ch.WriteLine(" rap", input);
		ch.WriteLine(" r", input);
		ch.WriteLine("\tEnter rapid mode (multiple single videos)");
		ch.WriteLine("");
		ch.WriteLine(" update", input);
		ch.WriteLine(" u", input);
		ch.WriteLine("\tUpdates yt-dlp");
		ch.WriteLine("");
		ch.WriteLine(" exit", input);
		ch.WriteLine(" x", input);
		ch.WriteLine("\tExits program");
	}
	
	static FormatString build(params (string, CharFormat?)[] p){
		FormatString fs = new FormatString();
		
		foreach((string s, CharFormat? f) in p){
			fs.Append(s, f);
		}
		
		return fs;
	}
	
	static async Task downloadFile(string url, string outputPath){
		try{
			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			
			string dir = Path.GetDirectoryName(outputPath);
			if(!string.IsNullOrEmpty(dir)){
				Directory.CreateDirectory(dir);
			}
			
			await using(FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true)){
				await response.Content.CopyToAsync(fs);
				await fs.FlushAsync();
			}
			
			ch.WriteLine("File downloaded succesfully", info);
		}catch(Exception e){
			ch.WriteLine(e.ToString(), error);
			ch.WriteLine("File could not be downloaded", error);
		}
	}
}
