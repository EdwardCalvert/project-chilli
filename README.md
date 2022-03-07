# prRecipieDatabase (now known as Project Chili)


What it aims to do: provide a centralised database of recipes, and then allow a way to search through them.

It can just about read .docx files in bulk and spot the ingredients and methods. 


If you want to build this server, create a appsettings.json file in Blazor Server App, replacing the bracketed text where appropriate
```
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
