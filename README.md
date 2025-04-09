# Feasibility Report: Dynamic Model and Form Generation in .NET MAUI

**Objective**  
To dynamically generate a C# model from an API-provided schema and use that model to dynamically render a form UI in a .NET MAUI application.

**Git Repo Link:** [https://github.com/Stanley-Metray/DynamicModel](https://github.com/Stanley-Metray/DynamicModel)

---

## 🔁 Workflow Overview

### 1. Dynamic Model Generation

- **Step:** An API returns a JSON schema.  
- **Goal:** Convert this schema into a strongly typed C# object at runtime.  
- **Tool:** A service (`SchemaService`) interprets the schema and creates a dynamic model structure — mainly a dictionary of property names and their types.

#### API returns JSON like:
```json
{
  "firstName": "string",
  "password": "password",
  "age": "int",
  "email": "email",
  "gender": "radio:Male,Female,Other",
  "country": "select:India,USA,Canada",
  "dob": "date",
  "isMember": "bool",
  "submit": "button"
}
```


Create DynamicModel.cs in the Models folder:

public class DynamicModel
{
    public Dictionary<string, string> PropertyTypes { get; set; } = new();
}


2. Dynamic Form Generation

    Step: The MainPage loads the model and iterates through each field.

    Goal: Dynamically create UI controls (Entry, Picker, Switch, etc.) based on the type of each field.

    Tracking: All controls are stored in a dictionary _inputControls for later value retrieval.

Examples:
JSON Type	UI Control
string	Entry
bool	Switch
email	Entry + validation
select:...	Picker
radio:...	RadioButton Group

    This technique does not alter the MVC or MVVM architecture in .NET.

✅ Advantages of This Approach
🔄 Flexibility

    Add, remove, or change fields without modifying UI code.

    API-driven and modular form design.

♻️ Reusability

    Can render different forms just by changing the JSON schema.

✅ Custom Validation

    Field-level validation like required, email format, etc., can be implemented.

🎨 Styling Support

    Supports both global styles (DynamicResource) and inline styles.

⚠️ Disadvantages & Limitations
🧠 Code Complexity

    LoadForm() becomes harder to manage with more input types.

    Conditional logic (if (key == "firstName")) increases cognitive load.

⚠️ Limited Compile-Time Safety

    No IntelliSense.

    More runtime errors.

    Debugging is harder than static XAML.

🔐 Validation Logic is Tight-Coupled

    All validations are handled in code-behind (OnSubmitClicked()), limiting reusability.

### 📊 Feasibility Summary

| Factor         | Rating   | Notes                                           |
|----------------|----------|-------------------------------------------------|
| Flexibility    | ⭐⭐⭐⭐⭐   | Highly adaptable for schema-driven forms        |
| Reusability    | ⭐⭐⭐⭐☆   | Good, but tied to one form container style      |
| Maintainability| ⭐⭐☆☆☆   | Gets harder as types/fields grow                |
| Readability    | ⭐⭐☆☆☆   | Mixed logic in code-behind increases overhead   |
| Extensibility  | ⭐⭐⭐☆    | Extendable with effort                          |

### 😰 Code Complexity: Is it Overwhelming?


Yes, especially:

    When many new input types are introduced.

    When styling or validation becomes conditional.

    When developers are unfamiliar with dynamic UI generation.

💡 Suggestions to Manage Complexity

    Break Down Code

        Move input creation into helper methods like View GenerateInputView(string key, string type)

    Use ViewModel Binding

        Leverage INotifyPropertyChanged and data binding for clean validation.

    Componentize Layouts

        Wrap label + input into reusable views for consistency.

    Add Comments & Logging

        Improves maintainability and debugging.

✅ Final Recommendation

Use this dynamic form generation approach when:

    Form structure changes based on roles or external configs.

    Forms are schema-driven (CMS or admin-defined).

    You're building form builders, surveys, onboarding tools, or admin panels.

Avoid this approach when:

    Form layout is static and rarely changes.

    App stability and clarity are top priorities for non-tech users.

    You're working with complex, nested, or multi-step forms — in such cases, consider using a form DSL or dedicated form engine.






