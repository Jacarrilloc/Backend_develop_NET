using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

public class Person
{
    public string UserName { get; set; }
    public int UserAge { get; set; }
}

class Program
{
    static void Main()
    {
        // Crear una instancia de Person con datos de ejemplo
        Person person = new Person
        {
            UserName = "JohnDoe",
            UserAge = 30
        };

        // Serialización Binaria
        string binaryFilePath = "person.dat";
        using (FileStream fs = new FileStream(binaryFilePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            writer.Write(person.UserName);
            writer.Write(person.UserAge);
        }
        Console.WriteLine("Binary serialization completed. Data saved in person.dat");

        // Serialización XML
        string xmlFilePath = "person.xml";
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
        using (StreamWriter writer = new StreamWriter(xmlFilePath))
        {
            xmlSerializer.Serialize(writer, person);
        }
        Console.WriteLine("XML serialization completed. Data saved in person.xml");

        string jsonFilePath = "person.json";
        string jsonString = JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonFilePath, jsonString);
        Console.WriteLine("JSON serialization completed. Data saved in person.json");
    }
}
