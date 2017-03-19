using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public static class ExtentionMethods
{
    public static void Shuffle<T>(this IList<T> list)
    {
        //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        System.Random rand = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector2 Copy(this Vector2 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 Copy(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static Quaternion Copy(this Quaternion quaternion)
    {
        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static Toggle GetActive(this ToggleGroup toggleGroup)
    {
        return toggleGroup.ActiveToggles().FirstOrDefault();
    }


    public static DirectoryInfo FindOrCreateDirectory(DirectoryInfo rootDirectory, string name)
    {
        if (rootDirectory == null)
            return null;

        //Check if that folder already exists
        DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
        DirectoryInfo ourDirectory = null;

        foreach (DirectoryInfo directory in subDirectories)
        {
            if (directory.Name == name)
            {
                ourDirectory = directory;
                break;
            }
        }

        //If not, create it.
        if (ourDirectory == null)
        {
            ourDirectory = rootDirectory.CreateSubdirectory(name);
        }

        return ourDirectory;
    }

    public static string FindUniqueDirectoryName(DirectoryInfo rootDirectory, string originalDirectoryName)
    {
        if (rootDirectory == null)
            return "";

        string uniqueFileName = originalDirectoryName;

        int count = 0;
        DirectoryInfo[] directories = rootDirectory.GetDirectories();
        for (int i = 0; i < directories.Length; ++i)
        {
            if (directories[i].Name.StartsWith(originalDirectoryName))
            {
                string testFilename = originalDirectoryName;
                if (count > 0) { testFilename += " (" + (count + 1) + ")"; }

                if (directories[i].Name != testFilename)
                {
                    uniqueFileName = testFilename;
                    break;
                }

                ++count;

                uniqueFileName = originalDirectoryName + " (" + (count + 1) + ")";
            }
        }

        return uniqueFileName;
    }
}
