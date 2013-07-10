

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SharePoint2007Migrator : AbstractMigrator
    {
        TypeMigration.GetGroupSharePoint2007.GetUserCollectionCompletedEventArgs users = new TypeMigration.GetGroupSharePoint2007.GetUserCollectionCompletedEventArgs(null, new Exception(), false, null);

        public override void Migrate()
        {
            //users.Result.
            throw new NotImplementedException();
        }
    }
}
