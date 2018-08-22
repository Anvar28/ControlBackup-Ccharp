# BackupControl
Программа для проверки создания архивов.
По переданным параметрам проверяет в каталоге архивов
1. Когда был создан последний архив, если дата меньше текущего дня то оправляет письмо
2. Проверяет размер архива по сравнению с прошлым архивом,  если размер меньше, то опправляет письмо
3. Проверяет существует ли каталог архива, если нет, то оправляет письмо
4. Раз в неделю отправляет письмо для проверки связи

Параметры запуска:

-f FILEPROPERTY, --fileproperty FILEPROPERTY  
Если заполнен данный параметр, то все остальные данные берутся из данного файла.

-s SMTPSERVER, --smtpserver SMTPSERVER
SMTP сервер, по умолчанию smtp.yandex.ru:465

-u USERNAME, --username USERNAME
Логин пользователя почты

-p PASSWORD, --password PASSWORD
Пароль пользователя почты

-t MAILTO, --mailto MAILTO
Куда отсылать информацию

-su SUBJECT, --subject SUBJECT
Тема письма

-pb PATHBACKUP, --pathbackup PATHBACKUP
Путь к каталогу где лежат архивы

-------------------------------  

Примеры запуска

Из командной строки:
D:\1C\Свои\backupControl>backupcontrol.exe -u test@yandex.ru -p pas$1 -t apxi2@yandex.ru -su Проверка архивов -pb E:\arhiv\

С помощью отдельного файла настроек:
D:\1C\Свои\backupControl>backupcontrol.exe -f test.ini

Файл test.ini
[settings]
smtpserver = smtp.yandex.ru:25
username = test@yandex.ru
password = pas
mailto = apxi2@yandex.ru
subject = Проверка архивов
pathbackup = E:\arhiv\
