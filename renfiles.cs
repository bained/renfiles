using System;
using System.Runtime.InteropServices;
using System.IO; // StreamReader
using System.Collections; // ArrayList
using System.Collections.Generic; // List
using System.Linq; // .ToList();

namespace ren_files
{
	public class CommandLineParser {
		private readonly List < string > _args;
		public CommandLineParser(string[] args) {
			_args = args.ToList();
		}
		public string GetStringArgument(string key, char shortKey) {
			var index = _args.IndexOf("--" + key);
			if (index >= 0 && _args.Count > index) {
			return _args[index + 1];
			}
			index = _args.IndexOf("-" + shortKey);
			if (index >= 0 && _args.Count > index) {
			return _args[index + 1];
			}
			return null;
		}
		public bool GetSwitchArgument(string value, char shortKey) {
			return _args.Contains("--" + value) || _args.Contains("-" + shortKey);
		}
	}

	public static class ConsoleEx {
		public static bool IsOutputRedirected {
			get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout)); }
		}
		public static bool IsInputRedirected {
			get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin)); }
		}
		public static bool IsErrorRedirected {
			get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr)); }
		}
	
		// P/Invoke:
		private enum FileType { Unknown, Disk, Char, Pipe };
		private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
		[DllImport("kernel32.dll")]
		private static extern FileType GetFileType(IntPtr hdl);
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(StdHandle std);
	}
	
	public static class Helper {
		
		public static string detectFile(string f){
			// Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.InputEncoding = System.Text.Encoding.UTF8;
			
			if(File.Exists(f)) {
				return "file";
			}
			else if(Directory.Exists(f)) {
			   return "dir";
			}
			else {
			   return "invalid";
			}
		}
		
		public static bool file_exists(string f){
			if(!System.IO.File.Exists(f)){
				return false;
			}
			return true;
		}
		public static string fnamePath(string f){
			FileInfo fileInfo = new FileInfo(f);
			string directoryFullPath = fileInfo.DirectoryName;
			string fname = fileInfo.Name;
			return directoryFullPath + "|" + fname;
		}
		
		public static string ren_filename(string f, string search, string replace){
			string new_str = f.Replace(search, replace);
			return new_str;
		}
		
		public static void usage(){
			Console.WriteLine("--=[ HELP ]=--\n");
			Console.WriteLine("Parameters:\n");
			Console.WriteLine("-h, --help		Print this help");
			Console.WriteLine("-s, --search		Search string");
			Console.WriteLine("-r, --replace		Replace string\n\n");
			Console.WriteLine("Usage examples:\n");
			Console.WriteLine("dir /b /s | renfiles -s \"search_string\" -r \"replace_string\" ");
			Console.WriteLine("renfiles -s \"search_string\" -r \"replace_string\" -f \"path_to_file\" \n");
			return;
		}

		public static void print_error(string errormsg){
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(errormsg);
			Console.ResetColor();
		}
	}

	// ========================================
	
	class Program
	{
		public static void Main(string[] args)
		{
			var argparser = new CommandLineParser(args);
	   		var tsearch = argparser.GetStringArgument("search", 's');
			var treplace = argparser.GetStringArgument("replace", 'r');
			var targetfile = argparser.GetStringArgument("file", 'f');
			var thelp = argparser.GetSwitchArgument("help", 'h');
			
			if(thelp) {
				Helper.usage();
				return;
			}

			if(String.IsNullOrEmpty(tsearch) || String.IsNullOrEmpty(treplace)){
				// Console.WriteLine("ERROR: Missing parameters!\n");
				Helper.print_error("ERROR: Missing parameters!\n");
				Helper.usage();
				return;
			}

			bool inputRedirected = ConsoleEx.IsInputRedirected;
			
			if (inputRedirected) {

				if(!String.IsNullOrEmpty(targetfile)){
					// Console.WriteLine("ERROR: Redirected output combined with target file!\n");
					Helper.print_error("ERROR: Redirected output combined with target file!\n");
					Helper.usage();
					return;
				}

				var arrFls = new ArrayList();
				using (var sr = new StreamReader(Console.OpenStandardInput(8192),System.Text.Encoding.UTF8)) {
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						if( Helper.detectFile(line) == "file" ){
							arrFls.Add(line);
						}
					}
				}

				if(arrFls.Count > 0){
					
					foreach (string f in arrFls) {
						if (!Helper.file_exists(f)){
							// Console.WriteLine("ERROR: File " + f + " doesn\'t exists!\n");
							Helper.print_error("ERROR: File " + f + " doesn\'t exists!\n");
							continue;
						}
						string fnameAndPath = Helper.fnamePath(f);
						string[] fnap = fnameAndPath.Split('|');
						string fname = fnap[1];
						string fdir = fnap[0];
						string trgtfile = fdir + "\\" + fname;
						string new_fname = Helper.ren_filename(fname, tsearch, treplace);
						string new_fpath = fdir + "\\" + new_fname;

						try {
							File.Move(trgtfile, new_fpath);
						}
						catch (Exception e) {
							Console.WriteLine(trgtfile);
							Console.WriteLine("The process failed: {0}", e.ToString());
						}
					}
				}

			} else {
				if(String.IsNullOrEmpty(targetfile)){
					// Console.WriteLine("ERROR: Missing target file!\n");
					Helper.print_error("ERROR: Missing target file!\n");
					Helper.usage();
					return;
				}

				if (!Helper.file_exists(targetfile)){
					// Console.WriteLine("ERROR: Missing target file!\n");
					Helper.print_error("ERROR: Missing target file!\n");
					Helper.usage();
					return;
				}
				string fnameAndPath = Helper.fnamePath(targetfile);
				string[] fnap = fnameAndPath.Split('|');
				string fname = fnap[1];
				string fdir = fnap[0];
				string new_fname = Helper.ren_filename(fname, tsearch, treplace);
				string new_fpath = fdir + "\\" + new_fname;

				try {
					File.Move(targetfile, new_fpath);
				}
				catch (Exception e) {
					Console.WriteLine(targetfile);
					Console.WriteLine("The process failed: {0}", e.ToString());
				}
			}

		}
	}
}