using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ControlBackup
{
    enum tParamData { tBoll, tString }
    class cParamData
    {

        public string command;
        public tParamData type;
        public string data;
        public string descript;

        public cParamData(string lCommand, tParamData ltype, string defData, string lDescript = "")
        {
            command = lCommand;
            type = ltype;
            data = defData;
            descript = lDescript;
        }
    }

    class cParam : IEnumerable, IEnumerator
    {
        private Dictionary<string, cParamData> paramList;
        int index = -1;

        public cParam()
        {
            paramList = new Dictionary<string, cParamData>();
        }

        public void Add(string lCommand, tParamData ltype, string defData, string lDescript = "")
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
                        if (value.type == tParamData.tBoll)
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

        // Реализуем интерфейс IEnumerable
        public IEnumerator GetEnumerator()
        {
            return this;
        }

        // Реализуем интерфейс IEnumerator
        public bool MoveNext()
        {
            if (index == paramList.Count() - 1)
            {
                Reset();
                return false;
            }

            index++;
            return true;
        }

        public void Reset()
        {
            index = -1;
        }

        public object Current
        {
            get
            {
                return paramList.ElementAt(index).Value;
            }
        }
    }
}
