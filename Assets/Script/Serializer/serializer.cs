using System;
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

    static byte[] bytes;
    public class testClass
    {
        public int hp = 12;
        public List<int> pos = new List<int> { 3, 3, 3 };
    }

    public static byte[] SerializePackage(PlayerPackage _message)
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
        foreach (float coordinate in _message.positions)
        {
            writer.Write(coordinate);
        }
        writer.Write(_message.id);
        foreach (var worldPlayer in _message.worldMatrix)
        {
            //id's
            writer.Write(worldPlayer.Item1);
            //life
            writer.Write(worldPlayer.Item2);
        }
        writer.Write(_message.playersOnline);
        //FIRE STUFF
        writer.Write(_message.fireAction);
        writer.Write(_message.amount);
        writer.Write(_message.fireID);
        writer.Write(_message.fireLife);
        bytes = stream.ToArray();
        return bytes;
    }
    public static PlayerPackage DeserializePackage(byte[] bytes)
    {
        MemoryStream stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        //Header
        string header = reader.ReadString();

        //Info
        string message = reader.ReadString();
        string username = reader.ReadString();
        float[] positions = new float[3];
        positions[0] = reader.ReadSingle();
        positions[1] = reader.ReadSingle();
        positions[2] = reader.ReadSingle();

        //foreach (var item in positions)
        //{
            //Debug.Log("Position :" + item);
        //}
        int id = reader.ReadInt32();
        Tuple<int,int>[] worldMatrix = new Tuple<int,int>[4];
        worldMatrix[0] = Tuple.Create(reader.ReadInt32(), reader.ReadInt32());
        worldMatrix[1] = Tuple.Create(reader.ReadInt32(), reader.ReadInt32());
        worldMatrix[2] = Tuple.Create(reader.ReadInt32(), reader.ReadInt32());
        worldMatrix[3] = Tuple.Create(reader.ReadInt32(), reader.ReadInt32());

        int playersOnline = reader.ReadInt32();

        int fireaction = reader.ReadInt32();
        int amount = reader.ReadInt32();
        int fireID = reader.ReadInt32();
        int fireLife = reader.ReadInt32();

        //foreach (var item in positions)
        //{
            //Debug.Log("Position :" + item);
        //}

        PlayerPackage newMessage = new PlayerPackage(message, username,positions,id, worldMatrix, playersOnline, fireaction, amount, fireID, fireLife);

        bytes = null;

        return newMessage;
    }

    #region 
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



    public static string DeserializeInfo(byte[] bytes)
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string newstring = reader.ReadString();

        return newstring;
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

    public static byte[] SerializePlayerInfo(PlayerInfo info)
    {
        var t = info;
        Debug.Log(t.message.message);
        // json only parses public elements
        string json = JsonUtility.ToJson(t);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);

        Debug.Log(json.ToString());

        bytes = stream.ToArray();
        return bytes;
    }

    public static PlayerInfo DeserializePlayerInfo(byte[] bytes)
    {
        stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);

        var t = new PlayerInfo(null);

        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        string json = reader.ReadString();
        Debug.Log(json);
        t = JsonUtility.FromJson<PlayerInfo>(json);
        Debug.Log(t.message.message.ToString() + " " + t.message.username.ToString());

        return t;
    }

    #endregion 
}