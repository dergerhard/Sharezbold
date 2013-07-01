
TODO:
- Quellsysteme: SharePoint 2010, SharePoint 2013
- Zielsystem: SharePoint 2013

- Quellobjekte: 
	+ Listen, 
	+ Bibliotheken, 
	+ Webs/Sites, 
	+ Site Collections 
	
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
