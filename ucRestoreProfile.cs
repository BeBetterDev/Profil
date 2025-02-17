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

namespace Profil
{
    public partial class ucRestoreProfile : UserControl
    {
        private string computerName = null;
        private string profilesFolder = null;

        public ucRestoreProfile(string _pcName)
        {
            InitializeComponent();
            computerName = _pcName;
            lblPcName.Text = $"Przywróć profil - komputer docelowy: {computerName}";
            profilesFolder = $@"\\{computerName}\C$\Users";
            fillListBoxProfilesFromRegister();
            fillListBoxProfilesFromDisk();
        }





        private void btnDeleteProfileFromRegister_tabPrzywracanie_Click(object sender, EventArgs e)
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


        private void btnRestoreProfilFromCopyToDisc_Click(object sender, EventArgs e)
        {
            RestoreProfileOnDisk(computerName);
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
            lbRegistersProfiles.Items.Clear();         // Czyszczenie ListBox przed załadowaniem nowych danych
            lbRegistersProfiles.Items.AddRange(profilesFromRegister.ToArray()); // Dodawanie profili do ListBox
        }

        private void fillListBoxProfilesFromDisk()
        {
            List<string> profilesFromDisk = ReadProfilesFromDisk(computerName);
            lbDisksProfiles.Items.Clear();   // Czyszczenie ListBox przed załadowaniem nowych danych
            lbDisksProfiles.Items.AddRange(profilesFromDisk.ToArray());  // Dodawanie profili do ListBox
        }





