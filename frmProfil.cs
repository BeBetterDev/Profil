using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Profil
{
    public partial class frmProfil : Form
    {
        private string computerName = null;
        private string profilesFolder = null;
        public frmProfil(string _pcName)
        {
            InitializeComponent();
            computerName = _pcName;
            profilesFolder = $@"\\{computerName}\C$\Users";
            FormConfiguration();
        }

        private void FormConfiguration()
        {       
            groupBox.Text = $"[   Komputer docelowy: {computerName}   ]";

            fillListBoxProfilesFromRegister();
            fillListBoxProfilesFromDisk();
        }

        private void enableRegisterListBox()
        { 
        
        }

        private void disableRegisterListBox()
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //# Deklaracja tablicy z nazwami plików


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

        private void fillListBoxProfilesFromRegister()
        {
            List<string> profilesFromRegister = ReadProfilesFromRegister(computerName);
            lbRegistersProfiles.Items.Clear();                      // Czyszczenie ListBox przed załadowaniem nowych danych
            lbRegistersProfiles.Items.AddRange(profilesFromRegister.ToArray()); // Dodawanie profili do ListBox
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

        private void fillListBoxProfilesFromDisk()
        {
            List<string> profilesFromDisk = ReadProfilesFromDisk(computerName);
            lbDisksProfiles.Items.Clear();              // Czyszczenie ListBox przed załadowaniem nowych danych
            lbDisksProfiles.Items.AddRange(profilesFromDisk.ToArray());              // Dodawanie profili do ListBox
        }


        private void ReadProfilesBackupFromDisk()
        { 
        }

        private void DeleteScriptsFromCSkrypty() //funkcja wywoływana po zakończeniu przywracania danych
        { 
        }

        private void InwentaryzacjaProfilu()
        { 
        }

        private string GetFolderSize(string path)
        {
            return null;
        }



        private void ExportRegProfile()
        { 
        }

        private void DeleteProfileFromRegister(string pcName)
        {
            string profileName = lbRegistersProfiles.SelectedItem?.ToString();
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

                MessageBox.Show($"Polecenie restartu zostało wysłane do komputera {pcName}.", "Zdalny restart komputera", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("Operacja została anulowana.", "Anulowano", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void RestoreRegProfile()
        { 
        }

        private void BackupProfileOnDisk(string pcName)
        {

            // Sprawdzenie połączenia z komputerem (ping)
            //if (PingComputer(pcName))
            //{
            try
            {
                if (lbDisksProfiles.SelectedItem != null)
                {

                    string profile = lbDisksProfiles.SelectedItem.ToString();
                    string profilePath = Path.Combine(profilesFolder, profile);

                    // Sprawdzenie, czy katalog profilu istnieje
                    if (Directory.Exists(profilePath))
                    {
                        string newProfilePath = profilePath + "_old";

                        // Zmiana nazwy katalogu profilu
                        Directory.Move(profilePath, newProfilePath);
                        System.Threading.Thread.Sleep(2000); // Odpowiednik Start-Sleep
                        
                        //restoreDataButton.Enabled = true; odblokowanie nastepnego klawisza

                        // Sprawdzenie, czy katalog został przeniesiony
                        if (Directory.Exists(profilePath))
                        {
                            MessageBox.Show("Przerwano wykonywanie kopii profilu","Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            fillListBoxProfilesFromDisk();
                            //ResetComboBox();
                        }
                        else
                        {
                            fillListBoxProfilesFromDisk();
                            MessageBox.Show($"Zmieniono nazwę katalogu z profilem na {newProfilePath}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Nie znaleziono profilu na dysku na komputerze {pcName}.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano Profilu z listy", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas próby dostępu do dysku na komputerze {pcName}: {ex.Message}", "Informacja",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //}
            //else
            // {
            //    Console.WriteLine($"Stacja {pcName} jest obecnie wyłączona");
            //   ShowMessageBox($"Stacja {pcName} jest obecnie wyłączona", "Informacja", MessageBoxIcon.Information);
            // }


        }

        private void DeleteProfilFromDisk()
        { 
        }

        private void RestoreProfileOnDisk()
        { 
        }

        private void restoreDataOnDisk()
        { 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lbRegistersProfiles.SelectedItems.Count == 1)  lbRegistersProfiles.Enabled = false;    
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartPc(computerName);
        }

        private void btnCopyProfilOnDisk_Click(object sender, EventArgs e)
        {
            BackupProfileOnDisk(computerName);
        }

        private void btnDeleteProfileFromRegister_Click(object sender, EventArgs e)
        {
            DeleteProfileFromRegister(computerName);
        }

        private void btnDeleteProfileFromRegister_tabPrzywracanie_Click(object sender, EventArgs e)
        {
            DeleteProfileFromRegister(computerName);
        }


    }
}
