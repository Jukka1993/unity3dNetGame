using System;
using System.Collections.Generic;
using System.Text;


public static class CommonUtil
{
    //public static void Log(string str)
    //{
    //    if (Constant.showLog)
    //    {
    //        Console.WriteLine(str);
    //    }
    //}
    public static void Log(params string[] strs)
    {
        if (Constant.showLog)
        {
            Console.WriteLine(string.Join("",strs));
        }
    }
    
    public static bool IsFilterProto(string protoName)
    {
        if (Constant.filterAllProto)
        {
            return true;
        }
        //todo 是否可以用哈希表优化一下,如果后面的协议相当的多的话
        foreach(string name in Constant.filterProtoNames)
        {
            if (name == protoName)
            {
                return true;
            }
        }
        return false;
    }
}

