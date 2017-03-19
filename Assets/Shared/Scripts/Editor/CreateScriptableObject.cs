using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class CreateScriptableObject
{
    //TODO: Find a better place to put these
    [MenuItem("Assets/Create/Custom/LevelData")]
    public static void CreateLevelData()
    {
        CreateAsset<LevelDataDefinition>();
    }

    [MenuItem("Assets/Create/Custom/LevelDataList")]
    public static void CreateLevelListData()
    {
        CreateAsset<LevelDataListDefinition>();
    }

    [MenuItem("Assets/Create/Custom/DifficultyMode")]
    public static void CreateDifficultyMode()
    {
        CreateAsset<DifficultyModeDefinition>();
    }

    [MenuItem("Assets/Create/Custom/DifficultyModeList")]
    public static void CreateDifficultyModeData()
    {
        CreateAsset<DifficultyModeListDefinition>();
    }

    [MenuItem("Assets/Create/Custom/SurfaceType")]
    public static void CreateSurfaceType()
    {
        CreateAsset<SurfaceTypeDefinition>();
    }

    [MenuItem("Assets/Create/Custom/AmmoType")]
    public static void CreateAmmoType()
    {
        CreateAsset<AmmoTypeDefinition>();
    }

    [MenuItem("Assets/Create/Custom/Impact Effect")]
    public static void CreateImpactEffect()
    {
        CreateAsset<ImpactEffectDefinition>();
    }

    [MenuItem("Assets/Create/Custom/Bullet Shell")]
    public static void CreateBulletShell()
    {
        CreateAsset<BulletShellDefinition>();
    }

    [MenuItem("Assets/Create/Custom/Faction Type")]
    public static void CreateFactionType()
    {
        CreateAsset<FactionTypeDefinition>();
    }

    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}