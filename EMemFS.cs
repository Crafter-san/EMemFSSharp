using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

public class EMemFS
{
    public abstract class EFSData
    {
        public string name { get; set; }
        public string data { get; set; }
        public byte[] bytes { get; set; } = { };
        public Dictionary<string, string> metadata { get; set; } = new Dictionary<string, string>();
        public DateTime Timestamp { get; init; }
        public EFSData()
        {
            Timestamp = DateTime.Now;
        }
    }
    public class Manage<EFSObj> where EFSObj : EFSData
    {
        public List<string> Dir { get; init; } = new();
        public Dictionary<string, EFSObj> List { get; init; } = new();
        public EFSObj Get(string key)
        {
            if (!Dir.Contains(key)) return null;
            return List[key];
        }
        public EFSObj Set(EFSObj obj)
        {
            if (!Dir.Contains(obj.name)) Dir.Add(obj.name);
            List[obj.name] = obj;
            return obj;
        }
        public EFSObj Destroy(EFSObj obj)
        {
            List.Remove(obj.name);
            Dir.Remove(obj.name);
            return obj;
        }
    }
    public class File : EFSData
    {
        public List<string> Lines { get; init; } = new() { "" };
        public File(string name = "Default Filename", string data = "")
        {

            this.name = name;
            this.data = data;
        }
        public string Append(string line, bool newline = false)
        {
            if (newline)
            {
                Lines.Add(line);
                line = $"\n{line}";
            }
            else Lines[^1] += line;
            data += line;
            return Lines[^1];
        }
    }
    public class Folder : EFSData
    {
        public Manage<File> Files { get; init; }
        public Manage<Folder> Folders { get; init; }
        public Folder(string name, Manage<File> files = null, Manage<Folder> folders = null)
        {
            Files = files ?? new();
            Folders = folders ?? new();
            this.name = name;
        }
    }
    public class User : EFSData
    {
        public Manage<File> Files { get; init; }
        public Manage<Folder> Folders { get; init; }
        public Manage<User> Users { get; init; }
        public User(string name, Manage<File> files = null, Manage<Folder> folders = null, Manage<User> users = null)
        {
            Files = files ?? new();
            Folders = folders ?? new();
            Users = users ?? new();
            this.name = name;
        }

    }
    public class Drive : EFSData
    {
        public Manage<File> Files { get; init; }
        public Manage<Folder> Folders { get; init; }
        public Manage<User> Users { get; init; }
        public Manage<Drive> Drives { get; init; }
        public Drive(string name, Manage<File> files = null, Manage<Folder> folders = null, Manage<User> users = null, Manage<Drive> drives = null)
        {
            Files = files ?? new();
            Folders = folders ?? new();
            Users = users ?? new();
            Drives = drives ?? new();
            this.name = name;

        }
    }

    public class Device : EFSData
    {
        public Manage<File> Files { get; init; }
        public Manage<Folder> Folders { get; init; }
        public Manage<User> Users { get; init; }
        public Manage<Drive> Drives { get; init; }
        public Manage<Device> Devices { get; init; }
        public Device(string name, Manage<File> files = null, Manage<Folder> folders = null, Manage<User> users = null, Manage<Drive> drives = null, Manage<Device> devices = null)
        {
            Files = files ?? new();
            Folders = folders ?? new();
            Users = users ?? new();
            Drives = drives ?? new();
            Devices = devices ?? new();
            this.name = name;
        }
    }

    public class DeviceManager
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };
        public static void Save(string filename, Device device)
        {

            string jsonString = JsonSerializer.Serialize<Device>(device, options);
            using var streamWriter = System.IO.File.CreateText(filename);
            streamWriter.Write(jsonString);
            streamWriter.Close();
        }
        public static Device Load(string filename)
        {

            using StreamReader streamReader = new(filename);
            Device? device_instance =
                    JsonSerializer.Deserialize<Device>(streamReader.ReadToEnd(), options);
            streamReader.Close();
            return device_instance;
        }
    }
}
