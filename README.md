# Nano Services 
## Spis treści 
1. Działanie, instalacja, itp..
2. Zasoby
3. Pierwsza pomoc
# 1. Działanie, instalacja, itp..
## 1.1 Działanie
Usługa pobiera dane z urządzenia pod addresem `address/temp1.txt`. Na stronie znajduję się tylko informacja o temperaturze która została zczytana z urządzenia. Przykładowy sposób na sprawdzenie czy użądzenie wypisuje dobrze temperature `Invoke-WebRequest "ipUrządzenia/temp1.txt" | Select-Object -ExpandProperty Content`  Usługa addressy urządzeń pobiera z pliku json `Dokaładny opis w 2. Zasoby`.
Jeżeli usługa nie będzie wstanie odczytać lub wartość pedzie przekraczać dopuszczalny zakres temperatur, to wyśle mail i w logach pojawi się komunikat `[Warning]`.
## 1.2 Instalacja i deinstalacja
#### 1.2.1 Ważne pliki
W folderze z aplikacją znajdują się 2 pliki:
- InstallNanoService.ps1
- UninstallNanoService.ps1

#### 1.2.2 Przed instalacją/deinstalacją
Przed uruchomieniem którego kolwiek z pliku trzeba najpierw upenić się 
że mamy odpowiednią politykę.

1. Uruchomienie powershella jako administrator
2. Wpisać komendę `Set-ExecutionPolicy RemoteSigned -Scope LocalMachine`. Bez tego nie będziecie wstanie uruchomić instalacji.

Istenieje jeszcze opcja zainstalowania `patrz 1.2.3` lub odinstalowania `patrz 1.2.4` przez PowerShell ISE.

#### 1.2.3 Instalacja
Jeżeli wykonaliśmy punkt `1.2.2`:
1. Musimy edytować zmienne w pliku `InstallNanoService.ps1` zgodnie z komnetarzami w kodzie poniżej \\/
```ps1
$nameServices = 'NanoServices'
#oznacza nazwę usługi
$pathApp = 'C:\Nano\NanoServices.exe'
#Ścieżka do aplikacji którą usługa ma uruchomić
$pathAppSettings = 'C:\Nano\json\UserSettings.json' 
#Ścieżka do pliku json
$pathSystem32 = 'C:\Windows\System32\NanoSettings\json' 
#POD ŻADNYM POZOREM NIE RUSZAĆ BO APLIKACJA NIE BĘDZIE MIAŁA DANYCH DO PRACY
```
Lokalizacja w zmiennej `$pathSystem32` jest podyktowana czytaniem pliku ustawień json.
```C#
private static string _json()
{
	try
	{
		return File.ReadAllText(@"./json/UserSettings.json"); 
		//ścieżka w projekcie w trakcie debugowania
	}
	catch
	{
		return File.ReadAllText(@"C:/Windows/System32/NanoSettings/json/UserSettings.json"); 
		//ścieżka w systemie dla usługi
	}
}

public static string json
{
	get { return _json(); }
}
```
2. Można instalator uruchomić albo za pomocą PowerShell podając do niego ściężkę. Jeśli jednak dostaniemy informację "Odmowa dostępu" to można wykonać to z PowerShell ISE i tam go uruchomić.
 
#### 1.2.4 Deinstalacja
W celu odinstalowania usługi trzeba:
1. W pliku `UninstallNanoService.ps1` trzeba edytować nazwę usługi na nazwę pod którą została zainstalowana.
```ps1
$nameServices = 'NanoServices' 
#nazwa usługi musi się zdadzać
$pathSystem32 = 'C:\Windows\System32\NanoSettings'
#TA SAMA ZASADA CO W PULKCIE 1.2.3
```
2. Można deinstalator uruchomić albo za pomocą PowerShell podając do niego ściężkę. Jeśli jednak dostaniemy informację "Odmowa dostępu" to można wykonać to z PowerShell ISE i tam go uruchomić.

# 2. Zasoby
Najważniejsze pliki do pracy usługi:
1. InstallNanoServices.ps1
2. UnInstallNanoServices.ps1
3. UserSettings.json
4. NanoServices.exe

#### 2.1 InstallNanoServices.ps1
```ps1
$nameServices = 'NanoServices'
#oznacza nazwę usługi
$pathApp = 'C:\Nano\NanoServices.exe'
#Ścieżka do aplikacji którą usługa ma uruchomić
$pathAppSettings = 'C:\Nano\json\UserSettings.json' 
#Ścieżka do pliku json
$pathSystem32 = 'C:\Windows\System32\NanoSettings\json' 
#POD ŻADNYM POZOREM NIE RUSZAĆ BO APLIKACJA NIE BĘDZIE MIAŁA DANYCH DO PRACY


#Tworzenie folderu i kopiowanie ustawień do stworzonego folderu
mkdir -Force $pathSystem32
cp -Force $pathAppSettings $pathSystem32


#Towżenie usługi
sc.exe create $nameServices start= auto binPath= $pathApp
sc.exe start $nameServices
```

