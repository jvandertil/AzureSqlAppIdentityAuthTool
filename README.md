# Azure SQL Managed Identity Authorization Tool

This tool can help you by authorizing the managed application identity in a Azure SQL database.

Essentially this tools allows you to perform the following SQL statements:
```sql
CREATE USER [<identity-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<identity-name>];
ALTER ROLE db_datawriter ADD MEMBER [<identity-name>];
ALTER ROLE db_ddladmin ADD MEMBER [<identity-name>];
GO
```

## Usage
The tool uses a connection string to connect to the Azure SQL database, it has the following form:
```
Server=<server-name>.database.windows.net,1433;Database=<db-name>;Encrypt=True;Authentication='Active Directory Password';User Id=<aad-user-name>;Password=<aad-password>;
```
Note that you have to use an Azure Active Directory user when connecting to authorize the identity.

You can optionally choose to skip adding the identity to a role by passing one (or more) of the following command line switches:
```
--no-datareader           (Default: false) Do not add the identity to the 'db_datareader' role.
--no-datawriter           (Default: false) Do not add the identity to the 'db_datawriter' role.
--no-ddladmin             (Default: false) Do not add the identity to the 'db_ddladmin' role.
```

More information is available in the tutorial in the [Microsoft Docs](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi#use-managed-identity-connectivity).