        private void DeleteProfileFromRegister(string pcName, string profileName)
        {

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

                            MessageBox.Show($"Wprowadzona nazwa komputera: '{pcName}' | Profil wybrany do usunięcia: '{profileName}'", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                                    //  restartPcButton.Enabled = true;  Odblokowanie nastepnego klawisza

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


        private void RestoreProfileOnDisk(string pcName)
        {
            if (lbDisksProfiles.SelectedItem == null)
            {
                MessageBox.Show($"Nie wybrano profilu z listy profili na dysku.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string profile = lbDisksProfiles.SelectedItem.ToString();

                // Sprawdź, czy nazwa kończy się `_old`
                if (!profile.EndsWith("_old"))
                {
                    MessageBox.Show($"To nie jest prawidłowa kopia  profilu wykonana HADES-em, nie można jej przywrócić.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string oldProfilePath = Path.Combine(profilesFolder, profile);

                // Usuń wszystkie wystąpienia `_old` tylko na końcu nazwy
                // Dopóki nazwa kończy się `_old`, usuwaj `_old` z końca
                string newProfileName = profile;
                while (newProfileName.EndsWith("_old"))
                {
                    newProfileName = newProfileName.Substring(0, newProfileName.Length - 4); // Usuwa `_old` z końca
                }


                string newProfilePath = Path.Combine(profilesFolder, newProfileName);
                if (Directory.Exists(newProfilePath)) //Jeżeli istnieje katalog z profilem o nazwie jak nowa nazwa nalezy go usunąć
                {
                    fillListBoxProfilesFromDisk();
                    MessageBox.Show($"Na dysku istnieje już profil  o nazwie {newProfilePath} \n jeżeli rzeczywiście chcesz przywrócić go z kopii musisz najpierw usunąć poprzedni folder", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                if (Directory.Exists(oldProfilePath))
                {
                    // Zmień nazwę katalogu
                    Directory.Move(oldProfilePath, newProfilePath);

                    if (Directory.Exists(oldProfilePath))
                    {
                        fillListBoxProfilesFromDisk();
                        MessageBox.Show("Przerwano operację przywracania profilu", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (Directory.Exists(newProfilePath))
                    {
                        fillListBoxProfilesFromDisk();
                        MessageBox.Show($"Zmieniono nazwę katalogu z profilem na {newProfilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Nie znaleziono profilu {oldProfilePath} na komputerze {pcName}.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fillListBoxProfilesFromDisk();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}:  {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestoreProfileFromBackupToRegister_Click(object sender, EventArgs e)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;//Ścieżka do katalogu głównego programu 
            string localScriptsPath = Path.Combine(baseDirectory, "Scripts");          //Ścieżka do katalogu ze skryptami w katalogu głównym
            string psexecPath = Path.Combine(baseDirectory, "Utils", "psexec.exe");// ścieżka do PsExec w katalogu Utils

            // Test czy zaznaczono plik kopi do importu
            if (lbBackups.SelectedItem == null)
            {
                MessageBox.Show("Nie wybrano pliku kopii zapasowej profilu z rejestru.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string restoreFileName = lbBackups.SelectedItem.ToString();
            string destination = $@"\\{computerName}\c$\Skrypty";

            try
            {

                string path1 = Path.Combine(destination, "reg_restore.cmd");
                string path2 = Path.Combine(destination, "reg_restore.ps1");

                // Kopiowanie plików skryptów do katalogu docelowego na zdalnym komputerze
                File.Copy(Path.Combine(localScriptsPath, "reg_restore.cmd"), path1, true);
                File.Copy(Path.Combine(localScriptsPath, "reg_restore.ps1"), path2, true);

                // Sprawdzenie, czy pliki istnieją w miejscu docelowym
                if (File.Exists(path1) && File.Exists(path2))
                {

                    // Uruchomienie PsExec do przywrócenia kopii rejestru na zdalnym komputerze
                    //RunPsExec(pcName, destination, restoreFileName);

                    try
                    {
                        Process psexecProcess = new Process();
                        psexecProcess.StartInfo.FileName = "PsExec.exe";
                        psexecProcess.StartInfo.Arguments = $"-nobanner -accepteula -i -s \\\\{computerName} -w C:\\Skrypty\\ cmd.exe /c reg_restore.cmd {restoreFileName}";
                        psexecProcess.StartInfo.UseShellExecute = false;
                        psexecProcess.StartInfo.CreateNoWindow = true;
                        psexecProcess.Start();
                        psexecProcess.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd podczas uruchamiania PsExec: {ex.Message}");
                    }



                    // Oczekiwanie na zakończenie operacji
                    System.Threading.Thread.Sleep(2000);


                    Console.WriteLine($"Zaimportowano profil {restoreFileName}");


                    // Odświeżenie listy profili
                    // ReadProfilesFromRegister();

                    // Resetowanie ComboBoxów
                    //ResetComboBox(objComboboxRegProfileList);
                    fillListBoxProfilesFromRegister();
                    //ResetComboBox(objComboboxRegBackupList);
                    fillListBoxProfilesFromDisk();
                }
                else
                {

                    Console.WriteLine("Nie udało się uruchomić skryptu.");


                    //ReadProfilesFromRegister();
                    //ResetComboBox(objComboboxRegProfileList);
                    fillListBoxProfilesFromRegister();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Błąd podczas przywracania profilu rejestru: {ex.Message}");

            }

        }

        private void btnDeleteProfileFromDisc_Click(object sender, EventArgs e)
        {

            string profileName = String.Empty;
            if (lbDisksProfiles.SelectedItem != null) profileName = lbDisksProfiles.SelectedItem?.ToString();
            else
            {
                MessageBox.Show("Nie wybrano z listy profilu do usunięcia", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DeleteprofileFromDisc(computerName, profileName);

        }


        private void DeleteprofileFromDisc(string pcName, string profile)
        {

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;//Ścieżka do katalogu głównego programu 
            string localScriptsPath = Path.Combine(baseDirectory, "Scripts");          //Ścieżka do katalogu ze skryptami w katalogu głównym
            string psexecPath = Path.Combine(baseDirectory, "Utils", "psexec.exe");// ścieżka do PsExec w katalogu Utils

            DialogResult result = MessageBox.Show($"Czy jesteś pewien, że chcesz usunąć profil {profile} \nz dysku na komputerze {pcName}?", "Potwierdzenie usunięcia profilu z dysku", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string profilePath = Path.Combine(profilesFolder, profile);
                    string destination = $@"\\{pcName}\c$\Skrypty";

                    if (Directory.Exists(profilePath))
                    {

                        string pathCmd = Path.Combine(destination, "run_deleteProfileFolder.cmd");
                        string pathPs1 = Path.Combine(destination, "deleteProfileFolder.ps1");


                        // Kopiowanie skryptów do zdalnej lokalizacji
                        File.Copy(Path.Combine(localScriptsPath, "run_deleteProfileFolder.cmd"), pathCmd, true);
                        File.Copy(Path.Combine(localScriptsPath, "deleteProfileFolder.ps1"), pathPs1, true);



                        if (File.Exists(pathCmd) && File.Exists(pathPs1))
                        {
                            frm_ProgressBar frmProgressBar = new frm_ProgressBar("Usuwanie profilu z dysku");
                            frmProgressBar.lblAction.Text = $"Usuwam z dysku katalog z profilem o nazwie: {profile}";
                            frmProgressBar.lblAction.Location = new Point(100, 217);
                            frmProgressBar.Show();
                            frmProgressBar.Refresh();


                            if (File.Exists(psexecPath))
                            {

                                // Uruchomienie PsExec do usunięcia katalogu profilu
                                ProcessStartInfo startInfo = new ProcessStartInfo
                                {
                                    FileName = psexecPath,
                                    Arguments = $"-nobanner -accepteula -i -s \\\\{pcName} -w C:\\Skrypty\\ cmd.exe /c run_deleteProfileFolder.cmd {profile}",
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


                            }
                            //Thread.Sleep(2000);

                            // Sprawdzenie statusu usuwania
                            string[] testFiles = { "errorDelete.txt", "finishDelete.txt" };
                            bool continueLoop = true;

                            while (continueLoop)
                            {
                                foreach (string file in testFiles)
                                {
                                    string filePath = Path.Combine(destination, file);
                                    if (File.Exists(filePath))
                                    {
                                        if (file == "errorDelete.txt")
                                        {
                                            frmProgressBar.Close();
                                            MessageBox.Show($"Nie udało się usunąć w całości folderu {profilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        if (file == "finishDelete.txt")
                                        {
                                            frmProgressBar.Close();
                                            MessageBox.Show($"Usunięto z dysku profil {profilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }

                                        continueLoop = false;

                                        // Usuwanie plików statusu
                                        foreach (string finishFile in testFiles)
                                        {
                                            string finishFilePath = Path.Combine(destination, finishFile);
                                            if (File.Exists(finishFilePath))
                                            {
                                                File.Delete(finishFilePath);
                                            }
                                        }
                                        break;
                                    }
                                }
                                Thread.Sleep(1000);
                            }

                            // Resetowanie ComboBoxa
                            fillListBoxProfilesFromDisk();
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się uruchomić skryptu.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Sprawdzenie czy profil nadal istnieje
                        if (Directory.Exists(profilePath))
                        {
                            fillListBoxProfilesFromDisk();
                            MessageBox.Show($"Przerwano operację usuwania profilu {profile} \nlub nie usunięto katalogu z profilem w całości.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            fillListBoxProfilesFromDisk();
                            MessageBox.Show($"Usunięto katalog {profilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    else
                    {
                          MessageBox.Show($"Nie znaleziono profilu {profilePath} \nna komputerze {pcName}.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}. {ex.Message}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show($"Usuwanie z dysku profilu {profile} zostało anulowane", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
