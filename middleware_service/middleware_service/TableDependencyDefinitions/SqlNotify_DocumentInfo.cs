using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_DocumentInfo
    {
        public int DocumentType { get; set; }
        public int OriginalDocumentID { get; set; }
        public int DocumentID { get; set; }
        public int PaymentMethod { get; set; }
    }
}
