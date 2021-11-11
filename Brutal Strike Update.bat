Echo "Updating the game please wait..."
if not exist ./git/ (

if not exist %SystemRoot%\System32\curl.exe (
%SystemRoot%\system32\certutil.exe -urlcache -split -f https://github.com/git-for-windows/git/releases/download/v2.30.1.windows.1/PortableGit-2.30.1-32-bit.7z.exe gitSetup.exe
)
%SystemRoot%\System32\curl -L https://github.com/git-for-windows/git/releases/download/v2.30.1.windows.1/PortableGit-2.30.1-32-bit.7z.exe --output gitSetup.exe
gitSetup.exe -o ./git -y
)
rem call git\post-install.bat
rem cd %~dp0
rem mkdir BrutalStrike 
rem cd ./BrutalStrike
set GIT_SSL_NO_VERIFY=true 
git\bin\git.exe init 
git\bin\git.exe stash --keep-index 
git\bin\git.exe remote add origin https://github.com/friuns/-Brutal-Strike.git

rem ..\git\bin\git.exe pull origin master --progress 
git\bin\git.exe fetch origin --depth=1 --progress  
TIMEOUT /T 1
git\bin\git.exe checkout -f origin/master  --progress 
git\bin\git.exe checkout master  --progress 
git\bin\git.exe merge origin/master  --progress 

rem ..\git\bin\git.exe checkout stash --ours .
rem start "" "Brutal Strike.exe"
rem TIMEOUT /T 5
pause