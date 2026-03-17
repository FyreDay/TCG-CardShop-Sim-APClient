using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Reflection;

class Program
{
    static List<int> GetIntList(AssetTypeValueField listField)
    {
        var intList = new List<int>();
        var array = listField["Array"];

        if (array.IsDummy) return intList;

        for (int i = 0; i < array.Children.Count; i++)
        {
            intList.Add(array[i].AsInt);
        }
        return intList;
    }
    static void Main()
    {
        Console.WriteLine("Hello, World!");
#if GAME_PATH
        // Retrieve the string value we passed via AssemblyMetadata
        var attr = typeof(Program).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "GamePath");

        string gamePathValue = attr?.Value ?? "Path defined but value missing";
        Console.WriteLine($"Game Path: {gamePathValue}");
#else
        return;
#endif

        string classdataPath = @"assets\lz4.tpk"; // from AssetsTools.NET repo

        var manager = new AssetsManager();

        manager.LoadClassPackage(classdataPath);
        string assetsPath = Path.Combine(gamePathValue, "Card Shop Simulator_Data", "sharedassets1.assets");
        var inst = manager.LoadAssetsFile(assetsPath, false);
        string managedPath = Path.Combine(gamePathValue, "Card Shop Simulator_Data", "Managed");
        manager.MonoTempGenerator = new MonoCecilTempGenerator(managedPath);

        Console.WriteLine(assetsPath);
        manager.LoadClassDatabaseFromPackage(inst.file.Metadata.UnityVersion);

        // 1. Target the specific asset by PathID
        AssetFileInfo info = inst.file.GetAssetInfo(78160);

        if (info != null)
        {
            // 2. Deserializing the asset into a readable field tree
            // We pass 'inst' so the manager knows which file context to use
            var baseField = manager.GetBaseField(inst, info);

            if (baseField != null)
            {
                Console.WriteLine("Successfully read the asset data!");

                // 3. Accessing the fields (Example based on StockItemData)
                // Note: Field names are case-sensitive and often match the C# class variables
                string itemName = baseField["m_Name"].AsString;
                Console.WriteLine($"Asset Name: {itemName}");

                // If you want to see ALL field names available in this asset:
                foreach (var child in baseField.Children)
                {
                    Console.WriteLine($"Field found: {child.FieldName} (Type: {child.TypeName})");
                }
                var shownAllItems = GetIntList(baseField["m_ShownAllItemType"]);
                Console.WriteLine($"Found {shownAllItems.Count} items in m_ShownAllItemType:");
                foreach (int val in shownAllItems)
                {
                    Console.WriteLine($" - Enum Int Value: {val}");
                }
            }
            else
            {
                Console.WriteLine("Failed to create BaseField. The .tpk might not support this specific class.");
            }
        }
        else
        {
            Console.WriteLine("PathID 78160 not found in sharedassets1.assets.");
        }

        //var rsrcInfo = ggm.file.GetAssetsOfType(AssetClassID.ResourceManager)[0];
        //var rsrcBf = am.GetBaseField(ggm, rsrcInfo);

        //var m_Container = rsrcBf["m_Container.Array"];

        //foreach (var data in m_Container.Children)
        //{
        //    var name = data[0].AsString;
        //    var pathId = data[1]["m_PathID"].AsLong;

        //    Console.WriteLine($"in resources.assets, pathid {pathId} = {name}");
        //}

        //manager.MonoTempGenerator = new MonoCecilTempGenerator("path/to/game/managed/folder");

        //// PathID you found
        //long pathId = 78160;

        //var assetFile = inst.file;
        //var assetInfo = assetFile.GetAssetInfo(pathId);

        //var baseField = manager.GetBaseField(inst, assetInfo);

        
        //PrintEnumList(baseField["m_ShownItemType"], "Shown Item Types");
        //PrintEnumList(baseField["m_ShownAccessoryItemType"], "Accessory Types");
        //PrintEnumList(baseField["m_ShownFigurineItemType"], "Figurine Types");
        //PrintEnumList(baseField["m_ShownBoardGameItemType"], "Board Game Types");
        //PrintEnumList(baseField["m_CardPackItemTypeList"], "Card Pack Types");
    }
}