using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Events.WebAPI.Util.Swagger;

/// <summary>
/// Describes JsonPatchDocument&lt;T&gt; request bodies as RFC 6902 arrays of operations
/// (instead of the C# class structure) and adds an example based on properties of T
/// </summary>
public class JsonPatchOperationFilter : IOperationFilter
{
  private static readonly string[] operations = ["add", "remove", "replace", "move", "copy", "test"];

  /// <inheritdoc/>
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    var patchType = context.ApiDescription.ParameterDescriptions
                           .Select(p => p.Type)
                           .FirstOrDefault(t => t != null && t.IsGenericType &&
                                                t.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>));
    if (patchType == null || operation.RequestBody?.Content == null)
      return;

    var targetType = patchType.GetGenericArguments()[0];
    var paths = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          .Where(p => p.CanWrite)
                          .Select(p => "/" + p.Name)
                          .ToList();

    var schema = new OpenApiSchema
    {
      Type = JsonSchemaType.Array,
      Description = "RFC 6902 JSON Patch document",
      Items = new OpenApiSchema
      {
        Type = JsonSchemaType.Object,
        Required = new HashSet<string> { "op", "path" },
        Properties = new Dictionary<string, IOpenApiSchema>
        {
          ["op"] = new OpenApiSchema
          {
            Type = JsonSchemaType.String,
            Enum = operations.Select(o => (JsonNode)JsonValue.Create(o)).ToList()
          },
          ["path"] = new OpenApiSchema
          {
            Type = JsonSchemaType.String,
            Description = "JSON Pointer to the target property, e.g. " + string.Join(", ", paths.Take(3))
          },
          ["value"] = new OpenApiSchema
          {
            Description = "New value (for add, replace and test)"
          },
          ["from"] = new OpenApiSchema
          {
            Type = JsonSchemaType.String,
            Description = "Source JSON Pointer (for move and copy)"
          }
        }
      }
    };

    // replace the first property that is not the key, e.g. /FirstName for PersonDTO
    string examplePath = paths.FirstOrDefault(p => !p.Equals("/Id", StringComparison.OrdinalIgnoreCase))
                         ?? paths.FirstOrDefault() ?? "/property";
    var example = new JsonArray(new JsonObject
    {
      ["op"] = "replace",
      ["path"] = examplePath,
      ["value"] = "new value"
    });

    var content = operation.RequestBody.Content;
    foreach (var contentType in content.Keys.ToList())
    {
      content[contentType] = new OpenApiMediaType
      {
        Schema = schema,
        Example = example.DeepClone()
      };
    }
  }
}
