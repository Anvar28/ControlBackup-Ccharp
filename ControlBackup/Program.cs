using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ControlBackup;
using System.Net.Mail;
using System.Net;
using System.Data;

class tProgramData
{
    public string smtpServer;
    public string userName;
    public string password;
    public string mailto;
    public string subject;
    public string pathBackup;
}

internal class Program
{
    const string strErrorNotFoundPath = "Не найден каталог где лежат архивы.";
    const string strInfoNotArchiv = "Каталог с архивами пустой";
    const string strInfoOldArсhiv = "Последний архив был сделан {0}, за сегодняшний день архивов нет.";
    const string strInfoSizeNewArchivSmaller = "Размер {0} нового архива {1} \r меньше чем размер {3} нового архива {4}";

    // Загрузка данные из ini файла 
    static void loadFromIni(string[] args, cParam param, tProgramData programData)
    {
        IniFiles ini = new IniFiles("param.ini");
        string iniSection = "Основные настройки";
        string iniSmtpServer = "smtpserver";
        string iniUsername = "username";
        string iniPassword = "password";
        string iniMailto = "mailto";
        string iniSubject = "subject";
        string iniPathbackup = "pathbackup";

        if (args.Count() == 0)
        {
            // Создадим INI файл с настройками
            ini.Write(iniSection, iniSmtpServer, "");
            ini.Write(iniSection, iniUsername, "");
            ini.Write(iniSection, iniPassword, "");
            ini.Write(iniSection, iniMailto, "");
            ini.Write(iniSection, iniSubject, "");
            ini.Write(iniSection, iniPathbackup, "");

            Console.WriteLine("17/08/2018 apxi2@yandex.ru");
            Console.WriteLine("Программа контроля создания архивов.");
            Console.WriteLine("При запуске с параметрами контролирует каталог из параметра -pb");
            Console.WriteLine("Если в каталоге сегодня не было свежего файла, значит архив не сделан");
            Console.WriteLine("отправляет письмо на почту. Также если размер архива меньше чем прошлый");
            Console.WriteLine("то также отправляет письмо на почту.");
            Console.WriteLine("Парамтеры: ");
            foreach (cParamData item in param)
            {
                Console.WriteLine("\t{0}  {1}", item.command, item.descript);
            }

        }

        // Если в параметрах указано что брать данные из файла настроек, то
        // попробует получить из него данные

        if (param.Get("-f").data != "")
        {
            // читаем данные из файла
            programData.smtpServer = ini.Read(iniSection, iniSmtpServer);
            programData.userName = ini.Read(iniSection, iniUsername);
            programData.password = ini.Read(iniSection, iniPassword);
            programData.mailto = ini.Read(iniSection, iniMailto);
            programData.subject = ini.Read(iniSection, iniSubject);
            programData.pathBackup = ini.Read(iniSection, iniPathbackup);
        }
    }

    static void ParseParam(string[] args, cParam param, tProgramData programData)
    {
        param.Add("-f", tParamData.tBoll, "", "Если указан, то настройки берутся из param.ini файла. Файл с пустыми настройками создан.");
        param.Add("-s", tParamData.tString, "smtp.yandex.ru:25", "адрес smtp сервера, например smtp.yandex.ru");
        param.Add("-u", tParamData.tString, "", "почта под который делать отправку писем");
        param.Add("-p", tParamData.tString, "", "пароль");
        param.Add("-m", tParamData.tString, "", "адрес почты на который нужно отправлять письма, например apxi2@yandex.ru");
        param.Add("-su", tParamData.tString, "", "тема письма");
        param.Add("-pb", tParamData.tString, "", "контролируемый каталог с архивами");
        param.Parse(args);

        programData.smtpServer = param.Get("-s").data;
        programData.userName = param.Get("-u").data;
        programData.password = param.Get("-p").data;
        programData.mailto = param.Get("-m").data;
        programData.subject = param.Get("-su").data;
        programData.pathBackup = param.Get("-pb").data;

        loadFromIni(args, param, programData);
    }

    static void sendMail(tProgramData programData, string body)
    {

        if ((programData.smtpServer != "") && (programData.userName != "") && (programData.password != "")) {
            using (MailMessage mm = new MailMessage(programData.userName, programData.mailto))
            {
                mm.Subject = programData.subject;
                mm.Body = body;
                mm.IsBodyHtml = false;

                string[] w = programData.smtpServer.Split(new char[] { ':' });
                int port = 0;
                if (w.Count() > 1)
                {
                    int.TryParse(w[1], out port);
                }

                SmtpClient sc = new SmtpClient(w[0], port);
                sc.EnableSsl = true;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.UseDefaultCredentials = false;
                sc.Credentials = new NetworkCredential(programData.userName, programData.password);
                sc.Send(mm);
            }
        }
    }

    class tDataFile{
        public string name;
        public DateTime dateCreate;

        public tDataFile(string lName, DateTime lDateCreate)
        {
            name = lName;
            dateCreate = lDateCreate;
        }
    }

    static void Main(string[] args)
    {

        tProgramData programData = new tProgramData();
        cParam param = new cParam();

        ParseParam(args, param, programData);

        // Проверяем путь
        if (!Directory.Exists(programData.pathBackup))
        {
            sendMail(programData, strErrorNotFoundPath);
        }
        else
        {
            List<tDataFile> fileList = new List<tDataFile>();

            string[] files = Directory.GetFiles(programData.pathBackup).OrderByDescending(x => new FileInfo(x).CreationTime).ToArray();
            foreach (string file in files)
            {
                fileList.Add(new tDataFile(file, Directory.GetCreationTime(file)));
            }

            int c = fileList.Count();
            if (c == 0)
            {
                // Нет файлов в контролируемом каталоге
                sendMail(programData, strInfoNotArchiv);
            }
            else
            {
                tDataFile dataFile1 = fileList[0];

                if (dataFile1.dateCreate <= DateTime.Today)
                {
                    // Последний архив меньше сегодняшенго дня
                    sendMail(programData, string.Format(strInfoOldArсhiv, dataFile1.dateCreate));
                }
                else
                {
                    if (c > 1)
                    {
                        tDataFile dataFile2 = fileList[1];
                        long lenFile1 = new FileInfo(dataFile1.name).Length;
                        long lenFile2 = new FileInfo(dataFile2.name).Length;
                        if (lenFile1 < lenFile2)
                        {
                            // Размер нового архива меньше чем старого
                            sendMail(programData, string.Format(strInfoSizeNewArchivSmaller, lenFile1, dataFile1.name, lenFile2, dataFile2.name));
                        }
                    }
                }
            }
        }
    }
}
