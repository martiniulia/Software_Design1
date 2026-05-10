Start-Process powershell -ArgumentList "-NoExit -Command `"cd FlowerShop.CatalogService; dotnet run --urls http://localhost:5001`""
Start-Process powershell -ArgumentList "-NoExit -Command `"cd FlowerShop.OrderService; dotnet run --urls http://localhost:5002`""
Start-Process powershell -ArgumentList "-NoExit -Command `"cd FlowerShop.UserService; dotnet run --urls http://localhost:5003`""
Start-Process powershell -ArgumentList "-NoExit -Command `"dotnet run`""
