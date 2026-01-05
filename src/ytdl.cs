using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Compression;
using AshLib;
using AshLib.Formatting;

partial class YTDownloader{
	static CharFormat error = new CharFormat(new Color3("E74856"));
	static CharFormat prompt = new CharFormat(new Color3("13A10E"));
	static CharFormat input = new CharFormat(new Color3("FF9B10"));
	static CharFormat info = new CharFormat(new Color3("11A8CD"));
	
	static Queue<CharFormat> pool = new();
	
	static ConsoleHandler ch;
	
	static FormatString promptVid = new FormatString(("Enter video url", prompt), (" > ", CharFormat.ResetAll));
	static FormatString promptPls = new FormatString(("Enter playlist url", prompt), (" > ", CharFormat.ResetAll));
	
	static string exePath;
	static string ytdlpPath;
	
	static int Main(string[] args){
		//Setup paths
		exePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        ytdlpPath = getYtdlpPath();
		
		//Also handles NO_COLOR
		FormatString.usesColors = !Console.IsOutputRedirected && FormatString.usesColors;
		
		initPool();
		
		Directory.CreateDirectory("out");
		
		if(args.Length > 0){
			return cli(args);
		}
		
		try{
			ch = new ConsoleHandler();
		}catch(Exception e){
			Console.WriteLine(new FormatString(e.ToString(), error));
			return 3;
		}
		
		if(!File.Exists(ytdlpPath)){
			downloadYtdlp();
		}
		
		FormatString promptStr = new FormatString(("Enter instruction", prompt), (" > ", CharFormat.ResetAll));
		
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
				{
					string videoUrl = ch.ReadLine(promptVid, input);
					vid(videoUrl);
				}break;
				
				case "AVID":
				case "AV":
				case "A":
				{
					string videoUrl = ch.ReadLine(promptVid, input);
					vidAudio(videoUrl);
				}break;
				
				case "PLS":
				case "P":
				{
					string playlistUrl = ch.ReadLine(promptPls, input);
					pls(playlistUrl);
				}break;
				
				case "APLS":
				case "AP":
				{
					string playlistUrl = ch.ReadLine(promptPls, input);
					plsAudio(playlistUrl);
				}break;
				
				case "RAP":
				case "R":
				{
					ch.WriteLine("You are now in rapid mode. Type 'exit' to exit", info);
					
					string videoUrl = ch.ReadLine(promptVid, input);
					
					while(videoUrl.Trim().ToUpper() != "X" && videoUrl.Trim().ToUpper() != "EXIT"){
						vid(videoUrl);
						videoUrl = ch.ReadLine(promptVid, input);
					}
				}break;
				
				case "ARAP":
				case "AR":
				{
					ch.WriteLine("You are now in rapid mode. Type 'exit' to exit", info);
					
					string videoUrl = ch.ReadLine(promptVid, input);
					
					while(videoUrl.Trim().ToUpper() != "X" && videoUrl.Trim().ToUpper() != "EXIT"){
						vidAudio(videoUrl);
						videoUrl = ch.ReadLine(promptVid, input);
					}
				}break;
				
				case "DOWNLOAD":
				case "D":
					downloadYtdlp();
				break;
				
				case "UPDATE":
				case "U":
					updateYtdlp();
				break;
				
				case "HELP":
				case "H":
					help();
				break;
			}
		}
	}
	
	static void vid(string videoUrl, bool wait = false){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp is needed", error);
			return;
		}
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("vid", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Video downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(ytdlpPath,
			"-f \"bv*[vcodec^=avc1]+ba[acodec^=mp4a]/b\" --merge-output-format mp4 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --no-playlist \"" + videoUrl + "\"",
			ind, f, wait);
	}
	
	static void vidAudio(string videoUrl, bool wait = false){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp is needed", error);
			return;
		}
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("avid", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Video audio downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(ytdlpPath,
			"-x --audio-format mp3 --audio-quality 0 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --no-playlist \"" + videoUrl + "\"",
			ind, f, wait);
	}
	
	static void pls(string videoUrl, bool wait = false){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp is needed", error);
			return;
		}
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("pls", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Playlist downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(ytdlpPath,
			"-f \"bv*[vcodec^=avc1]+ba[acodec^=mp4a]/b\" --merge-output-format mp4 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --yes-playlist -i --user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64)\" \"" + videoUrl + "\"",
			ind, f, wait);
	}
	
	static void plsAudio(string videoUrl, bool wait = false){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp is needed", error);
			return;
		}
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("apls", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Playlist audio downloading", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(ytdlpPath,
			"-x --audio-format mp3 --audio-quality 0 -o \"" + Environment.CurrentDirectory + "/out/%(title)s.%(ext)s\" --no-mtime --yes-playlist -i --user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64)\" \"" + videoUrl + "\"",
			ind, f, wait);
	}
	
	static void help(){
		ch.WriteLine("Youtube Downloader help", info);
		ch.WriteLine("This application creates a folder in the executing directory called 'out', that is where all downloaded videos will go");
		ch.WriteLine("mp4/mp3 format is always used, and it needs yt-dlp to work");
		ch.WriteLine("");
		ch.WriteLine("Instructions:", info);
		ch.WriteLine(" help", input);
		ch.WriteLine(" h", input);
		ch.WriteLine("\tDisplay help");
		ch.WriteLine("");
		ch.WriteLine(" vid", input);
		ch.WriteLine(" v", input);
		ch.WriteLine("\tDownload a single video as mp4");
		ch.WriteLine("");
		ch.WriteLine(" avid", input);
		ch.WriteLine(" av", input);
		ch.WriteLine(" a", input);
		ch.WriteLine("\tDownload a single video as mp3");
		ch.WriteLine("");
		ch.WriteLine(" pls", input);
		ch.WriteLine(" p", input);
		ch.WriteLine("\tDownload a whole playlist (multiple videos) as mp4");
		ch.WriteLine("");
		ch.WriteLine(" apls", input);
		ch.WriteLine(" ap", input);
		ch.WriteLine("\tDownloads a whole playlist (multiple videos) as mp3");
		ch.WriteLine("");
		ch.WriteLine(" rap", input);
		ch.WriteLine(" r", input);
		ch.WriteLine("\tEnter rapid mode (download multiple videos) as mp4");
		ch.WriteLine("");
		ch.WriteLine(" arap", input);
		ch.WriteLine(" ar", input);
		ch.WriteLine("\tEnter rapid mode (download multiple videos) as mp3");
		ch.WriteLine("");
		ch.WriteLine(" update", input);
		ch.WriteLine(" u", input);
		ch.WriteLine("\tUpdates existing yt-dlp");
		ch.WriteLine("");
		ch.WriteLine(" download", input);
		ch.WriteLine(" d", input);
		ch.WriteLine("\tDownload yt-dlp");
		ch.WriteLine("");
		ch.WriteLine(" exit", input);
		ch.WriteLine(" x", input);
		ch.WriteLine("\tExits program");
	}
	
	static void initPool(){
		pool.Enqueue(new CharFormat(new Color3("3030FF")));
		pool.Enqueue(new CharFormat(new Color3("FF30FF")));
		pool.Enqueue(new CharFormat(new Color3("30FFFF")));
		pool.Enqueue(new CharFormat(new Color3("FFFF30")));
		pool.Enqueue(new CharFormat(new Color3("FF3090")));
		pool.Enqueue(new CharFormat(new Color3("10FF30")));
		pool.Enqueue(new CharFormat(new Color3("FF6030")));
		pool.Enqueue(new CharFormat(new Color3("9933FF")));
		pool.Enqueue(new CharFormat(new Color3("98BF93")));
		pool.Enqueue(new CharFormat(new Color3("FF88DD")));
		pool.Enqueue(new CharFormat(new Color3("00FF66")));
		pool.Enqueue(new CharFormat(new Color3("95C7D8")));
		pool.Enqueue(new CharFormat(new Color3("B5BA72")));
		pool.Enqueue(new CharFormat(new Color3("E26E58")));
		pool.Enqueue(new CharFormat(new Color3("D88508")));
		pool.Enqueue(new CharFormat(new Color3("7DC1B4")));
		pool.Enqueue(new CharFormat(new Color3("CC33D1")));
	}
	
	//Yt-Dlp stuff
	static string getYtdlpPath(){
		if(OperatingSystem.IsWindows()){
			return exePath + "/yt-dlp.exe";
		}else if(OperatingSystem.IsLinux()){
			return exePath + "/yt-dlp.exe";
		}else if(OperatingSystem.IsMacOS()){
			return exePath + "/yt-dlp.exe";
		}else{
			return null;
		}
	}
	
	static void updateYtdlp(){
		if(!File.Exists(ytdlpPath)){
			ch.WriteLine("yt-dlp is needed", error);
			return;
		}
		
		CharFormat? f = pool.Dequeue();
		FormatString ind = new FormatString();
		ind.Append("[");
		ind.Append("upd", f);
		ind.Append("] ", CharFormat.ResetAll);
		
		ch.WriteLine("Updating yt-dlp", f);
		ch.WriteLine("");
		
		// Start the process
		runProcess(ytdlpPath, "--update", ind, f, true);
	}
	
	static void downloadYtdlp(bool wait = false){
		try{
			if(OperatingSystem.IsWindows()){
				Task t = downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", ytdlpPath, null);
				
				if(wait){
					t.Wait();
				}
			}else if(OperatingSystem.IsLinux()){
				string arch;
				bool downloadZip = false;
				bool isMusl = File.Exists("/lib/ld-musl-x86_64.so.1") || File.Exists("/lib/ld-musl-aarch64.so.1");
				
				switch(RuntimeInformation.OSArchitecture){
					case Architecture.X64:
						arch = isMusl ? "yt-dlp_musllinux" : "yt-dlp_linux";
						break;
					
					case Architecture.Arm64:
						arch = isMusl ? "yt-dlp_musllinux_aarch64" : "yt-dlp_linux_aarch64";
						break;
					
					case Architecture.Arm:
						// Only unpackaged armv7l exists
						arch = "yt-dlp_linux_armv7l.zip";
						downloadZip = true;
						break;
					
					default:
						ch.WriteLine("Unknown architecture", error);
						return;
				}
				
				if(downloadZip){
					Task t = downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/" + arch, exePath + "/temp.zip", async () => {
						try{
							ZipFile.ExtractToDirectory(exePath + "/temp.zip", exePath + "/temp", true);
							
							string p = Directory.GetFiles(exePath + "/temp", "yt-dlp_linux_armv7l", SearchOption.AllDirectories).FirstOrDefault();
							File.Copy(p, ytdlpPath, true);
							
							Directory.Delete(exePath + "/temp", true);
							File.Delete(exePath + "/temp.zip");
						}catch(Exception e){
							ch.WriteLine(e.ToString(), error);
						}
						
						try{
							Process.Start("chmod", "+x \"" + ytdlpPath + "\"");
						}catch(Exception e){
							ch.WriteLine(e.ToString(), error);
						}
					});
					
					if(wait){
						t.Wait();
					}
				}else{
					Task t = downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/" + arch,
					ytdlpPath, async () => {
						try{
							Process.Start("chmod", "+x \"" + ytdlpPath + "\"");
						}catch(Exception e){
							ch.WriteLine(e.ToString(), error);
						}
					});
					
					if(wait){
						t.Wait();
					}
				}
			}else if(OperatingSystem.IsMacOS()){
				Task t = downloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_macos",
				ytdlpPath, async () => {
					try{
						Process.Start("chmod", "+x \"" + ytdlpPath + "\"");
					}catch(Exception e){
						ch.WriteLine(e.ToString(), error);
					}
				});
				
				if(wait){
					t.Wait();
				}
			}
		}catch(Exception e){
			ch.WriteLine(e.ToString(), error);
			ch.WriteLine("yt-dlp failed to download", error);
		}
	}
	
	//Helpers
	static void runProcess(string name, string args, FormatString ind, CharFormat? f, bool wait){
		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = name,
			Arguments = args,
			
			//UseShellExecute = false,
			CreateNoWindow = true,
			
			RedirectStandardOutput = true,
			RedirectStandardError  = true
		};
		
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
		
		int exitCode = 0;
		
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
		};
	}
	
	static async Task downloadFile(string url, string outputPath, Func<Task> onComplete){
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
			if(onComplete != null){
				await onComplete();
			}
		}catch(Exception e){
			ch.WriteLine(e.ToString(), error);
			ch.WriteLine("File could not be downloaded", error);
		}
	}
}
