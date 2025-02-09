using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
            DeleteProfileFromRegister(computerName);
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
            lbRegistersProfiles.Items.Clear();                      // Czyszczenie ListBox przed załadowaniem nowych danych
            lbRegistersProfiles.Items.AddRange(profilesFromRegister.ToArray()); // Dodawanie profili do ListBox
        }

        private void fillListBoxProfilesFromDisk()
        {
            List<string> profilesFromDisk = ReadProfilesFromDisk(computerName);
            lbDisksProfiles.Items.Clear();              // Czyszczenie ListBox przed załadowaniem nowych danych
            lbDisksProfiles.Items.AddRange(profilesFromDisk.ToArray());              // Dodawanie profili do ListBox
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
                    return ;
                }


                if (Directory.Exists(oldProfilePath))
                {
                    // Zmień nazwę katalogu
                    Directory.Move(oldProfilePath, newProfilePath);

                    if (Directory.Exists(oldProfilePath))
                    {
                        fillListBoxProfilesFromDisk();
                        MessageBox.Show("Przerwano operację przywracania profilu", "Informacja", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        return ;
                    }

                    if (Directory.Exists(newProfilePath))
                    {
                        fillListBoxProfilesFromDisk();
                        MessageBox.Show($"Zmieniono nazwę katalogu z profilem na {newProfilePath}", "Informacja", MessageBoxButtons.OK,MessageBoxIcon.Information);
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


    }
}
