$nameServices = 'NanoServices'
$pathSystem32 = 'C:\Windows\System32\NanoSettings'

rmdir -Recurse -Force $pathSystem32
sc.exe stop $nameServices
sc.exe delete $nameServices

$value = Read-Host "Czy chcesz uruchomić ponownie komputer w celu dokończenia deinstalacji? (y/n)"
if ($value -eq 'y')
{
    Restart-Computer -Force
}
else
{
    Write-Output "Usługa została zatrzymana i ustawiona jako 'Do usunięcia'"
}