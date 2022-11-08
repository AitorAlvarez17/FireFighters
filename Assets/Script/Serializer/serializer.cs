using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class serializer
{
    static MemoryStream stream;
    // Start is called before the first frame update
    

    public class testClass
    {
        public int hp = 12;
        public List<int> pos = new List<int> { 3, 3, 3 };
    }

    static void serializeJson()
    {
        var t = new testClass();
        t.hp = 40;
        t.pos = new List<int> { 10, 3, 12 };
        string json = JsonUtility.ToJson(t);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
    }
    static void deserializeJson()
    {
        var t = new testClass();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string json = reader.ReadString();
        Debug.Log(json);
        t = JsonUtility.FromJson<testClass>(json);
        Debug.Log(t.hp.ToString() + " " + t.pos.ToString());
    }

    static void serializeXML()
    {
        var t = new testClass();
        t.hp = 40;
        t.pos = new List<int> { 10, 3, 12 };
        XmlSerializer serializer = new XmlSerializer(typeof(testClass));
        stream = new MemoryStream();
        serializer.Serialize(stream, t);
        bytes = stream.ToArray();

    }
    static void deserializeXML()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(testClass));
        var t = new testClass();
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        t = (testClass)serializer.Deserialize(stream);
        Debug.Log("Xml " + t.hp.ToString() + " " + t.pos.ToString());
    }
    static byte[] bytes;
    static void serialize()
    {
        double myfloat = 100f;
        int myint = 15;
        string mystring = "test";
        int[] mylist = new int[3] { 1, 2, 4 };
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(myfloat);
        writer.Write(myint);
        writer.Write(mystring);
        foreach (var i in mylist)
        {
            writer.Write(i);
        }
        Debug.Log("serialized!");
        bytes = stream.ToArray();
    }

    public static byte[] SerializeInfo(string message)
    {
        string mystring = message;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(mystring);
        bytes = stream.ToArray();
        Debug.Log("Serialized Message!");
        return bytes;
    }

    public static byte[] SerializeMessage(Message _message)
    {

        string message = _message.message;
        string username = _message.username;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        //Header
        writer.Write("/>Message:");
        //Info
        writer.Write(message);
        writer.Write(username);
        bytes = stream.ToArray();
        Debug.Log("Serialized Message!");
        return bytes;
    }

    public static string DeserializeInfo(byte[] bytes)
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string newstring = reader.ReadString();

        return newstring;
    }

    public static Message DeserializeMessage(byte[] bytes)
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        //Header
        string header = reader.ReadString();
        Debug.Log("Deserialize(): Header is " + header);
        //Info
        string message = reader.ReadString();
        string username = reader.ReadString();

        Message newMessage = new Message(message, username);

        bytes = null;

        return newMessage;
    }

    static void deserialize()
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        double newfloat = reader.ReadDouble();

        Debug.Log("float " + newfloat.ToString());

        int newint = reader.ReadInt32();
        Debug.Log("int " + newint.ToString());
        string newstring = reader.ReadString();
        Debug.Log("string " + newstring.ToString());
        int[] newlist = new int[3];
        for (int i = 0; i < newlist.Length; i++)
        {
            newlist[i] = reader.ReadInt32();
        }
        Debug.Log(newlist.ToString());


    }

    public static byte[] SerializePlayerInfo(float[] positions)
    {
        float[] mylist = positions;
        
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        //Header
        writer.Write("/>PlayerInfo:");
        //Info
        foreach (var i in mylist)
        {
            writer.Write(i);
        }
        bytes = stream.ToArray();
        Debug.Log("Serialized Info!");
        return bytes;
    }

    public static float[] DeserializePlayerInfo(byte[] bytes)
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        //Header
        string header = reader.ReadString();
        Debug.Log("Deserialize(): Header is " + header);
        //Info
        float[] newlist = new float[3];
        for (int i = 0; i < newlist.Length; i++)
        {
            newlist[i] = reader.ReadInt32();
        }

        bytes = null;

        return newlist;
    }

}