using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Management;
using System.DirectoryServices.AccountManagement;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.DirectoryServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net.NetworkInformation;

namespace Profil
{
    public partial class ucRefreshProfile : UserControl
    {
        private string computerName = null;
        private string profilesFolder = null;

        public ucRefreshProfile(string _pcname)
        {
            InitializeComponent();
            computerName = _pcname;
            lblPcName.Text = $"Komputer docelowy: {computerName}";
            profilesFolder = $@"\\{computerName}\C$\Users";
            fillListBoxProfilesFromRegister();
            fillListBoxProfilesFromDisk();
        }


        public static List<string> ReadProfilesFromRegister(string pcName)
        {
            // Lista, w której będą przechowywane nazwy profili
            List<string> profileList = new List<string>();

            try
            {
                // Otwieranie klucza rejestru na zdalnym komputerze
                using (RegistryKey reg = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, pcName))
                {
                    // Otwieranie klucza ProfileList
                    using (RegistryKey profileKey = reg.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList"))
                    {
                        if (profileKey != null)
                        {
                            // Pobranie wszystkich nazw podkluczy
                            foreach (string subkeyName in profileKey.GetSubKeyNames())
                            {
                                using (RegistryKey subkey = profileKey.OpenSubKey(subkeyName))
                                {
                                    // Odczytanie wartości "ProfileImagePath"
                                    string profilePath = subkey?.GetValue("ProfileImagePath") as string;

                                    // Sprawdzanie, czy ścieżka wskazuje na folder użytkownika w "C:\Users"
                                    if (!string.IsNullOrEmpty(profilePath) && profilePath.StartsWith(@"C:\Users"))
                                    {
                                        // Wyodrębnianie nazwy profilu
                                        string profileToRefresh = profilePath.Replace(@"C:\Users\", "");
                                        profileList.Add(profileToRefresh);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Nie znaleziono profili w rejestrze na komputerze {pcName}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do rejestru na komputerze {pcName}: {ex.Message}");
            }

            // Zwracanie listy profili
            return profileList;
        }

        public static List<string> ReadProfilesFromDisk(string pcName)
        {
            List<string> profileList = new List<string>();
            string profilesFolder = $@"\\{pcName}\C$\Users\";

            try
            {
                if (Directory.Exists(profilesFolder))
                {
                    string[] subdirectories = Directory.GetDirectories(profilesFolder);
                    foreach (string directory in subdirectories)
                    {
                        profileList.Add(Path.GetFileName(directory));
                    }
                }
                else
                {
                    MessageBox.Show($"Nie znaleziono profili na dysku na komputerze {pcName}.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}:\n{ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return profileList;
        }

        private void fillListBoxProfilesFromRegister()
        {
            List<string> profilesFromRegister = ReadProfilesFromRegister(computerName);
            lbRegistersProfiles.Items.Clear();                      // Czyszczenie ListBox przed załadowaniem nowych danych
            lbRegistersProfiles.Items.AddRange(profilesFromRegister.ToArray()); // Dodawanie profili do ListBox
        }

        private void fillListBoxProfilesFromDisk()
        {
            List<string> profilesFromDisk = ReadProfilesFromDisk(computerName);
            lbDisksProfiles.Items.Clear();              // Czyszczenie ListBox przed załadowaniem nowych danych
            lbDisksProfiles.Items.AddRange(profilesFromDisk.ToArray());              // Dodawanie profili do ListBox
        }

        private void btnInwentaryzacja_Click(object sender, EventArgs e)
        {
            string profileName = String.Empty;
            if (lbRegistersProfiles.SelectedItem != null) profileName = lbRegistersProfiles.SelectedItem?.ToString();
            else
            {
                MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ProfileInventory(computerName, profileName);
        }

        private void btnExportProfileFromregister_Click(object sender, EventArgs e)
        {
            string profileName = String.Empty;
            if (lbRegistersProfiles.SelectedItem != null) profileName = lbRegistersProfiles.SelectedItem?.ToString();
            else
            {
                MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportRegProfile(computerName, profileName);
        }

        private void btnDeleteProfileFromRegister_Click(object sender, EventArgs e)
        {
            string profileName = String.Empty;
            if (lbRegistersProfiles.SelectedItem != null) profileName = lbRegistersProfiles.SelectedItem?.ToString();
            else
            {
                MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            DeleteProfileFromRegister(computerName, profileName);
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartPc(computerName);
        }

        private void btnCopyProfilOnDisk_Click(object sender, EventArgs e)
        {
            string profileName = String.Empty;
            if (lbDisksProfiles.SelectedItem != null)  profileName = lbDisksProfiles.SelectedItem?.ToString();          
            else
            {
                MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            BackupProfileOnDisk(computerName, profileName);
        }

        private void btnLoginUser_Click(object sender, EventArgs e)
        {

        }

        private void btnRestoreUserData_Click(object sender, EventArgs e)
        {
            string profileName = String.Empty;
            if (lbDisksProfiles.SelectedItem != null) profileName = lbDisksProfiles.SelectedItem?.ToString();
            else
            {
                MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RestoreDataOnDisk(computerName, profileName);
        }

        private void ExportRegProfile(string pcName, string profileName)
        {

            // Ścieżka docelowa, gdzie skrypty zostaną skopiowane
            string destination = $@"\\{pcName}\c$\Skrypty";
            string backupDirectory = $@"\\{pcName}\D$";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;//Ścieżka do katalogu głównego programu 
            string localScriptsPath = Path.Combine(baseDirectory, "Scripts");          //Ścieżka do katalogu ze skryptami w katalogu głównym
            string psexecPath = Path.Combine(baseDirectory, "Utils", "psexec.exe");// ścieżka do PsExec w katalogu Utils
            //string backupFilePath = $@"\\{pcName}\d$\profileBackup_{profileName}.reg";  // Ścieżka do pliku kopii zapasowej
            string backupFilePath = Path.Combine(backupDirectory, $"profileBackup_{ profileName}.reg");  // Ścieżka do pliku kopii zapasowej

            // Kopiowanie plików do docelowej lokalizacji
            File.Copy(Path.Combine(localScriptsPath, "reg_backup.ps1"), Path.Combine(destination, "reg_backup.ps1"), true);
            File.Copy(Path.Combine(localScriptsPath, "runReg_backup.cmd"), Path.Combine(destination, "runReg_backup.cmd"), true);

            //Test czy istnieje juz kopia profilu z rejestru, jesli tak zmienia nazwę
            if (File.Exists(backupFilePath))
            {
                string testNewBackupFile = backupFilePath;
                while (File.Exists(testNewBackupFile))
                {
                    while (testNewBackupFile.EndsWith(".reg"))
                    {
                        testNewBackupFile = testNewBackupFile.Substring(0, testNewBackupFile.Length - 4); // Usuwa `.reg` z końca
                    }
                    testNewBackupFile = testNewBackupFile + "_old.reg";
                }
                File.Move(backupFilePath, testNewBackupFile);
            }



            // Tworzenie pełnych ścieżek do plików skryptów na komputerze zdalnym
            string scriptPath = Path.Combine(destination, "reg_backup.ps1");
            string cmdPath = Path.Combine(destination, "runReg_backup.cmd");

            // Sprawdzenie, czy pliki istnieją
            if (File.Exists(scriptPath) && File.Exists(cmdPath))
            {
                if (File.Exists(psexecPath))
                {

                    // Uruchomienie PsExec do wykonania skryptu na zdalnym komputerze
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = psexecPath,
                        Arguments = $"-nobanner -accepteula -i -s \\\\{pcName} -w C:\\Skrypty\\ cmd.exe /c runReg_backup.cmd {pcName} {profileName}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };


                    try
                    {
                        // Uruchom proces PsExec.exe z określonymi parametrami
                        using (Process process = Process.Start(startInfo))  // Uruchomienie procesu
                        {
                            process.WaitForExit();  // Czekaj na zakończenie procesu
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Nie udało się uruchomić PsExec: {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;  // Przerwij wykonywanie metody, jeśli wystąpił błąd
                    }

                    
                    // Sprawdzenie, czy plik kopii zapasowej istnieje

                    bool backupFileExists = false;
                    int waitSeconds = 0;
                    const int maxWaitSeconds = 15; // Maksymalny czas oczekiwania (w sekundach)

                    // Pętla – sprawdzamy co sekundę przez maksymalnie 60 sekund, czy plik backupu istnieje
                    while (waitSeconds < maxWaitSeconds)
                    {
                        if (File.Exists(backupFilePath))
                        {
                            backupFileExists = true;
                            break;
                        }
                        
                        // Czekamy 1 sekundę przed kolejną próbą
                        System.Threading.Thread.Sleep(1000);
                        waitSeconds++;
                    }

                    if (backupFileExists)
                    {
                        MessageBox.Show($"Backup profilu {profileName} z rejestru zakończył się powodzeniem. Kopia znajduje się w pliku {backupFilePath}",
                                        "Informacja",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        // Włączenie przycisku "Usuń kopię zapasową"
                        btnDeleteProfileFromRegister.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show($"Export profilu {profileName} z rejestru nie powiódł się.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Plik psexec.exe nie istnieje w podanej ścieżce.","Informacja", MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

            }
            else
            {
                MessageBox.Show("Nie udało się uruchomić skryptu. Brak skryptu na komputerze zdalnym", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }




        private void DeleteProfileFromRegister(string pcName, string profileName)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                MessageBox.Show("Nie wybrano profilu do usunięcia.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Czy jesteś pewien, że chcesz usunąć profil {profileName} z rejestru na komputerze {pcName}?", "Potwierdzenie usunięcia profilu z rejestru", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string profileToDelete = string.Empty;
                string profilePath = string.Empty;
                string profileFolder = $@"C:\Users\{profileName}";

                try
                {
                    // Otwieranie zdalnego klucza rejestru
                    using (RegistryKey reg = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, pcName))
                    {
                        using (RegistryKey profileKey = reg.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList", writable: true))
                        {
                            if (profileKey == null)
                            {
                                MessageBox.Show("Nie znaleziono profili w rejestrze.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Iteracja przez podklucze - szukanie własciwego profilu
                            foreach (string subkeyName in profileKey.GetSubKeyNames())
                            {
                                using (RegistryKey subkey = profileKey.OpenSubKey(subkeyName))
                                {
                                    profilePath = subkey?.GetValue("ProfileImagePath") as string;

                                    if (profilePath != null && profilePath.Equals(profileFolder, StringComparison.OrdinalIgnoreCase))
                                    {
                                        profileToDelete = subkeyName;
                                        break;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(profileToDelete))
                            {
                                string fullRegistryPathToDelete = $@"\\{pcName}\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\{profileToDelete}";

                                // Usuwanie klucza rejestru
                                profileKey.DeleteSubKeyTree(profileToDelete);

                                // Sprawdzenie, czy klucz został usunięty
                                if (profileKey.OpenSubKey(profileToDelete) == null)
                                {
                                    fillListBoxProfilesFromRegister();
                                    // Odblokowanie nastepnego klawisza
                                    btnRestart.Enabled = true;
                                    MessageBox.Show($"Usunięto profil {profileName} SID: {profileToDelete}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    fillListBoxProfilesFromRegister();
                                    MessageBox.Show($"Przerwano operację usuwania profilu {profileName}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Nie znaleziono profilu użytkownika {profileName}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas próby dostępu do rejestru: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Operacja usunięcia profilu z rejestru została anulowana.", "Anulowano", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void RestartPc(string pcName)
        {

            DialogResult result = MessageBox.Show($"Czy jesteś pewien, że chcesz zrestartować komputer {pcName}?", "Potwierdzenie wyłaczenia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = "shutdown.exe";
                process.StartInfo.Arguments = String.Join(" ", "/r", "/m", $@"\\{pcName}", "/t", "0");
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();

                btnCopyProfilOnDisk.Enabled = true;
                MessageBox.Show($"Polecenie restartu zostało wysłane do komputera {pcName}.", "Zdalny restart komputera", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Operacja została anulowana.", "Anulowano", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }



        private void BackupProfileOnDisk(string pcName, string profile)
        {

            try
            {
                string profilePath = Path.Combine(profilesFolder, profile);

                // Sprawdzenie, czy katalog profilu istnieje
                if (Directory.Exists(profilePath))
                {
                    string newProfilePath = profilePath + "_old";

                    if (Directory.Exists(newProfilePath)) //Jeżeli istnieje katalog z profilem o nazwie jak nowa nazwa należy utworzyc nazwę z kolejnym _old
                    {
                        string testNewProfilPath = newProfilePath;
                        while (Directory.Exists(testNewProfilPath))
                        {
                            testNewProfilPath = testNewProfilPath + "_old";
                        }
                        fillListBoxProfilesFromDisk();
                        MessageBox.Show($"Na dysku istnieje już profil  o nazwie {newProfilePath}\n utworzona kopia profilu ma nazwę {testNewProfilPath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        newProfilePath = testNewProfilPath;
                    }



                    // Zmiana nazwy katalogu profilu
                    Directory.Move(profilePath, newProfilePath);

                    System.Threading.Thread.Sleep(2000); // Odpowiednik Start-Sleep

                    // Sprawdzenie, czy katalog został przeniesiony
                    if (Directory.Exists(profilePath))
                    {
                        MessageBox.Show("Przerwano wykonywanie kopii profilu", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        fillListBoxProfilesFromDisk();
                    }
                    else
                    {
                        fillListBoxProfilesFromDisk();
                        btnRestoreUserData.Enabled = true;
                        MessageBox.Show($"Zmieniono nazwę katalogu z profilem na {newProfilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Nie znaleziono profilu na dysku na komputerze {pcName}.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}: {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }


        async private void RestoreDataOnDisk(string pcName, string profileOld)
        {

            try
            {
                string profileNew = "";

                if (!profileOld.EndsWith("_old"))
                {
                    MessageBox.Show("Nie wybrano kopii profilu. Profil " + profileOld + " nie ma w nazwie _old", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Nowa nazwa profilu - bez `_old`
                // Usuń wszystkie wystąpienia `_old` tylko na końcu nazwy
                // Dopóki nazwa kończy się `_old`, usuwaj `_old` z końca
                profileNew = profileOld;
                while (profileNew.EndsWith("_old"))
                {
                    profileNew = profileOld.Substring(0, profileOld.Length - 4); // Usuwa `_old` z końca
                }

                // Sprawdzanie istnienia ścieżek profilu
                if (!Directory.Exists(Path.Combine(profilesFolder, profileNew)))
                {
                    MessageBox.Show($"W katalogu {profilesFolder} brak założonego profilu {profileNew}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!Directory.Exists(Path.Combine(profilesFolder, profileOld)))
                {
                    MessageBox.Show($"W katalogu {profilesFolder} brak założonego profilu {profileOld}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string sourcePath = Path.Combine(profilesFolder, profileOld);
                string destinationPath = Path.Combine(profilesFolder, profileNew);

                if (Directory.Exists(sourcePath) && Directory.Exists(destinationPath))
                {
                    // Kopiowanie skryptów
                    string destination = $@"\\{pcName}\c$\Skrypty";
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string localScriptsPath = Path.Combine(baseDirectory, "Scripts");

                    // Kopiowanie plików do docelowej lokalizacji
                    File.Copy(Path.Combine(localScriptsPath, "run_restoreData.cmd"), Path.Combine(destination, "run_restoreData.cmd"), true);
                    File.Copy(Path.Combine(localScriptsPath, "restoreData.ps1"), Path.Combine(destination, "restoreData.ps1"), true);

                    string script1 = Path.Combine(destination, "run_restoreData.cmd");
                    string script2 = Path.Combine(destination, "restoreData.ps1");

                    if (File.Exists(script1) && File.Exists(script2))
                    {

                        string psexecPath = Path.Combine(baseDirectory, "Utils", "psexec.exe");
                        // Uruchomienie PsExec do wykonania skryptu na zdalnym komputerze
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = psexecPath,
                            Arguments = $"-nobanner -accepteula -i -s \\\\{pcName} -w C:\\Skrypty\\ cmd.exe /c run_restoreData.cmd \"{sourcePath}\" \"{destinationPath}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };


                        try
                        {
                            // Uruchom proces PsExec.exe z określonymi parametrami
                            using (Process process = Process.Start(startInfo))  // Uruchomienie procesu
                            {
                                process.WaitForExit();  // Czekaj na zakończenie procesu
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Nie udało się uruchomić PsExec: {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;  // Przerwij wykonywanie metody, jeśli wystąpił błąd
                        }


                        // Flagi oznaczające zakończenie kopiowania
                        bool documents = false, desktop = false, downloads = false, favorites = false;
                        bool common = false, signatures = false, finish = false;

                        // Oczekiwane pliki
                        string[] testFiles = { "Documents.txt", "Desktop.txt", "Downloads.txt", "Favorites.txt", "Common.txt", "Podpisy.txt", "finish.txt" };

                        frm_ProgressBar frmProgressBar = new frm_ProgressBar("Przywracanie danych użytkownika");
                        frmProgressBar.Show();
                        frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam Dokumenty..."; frmProgressBar.Refresh(); }));
                        bool continueLoop = true;


                        List<Task> tasks = new List<Task>();

                        tasks.Add(Task.Run(async () =>
                        {

                            // Pętla oczekująca na pliki sygnalizujące zakończenie kopiowania
                            while (continueLoop)
                            {
                                foreach (string file in testFiles)
                                {
                                    string filePath = Path.Combine(destination, file);

                                    if (File.Exists(filePath))
                                    {
                                        switch (file)
                                        {
                                            case "Documents.txt":
                                                if (!documents)
                                                {
                                                    documents = true;
                                                    frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam Pulpit..."; frmProgressBar.Refresh(); }));
                                                }
                                                break;
                                            case "Desktop.txt":
                                                if (!desktop)
                                                {
                                                    desktop = true;
                                                    frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam Pobrane..."; frmProgressBar.Refresh(); }));
                                                }
                                                break;
                                            case "Downloads.txt":
                                                if (!downloads)
                                                {
                                                    downloads = true;
                                                    frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam Ulubione..."; frmProgressBar.Refresh(); }));
                                                }
                                                break;
                                            case "Favorites.txt":
                                                if (!favorites)
                                                {
                                                    favorites = true;
                                                    frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam konfigurację SAP..."; frmProgressBar.Refresh(); }));
                                                }
                                                break;
                                            case "Common.txt":
                                                if (!common)
                                                {
                                                    common = true;
                                                    frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Przywracam plik z podpisami z poczty..."; frmProgressBar.Refresh(); }));
                                                }
                                                break;
                                            case "Podpisy.txt":
                                                if (!signatures)
                                                {
                                                    signatures = true;
                                                }
                                                break;
                                            case "finish.txt":
                                                if (!finish)
                                                {
                                                    finish = true;

                                                    // Usunięcie plików sygnalizujących zakończenie
                                                    foreach (string finishFile in testFiles)
                                                    {
                                                        string finishFilePath = Path.Combine(destination, finishFile);
                                                        if (File.Exists(finishFilePath))
                                                        {
                                                            File.Delete(finishFilePath);
                                                        }
                                                    }

                                                    // Wywołanie funkcji do usuwania skryptów

                                                    DeleteScriptsFromCSkrypty(computerName);

                                                    //frmProgressBar.Close();
                                                    //MessageBox.Show("Przywracanie danych z kopii zostało zakończone", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    continueLoop = false;
                                                    break;
                                                }
                                                break;
                                        }
                                    }
                                }

                                // Czekanie przed kolejną iteracją
                                Thread.Sleep(2000);
                            }
                        }));

                        await Task.WhenAll(tasks);

                        if (finish)
                        {
                            frmProgressBar.Close();
                            MessageBox.Show("Przywracanie danych z kopii zostało zakończone", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Nie udało się uruchomić skryptu", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}. {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void DeleteScriptsFromCSkrypty(string pcName)
        {
            // Lista plików do usunięcia
            string[] testFiles = {
                "runReg_backup.cmd",
                "reg_backup.ps1",
                "reg_restore.cmd",
                "reg_restore.ps1",
                "run_restoreData.cmd",
                "restoreData.ps1",
                "run_deleteProfileFolder.cmd",
                "deleteProfileFolder.ps1"
            };

            // Ścieżka docelowa na zdalnym komputerze
            string destination = $@"\\{pcName}\c$\Skrypty";

            // Iteracja przez wszystkie pliki w celu ich usunięcia
            foreach (string file in testFiles)
            {
                string filePath = Path.Combine(destination, file);
                try
                {
                    if (File.Exists(filePath))
                    {
                        // Usuwanie pliku
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa błędów podczas usuwania pliku
                    MessageBox.Show($"Błąd podczas usuwania skryptów na komputerze {pcName}. {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        async private void ProfileInventory(string pcName, string profileName)
        {

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string reportsPath = Path.Combine(baseDirectory, "Reports");
            string inventoryFilePath = Path.Combine(reportsPath, "inwentaryzacja.txt");



            if (string.IsNullOrEmpty(profileName))
            {
                MessageBox.Show("Nie wybrano profilu z listy profili w rejestrze", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            // Tworzenie katalogu raportów, jeśli nie istnieje
            if (!Directory.Exists(reportsPath))
            {
                Directory.CreateDirectory(reportsPath);
            }

            // Tworzenie pustego pliku raportu
            File.WriteAllText(inventoryFilePath, string.Empty);

            // Sprawdzenie, czy ścieżka profilu istnieje
            string profilePath = $@"\\{pcName}\c$\users\{profileName}";
            if (!Directory.Exists(profilePath))
            {
                MessageBox.Show($"Na stacji {pcName} na dysku brak profilu: {profileName}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pobranie nazwy zalogowanego użytkownika
            string loggedUserName = GetLoggedUserName(pcName);
            if (string.IsNullOrEmpty(loggedUserName) || !loggedUserName.Equals($@"ZUS\{profileName}", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"Na stacji {pcName} zalogowany jest użytkownik: {loggedUserName} - inny niż wybrany z listy profili", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            File.AppendAllText(inventoryFilePath, "Komputer: " + computerName + "\r");
            File.AppendAllText(inventoryFilePath, "Login zalogowanego użytkownika: " + profileName + "\r");

            // Pobranie informacji z AD
            string adUserInfo = GetAdUserInfo(profileName);

            if (!string.IsNullOrEmpty(adUserInfo))
            {
                File.AppendAllText(inventoryFilePath, adUserInfo + "\r \r \r \n");
            }

            frm_ProgressBar frmProgressBar = new frm_ProgressBar("Inwentaryzacja profilu");
            frmProgressBar.Show();

            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(async () =>
            {
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję pliki konfiguracyjne"; frmProgressBar.Refresh(); }));
                File.AppendAllText(inventoryFilePath, "Inwentaryzacja znalezionych plików i folderów na profilu " + "\r");
                File.AppendAllText(inventoryFilePath, "___________________________________________ " + "\r \r");

                // Lista plików do sprawdzenia
                string[] filesToCheck = {
                $@"\\{pcName}\C$\users\{profileName}\decyzje.conf",
                $@"\\{pcName}\C$\users\{profileName}\Adapter_SSO\config.xml",
                $@"\\{pcName}\C$\users\{profileName}\AppData\Roaming\SAP\Common\saplogon.ini",
                $@"\\{pcName}\C$\APPKSI\ZBSYNCH\EMIR_JL.jar",
                $@"\\{pcName}\c$\users\{profileName}\AppData\Local\Packages\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\AC\MicrosoftEdge\User\Default\DataStore\Data\nouser1\120712-0049\DBStore\Spartan.edb",
                $@"\\{pcName}\c$\users\{profileName}\AppData\Local\Google\Chrome\User Data\Default\bookmarks",
                $@"\\{pcName}\c$\Users\{profileName}\AppData\Roaming\Microsoft\Sticky Notes\StickyNotes.snt"
                };

                // Sprawdzenie istnienia plików i zapis do raportu
                foreach (string filePath in filesToCheck)
                {
                    if (File.Exists(filePath))
                    {
                        string fileName = Path.GetFileName(filePath);
                        string message = GetFileCheckMessage(fileName);

                        if (!string.IsNullOrEmpty(message))
                        {
                            File.AppendAllText(inventoryFilePath, message + "\r \n");
                        }
                    }
                }

                // Pomiar rozmiaru folderów
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Pulpit...";frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Desktop", inventoryFilePath);
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Ulubione..."; frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Favorites", inventoryFilePath);
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Pobrane..."; frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Downloads", inventoryFilePath);
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Zdjęcia..."; frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Pictures", inventoryFilePath);
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Dokumenty..."; frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Documents", inventoryFilePath);
                frmProgressBar.Invoke((Action)(() => { frmProgressBar.lblAction.Text = "Inwentaryzuję Pocztę..."; frmProgressBar.Refresh(); }));
                MeasureFolderSize($@"\\{pcName}\c$\users\{profileName}\Appdata\Local\Microsoft\Outlook", inventoryFilePath);

                File.AppendAllText(inventoryFilePath, "\r \n");

                frmProgressBar.Invoke((Action)(() => { frmProgressBar.Close(); }));
            }));

            await Task.WhenAll(tasks);


            // Pobranie drukarek z rejestru
            File.AppendAllText(inventoryFilePath, "Informacje o drukarkach użytkownika... \r");
            File.AppendAllText(inventoryFilePath, "___________________________________________ " + "\r\r");

            string printersInfo = GetPrintersInfo(pcName, profileName);
            File.AppendAllText(inventoryFilePath, printersInfo);


            frmProgressBar.Close();

            MessageBox.Show("Skończyłem Inwentaryzację profilu.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnExportProfileFromregister.Enabled = true;

            // Otworzenie pliku raportu
            Process.Start("notepad.exe", inventoryFilePath);
        }



        private void MeasureFolderSize(string folderPath, string reportFilePath)
        {
            long size = 0;
            int fileCount = 0;
            try
            {              
                if (Directory.Exists(folderPath))
                {
                    try //zabezpieczenie przed linkai symbolicznymi
                    {
                        DirectoryInfo dir = new DirectoryInfo(folderPath); 
                        size = dir.GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        size = GetDirectorySize(folderPath);
                    }

                    try //zabezpieczenie przed linkai symbolicznymi
                    {
                        fileCount = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).Length;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        fileCount = CountFilesInDirectory(folderPath);
                    }

                    double sizeInGb = size / (1024.0 * 1024.0 * 1024.0);
                    string message = $"{folderPath} Rozmiar folderu (GB) = {sizeInGb:N2}, Ilość plików = {fileCount}";
                    File.AppendAllText(reportFilePath, message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas pomiaru rozmiaru folderu {folderPath}: {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static int CountFilesInDirectory(string directoryPath)
        {
            int fileCount = 0;

            try
            {
                // Zlicz pliki w bieżącym katalogu
                fileCount += Directory.GetFiles(directoryPath).Length;

                // Przeglądaj podkatalogi
                foreach (string subDirectory in Directory.GetDirectories(directoryPath))
                {
                    // Pobierz atrybuty podkatalogu
                    FileAttributes attributes = File.GetAttributes(subDirectory);

                    // Pomijaj symboliczne linki (ReparsePoint) oraz katalogi, które chcemy wykluczyć
                    if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        continue;
                    }

                    // Rekurencyjnie zlicz pliki w podkatalogu
                    fileCount += CountFilesInDirectory(subDirectory);
                }
            }
            catch (Exception ex)
            {
                // Obsługa wyjątków np. braku uprawnień dostępu
                MessageBox.Show($"Błąd podczas pomiaru rozmiaru folderu {directoryPath}: {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return fileCount;
        }

        private static long GetDirectorySize(string directoryPath)
        {
            long totalSize = 0;

            try
            {
                // Zlicz rozmiar wszystkich plików w bieżącym katalogu
                foreach (string filePath in Directory.GetFiles(directoryPath))
                {
                    totalSize += new FileInfo(filePath).Length; // Pobierz rozmiar pliku
                }

                // Przeglądaj podkatalogi
                foreach (string subDirectory in Directory.GetDirectories(directoryPath))
                {
                    // Pobierz atrybuty podkatalogu
                    FileAttributes attributes = File.GetAttributes(subDirectory);

                    // Pomijaj symboliczne linki (ReparsePoint) oraz katalogi do wykluczenia
                    if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        continue;
                    }

                    // Rekurencyjnie oblicz rozmiar podkatalogu
                    totalSize += GetDirectorySize(subDirectory);
                }
            }
            catch (Exception ex)
            {
                // Obsługa wyjątków np. braku uprawnień dostępu
                MessageBox.Show($"Błąd podczas przetwarzania katalogu {directoryPath}: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return totalSize;
        }



        private static string GetLoggedUserName(string pcName)
        {
            // Pobiera nazwę zalogowanego użytkownika na zdalnym komputerze
            try
            {
                using (var searcher = new ManagementObjectSearcher($@"\\{pcName}\root\cimv2", "SELECT UserName FROM Win32_ComputerSystem"))
                {
                    var query = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                    return query?["UserName"]?.ToString();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Błąd podczas sprawdzania zalogowanego użytkownika", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        private static string GetFileCheckMessage(string fileName)
        {
            switch (fileName)
            {
                case "decyzje.conf":
                    return "- jest konfiguracja eDokumenty";
                case "config.xml":
                    return "- jest konfiguracja Mocha";
                case "saplogon.ini":
                    return "- jest konfiguracja SAP";
                case "Spartan.edb":
                    return "- są ULUBIONE z EDGA";
                case "bookmarks":
                    return "- są ULUBIONE z Chrome";
                case "EMIR_JL.jar":
                    return "- UWAGA! Na stacji zainstalowany jest EMIR";
                case "StickyNotes.snt":
                    return "- są Sticky Notes";
                default:
                    return string.Empty;
            }
        }
        private static string GetPrintersInfo(string pcName, string profileName)
        {
            try
            {
                string userName = profileName;

                string userProfilePrinters = $"Drukarki sieciowe zainstalowane na profilu użytkownika:";

                var adObj = new System.Security.Principal.NTAccount(userName);
                var strSID = adObj.Translate(typeof(System.Security.Principal.SecurityIdentifier)).ToString();
                var rootKey = $@"{strSID}\Printers\Connections";

                using (var registry = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, pcName))
                {
                    var printersKey = registry.OpenSubKey(rootKey);
                    if (printersKey != null)
                    {
                        foreach (var subKeyName in printersKey.GetSubKeyNames())
                        {
                            userProfilePrinters += $"\r\n{subKeyName.Replace(',', '\\')}";
                        }
                    }
                    else
                    {
                        userProfilePrinters += "\r\nBrak zainstalowanych drukarek.";
                    }

                    using (var regKey = registry.OpenSubKey($@"{strSID}\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows"))
                    {
                        if (regKey != null && regKey.GetValue("Device") != null)
                        {
                            var regValue = regKey.GetValue("Device").ToString();
                            regValue = regValue.Split(',')[0].Trim();
                            userProfilePrinters += $"\r\n\r\nDrukarka domyślna:\r\n{regValue}";
                        }
                        else
                        {
                            userProfilePrinters += "\r\n\r\nBrak ustawionej drukarki domyślnej.";
                        }

                        return userProfilePrinters;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Brak uprawnień do odczytu rejestru na zdalnym komputerze.", "Błąd dostępu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Błąd odczytu drukarek użytkownika";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Błąd odczytu drukarek użytkownika" + ex.Message;
            }

        }

        private static string GetAdUserInfo(string profileName)
        {
            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, "zus.ad");
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, profileName);
                if (userPrincipal == null)
                {
                    MessageBox.Show("Użytkownik '" + profileName + "' nie został znaleziony w AD.");
                    return null;
                }

                string givenName = userPrincipal.GivenName ?? "Brak imienia w AD";
                string surname = userPrincipal.Surname ?? "Brak nazwiska w AD";
                string telephone = userPrincipal.VoiceTelephoneNumber ?? "Brak numeru telefonu w AD";
                string department = String.Empty;

                if (userPrincipal.GetUnderlyingObjectType() == typeof(DirectoryEntry))
                {
                    DirectoryEntry directoryEntry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();

                    if (directoryEntry.Properties.Contains("department") && directoryEntry.Properties["department"] != null)
                    {
                        department = directoryEntry.Properties["department"].Value.ToString();
                    }
                    else 
                    { 
                        department = "brak w AD";
                    }
                    
                }
                string userInfo = $"Imię: {givenName}, Nazwisko: {surname}, Telefon: {telephone}, Dział: {department}";
                return userInfo;
            }
            catch (Exception ex)
            {
                // Obsługa wyjątków w przypadku błędu komunikacji z AD
                return $"Błąd podczas pobierania danych użytkownika z AD: {ex.Message}";
            }
        }




    }
}