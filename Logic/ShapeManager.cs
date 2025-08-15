using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;


namespace MonoTetris.Logic;

public class ShapeManager
{
    private const string ContentFolder = "shapes";
    
    private readonly List<Shape> _shapes = new List<Shape>();

    private readonly ContentManager _content;
    
    public ShapeManager(ContentManager content)
    {
        _content = content;
        
        // Get list of files from content folder with jsons shapes
        var files = GetFiles();
        
        // Setup serialization options
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        // Add each shape from deserialized data to list
        foreach (var file in files)
        {
            var jsonString = File.ReadAllText(file);
            _shapes.Add(JsonSerializer.Deserialize<Shape>(jsonString, options));
        }
    }
    
    public Shape GetRandomShape()
    {
        // Init random
        Random random = new Random();
        // Generate random index
        var randomIndex = random.Next(_shapes.Count);

        // Return random shape
        return _shapes[randomIndex];
    }
    
    private string[] GetFiles()
    {
        var dir = Path.Combine(_content.RootDirectory, ContentFolder);

        return Directory.GetFiles(dir, "*.json");
    }
}