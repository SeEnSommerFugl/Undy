# Readme_SQL
 
Dette dokument beskriver SQL-strukturen for projektet Undy, herunder tabeller, views, stored procedures, naming conventions samt korrekt eksekveringsrækkefølge.
 
Formålet er at sikre:
 
    - Konsistent datamodel
    - Stabil binding mellem SQL =>  Repository =>  ViewModel =>  View
    - Let genopbygning af databasen uden at droppe hele databasen
 
--------------------------------------------------------------------------------
 
## Overordnet struktur
 
SQL-laget er opdelt i følgende hovedkategorier:
 
    - Tables
        - Fysiske datatabeller
 
    - Views
        - Read-only visninger til UI og repositories
 
    - Stored Procedures
        - Insert / Update / Delete / SelectById
 
    - Seed / Dummy data
        - Testdata til udvikling
 
UI må aldrig binde direkte til tables – kun til views.
 
--------------------------------------------------------------------------------
 
## Naming conventions (kritisk)
 
### Tables
 
    - Singular
    - PascalCase
 
    Eksempler:
        - SalesOrder
        - SalesOrderLine
        - Customer
        - Payment
 
### Views
 
    - Prefix: vw_
    - Plural
    - Beskriver hvad UI skal vise
 
    Eksempler:
        - vw_SalesOrderHeaders
        - vw_SalesOrderLines
        - vw_WholesaleOrderLines
        - vw_CustomerSalesOrders
 
### Stored Procedures
 
    - Prefix: usp_
    - Verb-baseret
    - Enten:
        - usp_Insert_<Entity>
        - usp_Update_<Entity>
        - usp_DeleteById_<Entity>
        - usp_SelectById_<Entity>
 
    Eksempler:
        - usp_SelectById_SalesOrder
        - usp_Update_Product
        - usp_DeleteById_WholesaleOrderLine
 
--------------------------------------------------------------------------------
 
## Display-felter (vigtigt for UI)
 
Alle UI-venlige værdier genereres i views, ikke i C# og ikke i stored procedures.
 
    Eksempel:
        CONCAT('SALG-', CAST(so.SalesOrderNumber AS varchar(20)))
            AS DisplaySalesOrderNumber
 
    Eksempel:
        CONCAT('KØB-', CAST(wo.WholesaleOrderNumber AS varchar(20)))
            AS DisplayWholesaleOrderNumber
 
    Regler:
        - Display-felter må aldrig bruges i stored procedures
        - Display-felter findes kun i views
 
--------------------------------------------------------------------------------
 
## Views – ansvar
 
### vw_SalesOrderHeaders
 
    Bruges af:
        - SalesOrderView
        - PaymentsView
 
    Indeholder:
        - SalesOrderID
        - SalesOrderNumber
        - DisplaySalesOrderNumber
        - CustomerID
        - CustomerNumber
        - Kundeinformation (navn mv.)
        - Totaler og statusfelter
 
### vw_SalesOrderLines
 
    Bruges af:
        - SalesOrderLine repositories
        - Redigering af ordrelinjer
 
    Indeholder:
        - Quantity
        - UnitPrice
        - LineTotal
        - Produktinformation
 
### vw_WholesaleOrderLines
 
    Indeholder beregnede felter:
        - QuantityPending = Quantity - QuantityReceived
        - LineTotal = Quantity * UnitPrice
 
    Regler:
        - QuantityPending er read-only
        - UI må kun opdatere QuantityReceived
 
--------------------------------------------------------------------------------
 
## Stored Procedures – regler
 
### Insert / Update
 
    - Arbejder kun på tables
    - Ingen joins
    - Ingen beregnede felter
    - Ingen display-felter
 
### SelectById
 
    - Bruges af repositories
    - Returnerer table-struktur, ikke view-struktur
    - UI bruger views i stedet
 
--------------------------------------------------------------------------------
 
## Repository binding (C#)
 
    Repositories forventer:
        - At alle kolonner i views matcher IDataRecord.GetOrdinal(...)
        - At kolonnenavne er stabile
 
    Eksempel:
        r.GetString(r.GetOrdinal("CustomerNumber"));
 
    Hvis en kolonne mangler eller er omdøbt:
        - Resultat: runtime exception
 
--------------------------------------------------------------------------------
 
## Drop & rebuild strategi
 
    Databasen slettes ikke.
 
    I stedet udføres:
        1. Drop views
        2. Drop stored procedures
        3. Drop tables (korrekt rækkefølge pga. foreign keys)
        4. Opret tables
        5. Opret stored procedures
        6. Opret views
        7. Indsæt seed / dummy data
 
    Dette kan køres samlet i én SQL-fil.
 
--------------------------------------------------------------------------------
 
## Eksekveringsrækkefølge (én samlet SQL-fil)
 
    Anbefalet struktur:
        - Drop views
        - Drop stored procedures
        - Drop tables
 
        - Create tables
        - Create stored procedures
        - Create views
 
        - Insert seed / dummy data
 
    GO bruges mellem hvert logisk afsnit.
 
--------------------------------------------------------------------------------
 
## Typiske fejl og årsager
 
### Invalid column name DisplaySalesOrderNumber
 
    Årsag:
        - Feltet bruges i stored procedure
        - Eller forventes i repository, men findes kun i view
 
    Løsning:
        - Flyt feltet til view
        - Ret repository til at binde mod view
 
### System.IndexOutOfRangeException
 
    Årsag:
        - View mangler kolonne
        - Kolonnenavn ændret uden repository-opdatering
 
    Løsning:
        - Sammenlign SELECT i view med Map() i repository
 
--------------------------------------------------------------------------------
 
## Vigtige principper (kort)
 
    - UI =>  Views =>  Repositories =>  Tables
    - Ingen UI-logik i SQL
    - Ingen SQL-logik i ViewModels
    - Display-felter = views only
    - Calculated fields = views only
 
--------------------------------------------------------------------------------
 
## Status
 
    Denne README matcher den nuværende faktiske arkitektur i projektet.
 
    Skal opdateres ved:
        - Nye views
        - Nye display-felter
        - Ændringer i naming conventions
