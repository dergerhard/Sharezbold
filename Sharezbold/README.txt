
TODO:
- Quellsysteme: SharePoint 2010, SharePoint 2013
- Zielsystem: SharePoint 2013

- Quellobjekte: 
	+ Listen, 
	+ Bibliotheken, 
	+ Webs/Sites, 
	+ Site Collections 

	Client			|	Server
	----------------------------------
	ClientContext	|	SPContext
	Site			|	SPSite			+ = Site Collection
	Web				|	SPWeb			+ = Web(site)
	List			|	SPList			+
	ListItem		|	SPListItem		+
	Field			|	SPField			+
	
- Mapping von unterschiedlichen Spaltennamen 

- Migration von und in fremde Farmen und Domains

- Es müssen auch Elemente wie 
	+ Content Typs, 
	+ Site Columns, 
	+ Benutzer, 
	+ Gruppen, 
	+ Permissionlevels, 
	+ Workflows
  migriert werden 


- Versionen/History und Berechtigungen müssen wenn möglich erhalten bleiben 
	---> war nicht möglich

- Am Quellsystem dürfen nach der Migration keine Änderungen zurückbleiben (gleicher 
Status)--> eh klar

- Es müssen Profile/Projekte definiert und gespeichert werden können.


- Die Auswahl der Systeme und Elemente muss über einen TreePicker realisiert werden.

- Konfigurationsdaten wie z.B.: 
	+ Authentifizierungsdaten, 
	+ Proxy-Einstellungen, usw. müssen pro 
Profil, unter Berücksichtigung aktueller Methoden und Technologien, gespeichert werden. 
Hierbei müssen ggf. vorhandenen Microsoft-Programmierrichtlinien eingehalten werden.

- Der Upload der Dokumente muss – ähnlich der Konfigurationsmöglichkeiten eines 
Download-Managers – parametriert werden können. Hierbei sollen unter anderem die 
Anzahl der gleichzeitigen Uploads sowie die maximal zu verwendende Bandbreite eingestellt 
werden können.

- Dateibeschränkungen (Type, Größe, …) müssen ausgelesen werden und in den Upload 
Prozess integriert werden. 


Verwendete Libraries:
	- log4net


##############
MigrationSettings - stores migration configuration
-------
Creation:
 - create an XSD schema file, describing the settings file with all its attributes
 - open the Visual Studion console prompt, navigate to the schema and type "xsd \c schema.xsd"
 - A fully serializable class "MigrationSettings" is created and ready to be used
 - further info: http://msdn.microsoft.com/en-us/library/ms950721.aspx
 -----
 Classe Design:
 - MigrationSettings holds data
 - MigrationSettingsManager loads and saves (load(string path), save(MigrationSettings settings, string path))


 Wer macht was?
 ##############
 Thomas:
	Content Types, Site Columns, Benutzer, Gruppen, Permission levels, Workflows
	Installer

 Gerhard:
	TreePicker mit Listen, Bibliotheken, Webs/Sites, Site Collections
	Mapping Spaltennamen
	Version/History migrieren
	Download/Uplaod Manager

Projekte:
	ContentMigration, TypeMigration



##############
Server Tomas:
-----------
10.10.102.36
CSSDEV\Administrator
P@ssw0rd

Sharepoint 2010
-----------
10.10.102.48
CSSDEV\Administrator
P@ssw0rd

Server Gerhard:
-----------
10.10.102.38
CSSDEV\Administrator
P@ssw0rd



#### Administrator kann eigene Site-Collections nicht sehen ####
--> disable loopback in der registry
http://social.technet.microsoft.com/Forums/sharepoint/en-US/d446b9a3-d18c-42d9-85f7-383d6b67af64/cannot-access-site-collection-from-wss-server?forum=sharepointgenerallegacy



//-----------------------------------------------------------------------
// <copyright file=".cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------


Review Infos Gerhard:
###############################
+ es war nicht klar, dass beim laden des objekts nicht alle Daten geladen werden - dass man das explizit angeben muss hat stunden gekostet
	ListItemCollection listItemColl = li.GetItems(cq);
	context.Load(listItemColl, items => items.Include(
                    item => item.Id,
                    item => item.DisplayName));
	context.ExecuteQuery();
	
+ Beim Client Object Model ist es offensichtlich nciht möglich alles Site Collections zu laden. Die Site Collection ist die oberste Ebene
+ Web Service für migration gibt es
+ im Client Object Model wird die SiteCollection wie ein Web behandelt
+ Asynchrones laden: 
	Viele Methoden (async keyword, Threads, BackgroundWorker,...)
	Zuerst mit BackgroundWorker --> handling ist aber sperrig
	Jetzt mit delegates und BeginInvoke, Invoke und es ist einfach, übersichtlich und funktioniert fehlerfrei
	http://dougzuck.com/c-ui-threading-example

+ Fragwürdiges Verhalten der web-services z.B. Sites.CreateWeb wirft eine Exception ("Error in XML document(1,30)"), erzeugt jedoch trotzdem die Seite!

Async Programming
http://msdn.microsoft.com/de-de/library/vstudio/hh191443.aspx




What to migrate
Site Collection:
	Name
	Description
	Url
	Template
	Primary Admin
	Secondary Admin
	Quota Template



	            /*
             * 	Client          |  Server
             * 	----------------------------------
             * 	ClientContext	|  SPContext
             * 	Site			|  SPSite			+ = Site Collection
             * 	Web				|  SPWeb			+ = Web(site)
             * 	List			|  sharepointList			+
             * 	ListItem		|  sharepointListItem		+
             * 	Field			|  SPField			+
             */


Ich:
Gui fertigmachen, migration content fertigmachen

Thomas:
file-migration

Gemeinsam:
 - Doc (Manual)
 - C# Doc
 - Powerpoint



NEUER SHAREPOINT 2010
--------------------------------------------------------------------------------
10.10.7.179 (=main site collection)
Administrator
P@ssw0rd
CSSSPS2010003
--------------------------------------------------------------------------------
Eingestellt wurde
--------------------------------------------------------------------------------
	theme: azure
	site columns:
		Group: JobApplication
				 CompanyName
				 ApplicationDate
				 ContactPerson
				 Interview
	Custom list:
		MyJobApplications
			View "AllItems" changed to show site columns only
			
			2 items added
			
	Site Pages:
		+ Product Placemet
		- How to use this Library (Recycle bin)
		
	Shared Documents:
		+ kite_for_kitesurfing..... jpg
		+ kite_surfing_Oahu.jpg
		+ KITE.docx --> CHECKED OUT
		
	Tasks
		+ Something about Sharepoint