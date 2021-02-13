using System;
using Microsoft.Win32;

namespace Octarine {
public class Config {
public static string ReadConfig(string value, string subkey=null) {
string path=@"SOFTWARE\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.OpenSubKey(path)) {
if(key==null) return null;
return (String)key.GetValue(value);
}
}

public static void WriteConfig(string value, string data, string subkey=null) {
string path=@"SOFTWARE\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.CreateSubKey(path)) {
key.SetValue(value, data);
}
}
}
}