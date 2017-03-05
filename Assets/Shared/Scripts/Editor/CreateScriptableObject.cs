using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class CreateScriptableObject
{
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

    [MenuItem("Assets/Create/Custom/Weapon Impact Effect")]
    public static void CreateWeaponImpactEffect()
    {
        CreateAsset<WeaponImpactEffectDefinition>();
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