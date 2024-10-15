using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;

public class DictionarySerial : MonoBehaviour
{
    private Dictionary<string, int> Dictionary = new Dictionary<string, int>();

    private void Start()
    {
        var character = new SampleJsonCharacter()
        {
            characterLevel = 1,
            characterName = "Jhan",
            chosenClass = CharacterClass.Assassin,
            dateCreated = DateTime.Now

        };

        Debug.Log(JsonConvert.SerializeObject(character));
    }

    private void Update()
    {

    }
}

[Serializable]
public struct FreeIpApiResponse 
{
    public int ipVersion;
    public string ipAddress;
    public float latitude;
    public float longitude;
    public string countryName;
    public string countryCode;
}

//Sample
[Serializable]
public class Enemy 
{
    [JsonProperty]
    private float resistance;
    [JsonProperty]
    private float blockChance;

    public float Resistance => resistance;
    public float BlockChance => blockChance;
}
//Sample


public enum CharacterClass
{
    Warrior,
    Magician,
    Assassin
}

public struct SampleJsonCharacter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public CharacterClass chosenClass;
    public int characterLevel;
    public string characterName;

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime dateCreated;
}

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return DateTime.ParseExact((string)reader.Value, "yy-MM-dd", null);
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("yy-MM-dd"));
    }
}
