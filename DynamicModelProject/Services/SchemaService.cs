using DynamicModelProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicModelProject.Services
{
    public class SchemaService
    {
        
        public SchemaService()
        {
            
        }

        public DynamicModel GenerateModelFromSchema(string jsonSchema)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonSchema);
            var model = new DynamicModel();

            foreach (var field in dict)
            {
                model.PropertyTypes[field.Key] = field.Value;
            }

            return model;
        }
    }
}