#### 2.2 UninstallNanoServices.ps1
```ps1
$nameServices = 'NanoServices' 
#nazwa usługi musi się zdadzać
$pathSystem32 = 'C:\Windows\System32\NanoSettings'
#TAK SAMO JAK WYRZEJ TEGO DNIE DOTYKAĆ

#Usuwanie pliku i katalogu z ustawieniami 
rmdir -Recurse -Force $pathSystem32

#Zatrzymanie usługi i nadanie jej flagi do usunięcia
sc.exe stop $nameServices
sc.exe delete $nameServices

#Restart komputera, potrzebny jest by usługa się usuneła 
$value = Read-Host "Czy chcesz uruchomić ponownie komputer w celu dokończenia deinstalacji? (y/n)"
if ($value -eq 'y')
{
    Restart-Computer -Force
}
else
{
    Write-Output "Usługa została zatrzymana i ustawiona jako 'Do usunięcia'"
}
```

#### 2.3 UserSettings.json
W Jsonie można znaleść wszystkie zmienne do konfiguracji usługi. Najlepiej edytować Notepad++ lub innym edytorem programistycznym/specjalistycznym.
```json
{
  "DeviceSettings": {
    //Zmienna sprawdzająca ile użądzeń jest podpięta
	  //Jeżeli jest więcej niż jedno urządzenie trzeba zmienić na odpowiednią liczbę
    "DevicesCountConnect": 1,
    "Device1": {
      "Name": "Nano 1",
      "IP": ""
    } //,
    // Przykład dodawania kolejnego urządzenia
    //    "Device2": {
    //      "Name": "",
    //      "IP": ""
    //    }
  },

  "AppSettings": {
	  //IntervalCheker odpowiada jak często ma usługa sprawdzać temperature na urządzeniu
	  // !! nie jest to czas jak często ma być wysyłany mail ostrzegawczy
    "IntervalChecker": 2000, //milsec
	  // Tutaj ustawiamy granice akceptowalnej temperatury
    "Alert": {
      "High": 30,
      "Low": 10
    },
    "Logs": {
      // wpisanie w scieżce podwojego % spowoduje ze w tym miejscu wpisze date i utworzy nowy plik
      "Path": "A:\\nano test\\logs%%.txt",
      // Jakie logi maja byc zapisywane
      // 0 - wszystkie, co kazdy interval
      // 1 - o typie warning, tylko krytyczne
      "LogsPioryty": 0
    }
  },

  "MailSettings": {
    "MailProperty": {
      "Domain": "", //Domena
      "Server": "", //Serwer pocztowy
      "Port": 587, //587 lub 25
      "EnableSSl": true, //szyfrowanie SSL
      "Username": "", //Wyświetlana nazwa, 
		//w przypadku autoryzacji maila nie jest wymagane lecz warto podać
      "Login": "", //login, a dokłanie mail z którego będzie wysyłany mail,
		//Oraz będzie dokonywana autoryzacja w serwerze pocztowym
      "Password": "", //w przypadku braku autoryzacji zostawić puste
      "MailSubject": "Test termometru", //Tytuł maili
      "To": "" //Na jaką skrzynke ma być wysyłane
    },
    "MailCounter": 5, //Ile maili ma przyjść o treści ALLERT
    "MailIntervalSend": 900000, //milsec - 900 000 = 15 min
	  //Czas pomiędzy wysłanymi mailami o treści ALLERT
    "MailIntervalStatus": 1800000 //milsec - 1 800 000 = 30 min
	  //Czas pomiędzy wysłanymi mailami o treści status
  }
}
```

#### 2.4 NanoServices.exe
Program uruchamiający cała usługę. Znajduję się w gółwnym katalogu programu
# 3. Pierwsza pomoc
1. Wykonanie w powershell komendy
```ps1
Invoke-WebRequest "ipUrządenia/temp1.txt" | Select-Object -ExpandProperty Content
```
Jeżeli komenda zwróci co kolwiek innego a na podanej stronie w komendzie możemy się bez problemu dostać trzeba zresetować urządzenie (raz w trakcie testów mi się to zdażyło, prawdopodobnie jakieś zwiechy dostało)

2. Jeżeli przy próbie uruchomienia usługi pojawia się błąd o numerze 3 lub usługa odpala się błyskawicznie oznacza że albo lokalizajca aplikacji się zmieniła albo źle została utworzona usługa. W tym przypadku należy ponownie ją zainstalować o porawnej ścieżce w pliku `InstallNanoServices.ps1`
3. Błąd 1053 przy uruchamianiu może być spowodowana albo błędem aplikacji albo błędnie utworzoną usługą.
4. Błąd 1067 oznacza że aplikacja próbuję dostać się do zasobu do którego nie ma dostępu np. UserSettings.json lub to katalogu gdzie zapisywane są logi.
5. Upewnienie się że usługa działa, jeżeli działa i ani logi ani maile nie przychodzą to:
	1. Zatrzym usługę
	2. W powershelu wpisać lokalizację pliku ***exe*** i po spacji dopisać install. Uruchomi się konsla usługi która będzie pracować w ramach zalogowanego użytkownika
	3. Jeżeli wszystko poprawnie tam działa, logi się zapisują to błąd nie leży po utworzonej usługi
