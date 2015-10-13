pushd ..\Dlls
& .\pack.bat

if (-not (Test-Path ../../packages)) { mkdir ../../packages }
cp *.nupkg ../../packages
popd
