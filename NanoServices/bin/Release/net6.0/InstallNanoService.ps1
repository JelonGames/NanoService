$nameServices = 'NanoServices'
$pathApp = 'C:\Nano\NanoServices.exe'
$pathAppSettings = 'C:\Nano\json\UserSettings.json'
$pathSystem32 = 'C:\Windows\System32\NanoSettings\json'


mkdir -Force $pathSystem32
cp -Force $pathAppSettings $pathSystem32


sc.exe create $nameServices start= auto binPath= $pathApp
sc.exe start $nameServices