using DynamicModelProject.Models;
using DynamicModelProject.Services;
using System.Text.RegularExpressions;

namespace DynamicModelProject;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly SchemaService _schemaService;
    private readonly Dictionary<string, View> _inputControls = new();

    public MainPage(ApiService apiService, SchemaService schemaService)
    {
        InitializeComponent();
        _apiService = apiService;
        _schemaService = schemaService;
        LoadForm();
    }

    private async void LoadForm()
    {
        var schemaJson = await _apiService.GetSchemaJsonAsync();
        var model = _schemaService.GenerateModelFromSchema(schemaJson);

        foreach (var field in model.PropertyTypes)
        {
            string key = field.Key;
            string rawType = field.Value;
            string type = rawType.ToLower();

            Label? label = null;
            if (type != "button")
                label = new Label
                {
                    Text = key,
                    Style = Application.Current.Resources["FormLabel"] as Style // ✅ GLOBAL style for label
                };

            View inputView;

            if (type.StartsWith("select:"))
            {
                var options = rawType.Split(':')[1].Split(',');
                var picker = new Picker { Title = $"Select {key}" };

                foreach (var option in options)
                    picker.Items.Add(option);

                picker.Style = Application.Current.Resources["FormPicker"] as Style; // ✅ GLOBAL style for picker
                inputView = picker;
            }
            else if (type.StartsWith("radio:"))
            {
                var options = rawType.Split(':')[1].Split(',');
                var stack = new VerticalStackLayout();

                foreach (var option in options)
                {
                    var radio = new RadioButton
                    {
                        Content = option,
                        GroupName = key,
                        Style = Application.Current.Resources["FormRadio"] as Style // ✅ GLOBAL style for radio
                    };
                    stack.Children.Add(radio);
                }

                inputView = stack;
            }
            else
            {
                inputView = type switch
                {
                    "string" => new Entry
                    {
                        Placeholder = $"Enter {key}",
                        Style = key == "firstName"
                            ? null // 🟡 INLINE STYLE for 'firstName'
                            : Application.Current.Resources["FormEntry"] as Style,
                        BackgroundColor = key == "firstName" ? Colors.LightYellow : null, // 🟡 INLINE STYLE
                        FontAttributes = key == "firstName" ? FontAttributes.Italic : FontAttributes.None // 🟡 INLINE STYLE
                    },
                    "password" => new Entry
                    {
                        Placeholder = $"Enter {key}",
                        IsPassword = true,
                        Style = Application.Current.Resources["FormEntry"] as Style // ✅ GLOBAL
                    },
                    "int" => new Entry
                    {
                        Placeholder = $"Enter {key}",
                        Keyboard = Keyboard.Numeric,
                        Style = Application.Current.Resources["FormEntry"] as Style
                    },
                    "double" => new Entry
                    {
                        Placeholder = $"Enter {key}",
                        Keyboard = Keyboard.Numeric,
                        Style = Application.Current.Resources["FormEntry"] as Style
                    },
                    "email" => new Entry
                    {
                        Placeholder = $"Enter {key}",
                        Keyboard = Keyboard.Email,
                        Style = Application.Current.Resources["FormEntry"] as Style
                    },
                    "date" => new DatePicker
                    {
                        Style = Application.Current.Resources["FormDatePicker"] as Style // ✅ GLOBAL
                    },
                    "bool" => new Switch
                    {
                        Style = Application.Current.Resources["FormSwitch"] as Style // ✅ GLOBAL
                    },
                    "button" => new Button
                    {
                        Text = key.ToUpper(),
                        Command = new Command(OnSubmitClicked),
                        Style = Application.Current.Resources["FormButton"] as Style // ✅ GLOBAL
                    },
                    _ => new Label { Text = "Unsupported Type" }
                };
            }

            if (type != "button")
                _inputControls[key] = inputView;

            FormContainer.Children.Add(new VerticalStackLayout
            {
                Children = { label, inputView }
            });
        }
    }

    private void OnSubmitClicked()
    {
        Dictionary<string, object> result = new();
        bool hasError = false;

        foreach (var field in _inputControls)
        {
            string key = field.Key;
            View control = field.Value;

            object value = control switch
            {
                Entry e => e.Text,
                Switch s => s.IsToggled,
                Picker p => p.SelectedItem,
                DatePicker dp => dp.Date,
                VerticalStackLayout v => v.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked)?.Content,
                _ => null
            };

            // Validation: Empty field
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                hasError = true;
                DisplayAlert("Validation Error", $"Please fill the '{key}' field.", "OK");
                return;
            }

            // Email validation
            if (key.ToLower() == "email" && value is string emailText)
            {
                var isValidEmail = Regex.IsMatch(emailText, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!isValidEmail)
                {
                    hasError = true;
                    DisplayAlert("Validation Error", $"Please enter a valid email address.", "OK");
                    return;
                }
            }

            result[key] = value;
        }

        if (!hasError)
        {
            string summary = string.Join("\n", result.Select(kv => $"{kv.Key}: {kv.Value}"));
            DisplayAlert("Form Submitted", summary, "OK");
        }
    }
}
