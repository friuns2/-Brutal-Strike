using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;


public class RegistryUtilities
{
    public static bool RenameSubKey(RegistryKey parentKey, string subKeyName, RegistryKey newSubKeyName)
    {
        CopyKey(parentKey, subKeyName, newSubKeyName);
        parentKey.DeleteSubKeyTree(subKeyName);
        return true;
    }

    public static bool CopyKey(RegistryKey parentKey, string name, RegistryKey dest)
    {
        RegistryKey destinationKey = dest.CreateSubKey(name, RegistryKeyPermissionCheck.ReadWriteSubTree);
        RegistryKey sourceKey = parentKey.OpenSubKey(name);
        RecurseCopyKey(sourceKey, destinationKey);
        return true;
    }

    private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
    {
        foreach (string valueName in sourceKey.GetValueNames())
        {
            object objValue = sourceKey.GetValue(valueName);
            RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
            destinationKey.SetValue(valueName, objValue, valKind);
        }
        foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
        {
            RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
            RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
            RecurseCopyKey(sourceSubKey, destSubKey);
        }
    }
}
