# prRecipieDatabase (now known as Project Chili)
```
If you want to build this server, create a appsettings.json file in Blazor Server App, replacing the bracketed text where appropriate

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },

  "AllowedHosts": "*",
  "ConnectionStrings": {
    "recipeDatabase": "Server=(yourIP);Port=(yourPort);Database=RecipeDatabase;user id=(yourUsername);Pwd=(yourPassword);",
    "WordsAPI": "(yourAPIKey)"
  }
}
```
