using System;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary; // Бібліотеку для роботи з ярликами  

class RegistryBackup
{
    static void Main(string[] args)
    {
        try
        {
            // Визначаємо шлях до резервної копії реєстру, розташування -- робочий стіл  
            string backupFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RegistryBackup.reg");

            // Відображаємо інформацію про частину реєстру, яку будемо копіювати  
            Console.WriteLine("Виконується резервне копіювання наступної частини реєстру:");
            Console.WriteLine(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            Console.WriteLine($"Дані будуть збережені у файл: {backupFilePath}");

            // Створюємо новий процес для запуску утиліти REG  
            Process process = new Process();
            process.StartInfo.FileName = "reg.exe"; // Вказуємо ім'я програми  
            // Формулюємо аргументи для команди REG  
            process.StartInfo.Arguments = $"export \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\" \"{backupFilePath}\" /y";
            process.StartInfo.UseShellExecute = false; // Не використовуємо оболонку Windows  
            process.StartInfo.RedirectStandardOutput = true; // Перенаправляємо стандартний вихід (вихідні дані)  
            process.StartInfo.RedirectStandardError = true; // Перенаправляємо стандартний вихід помилок  
            process.StartInfo.CreateNoWindow = true; // Не створюємо нове вікно  

            // Запускаємо процес  
            process.Start();

            // Читаємо вихідні дані та помилки  
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Чекаємо, поки процес завершиться  
            process.WaitForExit();

            // Перевіряємо код виходу процесу  
            if (process.ExitCode == 0)
            {
                Console.WriteLine("Резервне копіювання успішно завершено!");
            }
            else
            {
                Console.WriteLine("Сталася помилка під час резервного копіювання:");
                Console.WriteLine(error);
            }
        }
        catch (Exception ex)
        {
            // Обробка виняткових ситуацій (наприклад, доступу до реєстру)  
            Console.WriteLine("Виникла помилка: " + ex.Message);
        }

        // Створюємо ярлик для запуску цього сценарію на робочому столі  
        CreateShortcut();
    }

    static void CreateShortcut()
    {
        try
        {
            // Визначаємо розташування для створення ярлика на робочому столі  
            string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RegistryBackup.lnk");

            // Створюємо об'єкт WshShell для роботи з ярликами  
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location; // Шлях до виконуваного файлу  
            shortcut.Description = "Запустити резервне копіювання реєстру"; // Опис ярлика  
            shortcut.Save(); // Зберігаємо ярлик  

            // Виводимо повідомлення про успішне створення ярлика  
            Console.WriteLine($"Ярлик створено на робочому столі: {shortcutLocation}");
        }
        catch (Exception ex)
        {
            // Обробка виняткових ситуацій при створенні ярлика  
            Console.WriteLine("Не вдалося створити ярлик: " + ex.Message);
        }
        Console.ReadKey();
    }
}