#Solución demoms en .Net Core 6 para demo de microservicios

##Crear contenedores de base de datos de los microservicios

###DiscountDB
```
docker container run --env "ACCEPT_EULA=Y" --env "SA_PASSWORD=@dm1n2023!" --env "MSSQL_PID=Express" --publish 1450:1433 --volume VolDiscount:/var/opt/mssql --name SQLDiscount --detach mcr.microsoft.com/mssql/server:2019-latest
docker exec -it SQLDiscount /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P @dm1n2023!
1> CREATE DATABASE DiscountDB
2> GO
1> exit
```
###ShoppingBasketDB
```
docker container run --env "ACCEPT_EULA=Y" --env "SA_PASSWORD=@dm1n2023!" --env "MSSQL_PID=Express" --publish 1460:1433 --volume VolShoppingBasket:/var/opt/mssql --name SQLShoppingBasket --detach mcr.microsoft.com/mssql/server:2019-latest
docker exec -it SQLShoppingBasket /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P @dm1n2023!
1> CREATE DATABASE ShoppingBasketDB
2> GO
1> exit
```
###OrdersDB
```
docker container run --env "ACCEPT_EULA=Y" --env "SA_PASSWORD=@dm1n2023!" --env "MSSQL_PID=Express" --publish 1470:1433 --volume VolOrders:/var/opt/mssql --name SQLOrders --detach mcr.microsoft.com/mssql/server:2019-latest
docker exec -it SQLOrders /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P @dm1n2023!
1> CREATE DATABASE OrdersDB
2> GO
1> exit
```
