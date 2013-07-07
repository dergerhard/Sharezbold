namespace Sharezbold.ElementsMigration
{
    using Microsoft.SharePoint.Client;

    abstract class AbstractMigrator
    {
        /// <summary>
        /// ClientContext of source SharePoint.
        /// </summary>
        protected ClientContext sourceClientContext;

        /// <summary>
        /// ClientContext of target SharePoint.
        /// </summary>
        protected ClientContext targetClientContext;

        protected AbstractMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        public abstract void Migrate();
    }
}
