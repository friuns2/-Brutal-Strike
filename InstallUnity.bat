curl https://download.unity3d.com/download_unity/ca5b14067cec/Windows64EditorInstaller/UnitySetup64-2019.4.19f1.exe -o UnitySetup64.exe
curl http://download.unity3d.com/download_unity/ca5b14067cec/TargetSupportInstaller/UnitySetup-Android-Support-for-Editor-2019.4.19f1.exe -o UnitySetup64Android.exe
curl http://download.unity3d.com/download_unity/ca5b14067cec/TargetSupportInstaller/UnitySetup-WebGL-Support-for-Editor-2019.4.19f1.exe -o UnitySetup64Web.exe

"UnitySetup64.exe" -UI=reduced 
"UnitySetup64Android.exe" /S 
"UnitySetup64Web.exe" /S 
