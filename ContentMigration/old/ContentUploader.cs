//-----------------------------------------------------------------------
// <copyright file="ContentUploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Responsible for uploading content
    /// </summary>
    public class ContentUploader
    {
        /// <summary>
        /// Source context of share point server
        /// </summary>
        private ClientContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentUploader"/> class.
        /// </summary>
        /// <param name="context">The ClientContext of the destination SharePoint.</param>
        public ContentUploader(ClientContext context)
        {
            this.context = context;
        }

        public void MigrateListAndItsItems(Microsoft.SharePoint.Client.List list, Web createHere)
        {
            //ClientContext clientContext =
            //new ClientContext("http://intranet.contoso.com");
            //Web site = clientContext.Web;

            // Create a list.
            ListCreationInformation listCreationInfo =
                new ListCreationInformation();
            listCreationInfo.Title = list.Title;
            listCreationInfo.TemplateType =   list.BaseTemplate; //(int)ListTemplateType.GenericList

            List newList = createHere.Lists.Add(listCreationInfo);

            //copy all fields
            foreach (Field f in list.Fields)
            {
                //remove ID="{...}" to avoid duplicates/exception
                string schema = f.SchemaXml;
                Regex rgx = new Regex(" ID=\".*\"");
                schema = rgx.Replace(schema, "");
            
                newList.Fields.AddFieldAsXml(schema, true, AddFieldOptions.DefaultValue);
                Debug.WriteLine("SCHEMA::");
                Debug.WriteLine(f.SchemaXml);
            }
            
           /* // Add fields to the list.
            Field field1 = list.Fields.AddFieldAsXml(
                @"<Field Type='Choice'
                     DisplayName='Category'
                     Format='Dropdown'>
                <Default>Specification</Default>
                <CHOICES>
                  <CHOICE>Specification</CHOICE>
                  <CHOICE>Development</CHOICE>
                  <CHOICE>Test</CHOICE>
                  <CHOICE>Documentation</CHOICE>
                </CHOICES>
              </Field>",
                true, AddFieldOptions.DefaultValue);
            Field field2 = list.Fields.AddFieldAsXml(
                @"<Field Type='Number'
                     DisplayName='Estimate'/>",
                true, AddFieldOptions.DefaultValue);
            */
            /*
            // Add some data.
            ListItemCreationInformation itemCreateInfo =
                new ListItemCreationInformation();
            ListItem listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = "Write specs for user interface.";
            listItem["Category"] = "Specification";
            listItem["Estimate"] = "20";
            listItem.Update();

            listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = "Develop proof-of-concept.";
            listItem["Category"] = "Development";
            listItem["Estimate"] = "42";
            listItem.Update();

            listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = "Write test plan for user interface.";
            listItem["Category"] = "Test";
            listItem["Estimate"] = "16";
            listItem.Update();

            listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = "Validate SharePoint interaction.";
            listItem["Category"] = "Test";
            listItem["Estimate"] = "18";
            listItem.Update();

            listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = "Develop user interface.";
            listItem["Category"] = "Development";
            listItem["Estimate"] = "18";
            listItem.Update();*/


            /*
            tring siteUrl = "http://yourSiteCollection";
            ClientContext clientContext = new ClientContext(siteUrl);
            Web site = clientContext.Web;
            List sourceList = site.Lists.GetByTitle("SourceListName");
            
            //If you know the ID of the ListItem already that you want to copy, get the ListItem by ID instead of the Query below	
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "Query to retrieve the right ListItem to be copied";
            ListItemCollection collListItem = sourceList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            //Loop through to get the exact ListItem	
            foreach (ListItem oListItem in collListItem)
            {
                If("Some Condition to check if this is the right ListItem to copy")
	{
	      List targetList = site.Lists.GetByTitle("TargetList");
                      ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                      ListItem newListItem = targetList.AddItem(itemCreateInfo);
	      //populate the field values. If needed loop through each fields from the field collection to assign values	
	      newListItem["Title"] = oListItem["Title"];
                      newListItem["Body"] = oListItem["Body"] ;
                      newListItem.Update();
	      clientContext.Load(newListItem);
                      clientContext.ExecuteQuery();	
                      //newListItem.ID is the ID of the new item copied in the target site. 
	}
            }*/

            context.ExecuteQuery();
        }

    }
}
