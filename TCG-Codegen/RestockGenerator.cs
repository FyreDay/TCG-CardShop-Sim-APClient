using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCG_Codegen;

public class RestockGenerator
{
    static void PrintEnumList(AssetTypeValueField listField, string name)
    {
        var array = listField["Array"];

        int count = array["size"].AsInt;

        Console.WriteLine($"\n{name} ({count})");

        for (int i = 0; i < count; i++)
        {
            int enumValue = array["data"][i].AsInt;
            Console.WriteLine($"  {enumValue}");
        }
    }
}
