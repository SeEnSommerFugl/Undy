// TODO: Drop all tables in the Undy Dundy Database, and remove DEFAULT NEWID() from all primary key columns
// TODO: Update all stored procedures to accept primary key parameters

/* TODO: TestSalesOrderView, tjek op på insert og bindings, da vi passer GUID med, 
 * men insert forventer CustomerNumber
 * Tjek op på repo og bindings
 * BØF MED LØG
 */

//Ændre salgsordrestadier til Afventer og Afsendt



/* TODO:
 * Startside mangler binding, hvis det kan nåes.
 * 
 * Indkøb mangler binding til "Vælg vare"
 * 
 * Varemodtagelse 2nd grid loader ikke, før man tjekker flueben fra, 
 * men loader samme uanset ordre.
 * 
 * Salgsordre 2nd grid mangler binding. Main Grid opdatere ikke ved ny salgsordre.
 * Lister skal kobles sammen. 
 * 
 * Betalinger mangler SQL "SelectByID"
 * Mangler binding på checkmark til at select produkt 
 * Checkmark selecter ikke, medmindre linjen er selected.
 * 
 * TestReturneringer fejler ved ordrenummer.. Binding?
 * 
 * TestIndkøbsordre og indkøb skal bindes sammen til samme View.
 * 
 * Vi har ingen metode til at oprette nye produkter???
*/