# prRecipieDatabase (now known as Project Chili)
Part of my A Level Computer Science Course. 
You can find the doucmentation here:https://1drv.ms/w/s!Al4Lg6dVSrCSg9ARa24R2Y0yDEf5tg?e=cFfevy

If you want to build this server, create a appsettings.json file in Blazor Server App

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
    "recipeDatabase": "Server=<yourIP>;Port=<yourPort>;Database=RecipeDatabase;user id=<yourUsername>;Pwd=<yourPassword>;",
    "WordsAPI": "<yourAPIKey>"
  }
}
