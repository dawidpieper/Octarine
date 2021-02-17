using System;
using System.IO;
using Microsoft.Win32;

namespace Octarine {
public class Config {
public static string ReadConfig(string value, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.OpenSubKey(path)) {
if(key==null) return null;
return (String)key.GetValue(value);
}
}

public static void WriteConfig(string value, string data, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.CreateSubKey(path)) {
key.SetValue(value, data);
}
}

public static int ReadConfigInt(string value, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.OpenSubKey(path)) {
if(key==null) return 0;
if(key.GetValue(value)==null) return 0;
return (int)key.GetValue(value);
}
}

public static void WriteConfig(string value, int data, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.CreateSubKey(path)) {
key.SetValue(value, data);
}
}

public static byte[] ReadConfigBinary(string value, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.OpenSubKey(path)) {
if(key==null) return null;
if(key.GetValue(value)==null) return null;
return (byte[])key.GetValue(value);
}
}

public static void WriteConfig(string value, byte[] data, string subkey=null) {
string path="SOFTWARE\\Octarine";
if(subkey!=null) path+=@"\"+subkey;
using(RegistryKey key = Registry.CurrentUser.CreateSubKey(path)) {
key.SetValue(value, data);
}
}

public static string GetDataDir(string subdir=null) {
string basedir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData )+"\\Octarine";
string dir=basedir;
if(subdir!=null) dir+="\\"+subdir;
Directory.CreateDirectory(dir);
return dir;
}
}
}