using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SwaggerHierarchySupport
{
  public static class SwaggerUIExtensions
  {
    /// <summary>
    /// Adds the SwaggerUI Plugin that supports hierarchical tags
    /// </summary>
    /// <param name="opt">The options object.</param>
    public static void AddHierarchySupport(this SwaggerUIOptions opt)
    {
      // Adds the plugin onto the page
      opt.InjectJavascript("https://unpkg.com/swagger-ui-plugin-hierarchical-tags");
      
      // Retrieves original function that is executed to create the index.html
      var oldIndexStream = opt.IndexStream;

      // Replace the creation of the HTML with our own function
      opt.IndexStream = () =>
      {
        // Execute the old stream to get the original index.html
        var stream = oldIndexStream();
        stream.Position = 0;
        var rdr = new StreamReader(stream);
        var html = rdr.ReadToEnd();

        // Now that we have the entire HTML, inject our plugin
        // Tried to do this via the Additional options, but JSON (which is parsed)
        // a literal value for the plugin
        html = html.Replace("const ui = SwaggerUIBundle(configObject);", @"
        if ('plugins' in configObject) configObject.plugins.push(HierarchicalTagsPlugin);
        else configObject.plugins = [ HierarchicalTagsPlugin ];
        const ui = SwaggerUIBundle(configObject);");
        
        // Create a new stream to contain the new HTML file
        var newStream = new MemoryStream();
        var writer = new StreamWriter(newStream, Encoding.UTF8);
        foreach (var line in html.Split("\n"))
        {
          writer.Write($"{line}\n");
          writer.Flush();
        }

        // Reset the stream to the starting position
        newStream.Position = 0;

        // Return the stream when requested.
        return newStream;
      };

    }
  }
}
