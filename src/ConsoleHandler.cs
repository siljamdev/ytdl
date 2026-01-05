using System;
using System.Text;
using AshLib.Formatting;

class ConsoleHandler{
	FormatString prompt; //For example $>
	int promptY;
	
	int promptFinishX;
	int promptFinishY;
	
	FormatString input; //Actual user input
	int cursor; //Location in input. 0 is end(default), 1 is one to the left...
	
	bool writing;
	
	readonly object _lock = new object();
	
	public ConsoleHandler(bool b = true){
		if(b && !isConsoleInteractive()){
			throw new InvalidOperationException("The console needs to be interactive");
		}
	}
	
	public void WriteLine(string s, CharFormat f){
		WriteLine(new FormatString(s, f));
	}
	
	public void WriteLine(FormatString f){
		lock(_lock){
			if(writing){
				hideCursor();
				setCursorPosition(0, promptY);
				clearAllBelowCursor();
			}
			
			Console.WriteLine(f);
			
			if(writing){
				promptY = Console.CursorTop;
				Console.Write(prompt);
				promptFinishY = Console.CursorTop;
				Console.Write(input);
				fixCursor();
			}
		}
	}
	
	public string ReadLine(string pro, CharFormat proFormat){
		return ReadLine(new FormatString(pro, proFormat));
	}
	
	public string ReadLine(FormatString pro){
		return ReadLine(pro, CharFormat.ResetAll);
	}
	
	public string ReadLine(string pro, CharFormat proFormat, CharFormat inputFormat){
		return ReadLine(new FormatString(pro, proFormat), inputFormat);
	}
	
	public string ReadLine(FormatString pro, CharFormat inputFormat){
		while(writing){
			Thread.Sleep(1);
		}
		
		lock(_lock){
			writing = true;
			
			prompt = pro;
			promptY = Console.CursorTop;
			
			input = new FormatString();
			cursor = 0;
			
			Console.Write(prompt);
			
			promptFinishX = Console.CursorLeft;
			promptFinishY = Console.CursorTop;
		}
		
		while(true){
			ConsoleKeyInfo key = Console.ReadKey(intercept: true);
			
			lock(_lock){
				switch(key.Key){
					case ConsoleKey.Enter:{
						writing = false;
						
						Console.WriteLine();
						return input.content;
					}break;
					
					case ConsoleKey.Backspace:{
						if(input.Length > 0){
							if(cursor == 0){
								input.RemoveFromEnd(1);
								printInput();
							}else if(cursor != input.Length){
								input.RemoveRange(input.Length - 1 - cursor, 1);
								printInput();
							}
						}
					}break;
					
					case ConsoleKey.Delete:{
						if(cursor != 0){
							input.RemoveRange(input.Length - cursor, 1);
							cursor--;
							printInput();
						}
					}break;
					
					case ConsoleKey.Clear:
					case ConsoleKey.Escape:{
						input.Clear();
						cursor = 0;
						printInput();
					}break;
					
					case ConsoleKey.Home:{
						cursor = input.Length;
						fixCursor();
					}break;
					
					case ConsoleKey.End:{
						cursor = 0;
						fixCursor();
					}break;
					
					case ConsoleKey.LeftArrow:{
						if(cursor < input.Length){
							cursor++;
							fixCursor();
						}
					}break;
					
					case ConsoleKey.RightArrow:{
						if(cursor > 0){
							cursor--;
							fixCursor();
						}
					}break;
					
					default:{
						if(!char.IsControl(key.KeyChar)){
							if(cursor == 0){
								input.Append(key.KeyChar, inputFormat);
								
								Console.Write(new FormatString(key.KeyChar.ToString(), inputFormat));
								
								fixCursor();
							}else{
								FormatString n = new FormatString(key.KeyChar.ToString(), inputFormat) + input.Substring(input.Length - cursor, cursor);
								input = input.Substring(0, input.Length - cursor) + n;
								
								printInput();
							}
						}
					}break;
				}
			}
		}
	}
	
	void printInput(){
		hideCursor();
		
		setCursorPosition(promptFinishX, promptFinishY);
		
		Console.Write(input);
		
		clearAllBelowCursor();
		
		fixCursor();
	}
	
	void fixCursor(){
		int totalChars = promptFinishX + input.Length - cursor;
		int absoluteY = promptFinishY + totalChars / Console.BufferWidth;
		int absoluteX = totalChars % Console.BufferWidth;
		
		setCursorPosition(absoluteX, absoluteY);
		
		showCursor();
	}
	
	string clear(FormatString fsog){
		FormatString[] fsl = fsog.SplitIntoLines();
		StringBuilder sb = new();
		
		for(int i = 0; i < fsl.Length - 1; i++){
			sb.AppendLine(new string(' ', fsl[i].Length));
		}
		
		if(fsl.Length > 0){
			sb.Append(new string(' ', fsl[^1].Length));
		}
		
		return sb.ToString();
	}
	
	static void setCursorPosition(int x, int y){
		Console.SetCursorPosition(x, y);
		//Console.Write("\x1b[" + (y + 1) + ";" + (x + 1) + "H"); //This single line for some reason doesnt work and breaks everything
	}
	
	static void clearAllBelowCursor(){
		Console.Write("\x1b[0J");
	}
	
	static void hideCursor(){
		Console.Write("\x1b[?25l");
	}
	
	static void showCursor(){
		Console.Write("\x1b[?25h");
	}
	
	static bool isConsoleInteractive(){
		return Environment.UserInteractive && !Console.IsInputRedirected && !Console.IsOutputRedirected;
	}
}