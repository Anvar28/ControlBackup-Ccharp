using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum tParamData { tboll, tstring }
class cParamData
{

    public string command;
    public tParamData type;
    public string data;
    public string descript;

    public cParamData(string lCommand, tParamData ltype = tParamData.tstring, string defData = "", string lDescript = "")
    {
        command = lCommand;
        type = ltype;
        data = defData;
        descript = lDescript;
    }
}

class cParam : IEnumerable
{
    private Dictionary<string, cParamData> paramList;

    void Param()
    {
        paramList = new Dictionary<string, cParamData>();
    }

    public void Add(string lCommand, tParamData ltype = tParamData.tstring, string defData = "", string lDescript = "")
    {
        paramList.Add(lCommand, new cParamData(lCommand, ltype, defData, lDescript));
    }

    public cParamData Get(string lCommand)
    {
        return paramList[lCommand];
    }

    public void Parse(string[] args)
    {
        // Заполним параметры из командной строки
        int count = args.Count();
        for (int i = 0; i < paramList.Count(); i++)
        {
            var itemParam = paramList.ElementAt(i);
            string key = itemParam.Key;
            cParamData value = itemParam.Value;
            for (int j = 0; j < count; j++)
            {
                if (key == args[j])
                {
                    if (value.type == tParamData.tboll)
                    {
                        value.data = "True";
                    }
                    else
                    {
                        // Следущий должно идти значение параметра
                        if (j + 1 < count)
                        {
                            value.data = args[j + 1];
                        }
                    }
                }
            }
        }
    }
}

internal class Program
{    

    static void Main(string[] args)
    {

        cParam param = new cParam();
        param.Add("-f", tParamData.tboll, "", "-fileproperty");
        param.Add("-s", tParamData.tstring, "", "-smtpserver");
        param.Add("-u", tParamData.tboll, "", "-username");
        param.Add("-p", tParamData.tboll, "", "-password");
        param.Add("-t", tParamData.tboll, "", "-mailto");
        param.Add("-su", tParamData.tboll, "", "-subject");
        param.Add("-pb", tParamData.tboll, "", "-pathbackup");
        param.Parse(args);

        foreach (var item in param)
        {
            Console.WriteLine(item.Key + "    " + item.Value.data);
        }
        Console.ReadLine();
    }
}
