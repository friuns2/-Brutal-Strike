if not exist %windir%\Microsoft.NET\Framework\v4.0.30319 (
	curl https://download.microsoft.com/download/B/A/4/BA4A7E71-2906-4B2D-A0E1-80CF16844F5F/dotNetFx45_Full_setup.exe --output setup.exe
	setup.exe /q /norestart	
)
xcopy Scripts "%USERPROFILE%\Documents\BrutalStrike\Scripts\" /S /Y /D
"SharpDevelop/bin/SharpDevelop.exe" %USERPROFILE%\Documents\BrutalStrike\Scripts\ModTemplate.csproj