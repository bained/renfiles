# renfiles - rename files command line tool for Windows

This is a simple tool that can help you rename multiple files with a command line pipe. It can also be used to rename a single file. The code used is not particularly optimized, but I am still learning to write in C#. The code can be compiled on any Windows that has the .NET Framework installed.


## How to compile

Just open a Windows command prompt and specify the path to your **csc.exe**. For example:
```batch
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:exe renfiles.cs
```

## Usage

### Parameters
-h, --help &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  Print this help   
-s, --search &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Search string   
-r, --replace &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Replace string  

### Optional
-f, --file &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Path to single file

### Sample commands
```batch
dir /b /s | renfiles -s "search_string" -r "replace_string"
renfiles -s "search_string" -r "replace_string" -f "path_to_file"
cat list_files.txt | renfiles -s "search_string" -r "replace_string"
```

See a short video on how to use the tool on my blog:
[blog.nediko.info](http://blog.nediko.info/renfiles-rename-files-command-line-tool-for-windows)
