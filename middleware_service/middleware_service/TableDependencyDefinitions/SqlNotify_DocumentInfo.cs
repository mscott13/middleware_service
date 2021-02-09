using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    public class SqlNotify_DocumentInfo
    {
        public SqlNotify_DocumentInfo() { }
        public SqlNotify_DocumentInfo(int documentType, int originalDocumentId, int documentId, int paymentMethod)
        {
            DocumentType = documentType;
            OriginalDocumentID = originalDocumentId;
            DocumentID = documentId;
            PaymentMethod = paymentMethod;
        }

        public int DocumentType { get; set; }
        public int OriginalDocumentID { get; set; }
        public int DocumentID { get; set; }
        public int PaymentMethod { get; set; }
    }
}
