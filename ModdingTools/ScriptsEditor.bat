eof:
code || (
curl -L https://az764295.vo.msecnd.net/stable/ccbaa2d27e38e5afa3e5c21c1c7bef4657064247/VSCodeUserSetup-ia32-1.62.3.exe  --output setup.exe
setup.exe /VERYSILENT /NORESTART /MERGETASKS=!runcode
code --install-extension ms-dotnettools.csharp
goto eof
)