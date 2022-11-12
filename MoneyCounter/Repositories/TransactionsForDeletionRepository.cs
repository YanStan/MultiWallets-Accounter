using MoneyCounter.Models;
using System.Linq;

namespace MoneyCounter.Repositories
{
    class TransactionsForDeletionRepository
    {
        readonly MoneyCounterContext db = new MoneyCounterContext();

        public void SetActiveTransactionIdForDeletion(int transId, int operatorId, string operatorUserName)
        {
            TransactionIDForDeletion transactionId = new TransactionIDForDeletion();
            transactionId.TransactionId = transId;
            transactionId.OperatorThatRequests = operatorUserName;
            transactionId.OperatorThatRequestsId = operatorId;
            transactionId.WasDeletionAllowed = false;
            transactionId.IsActive = true;
            db.TransactionIDsForDeletion.Add(transactionId);
            db.SaveChanges();
        }
        public void SetFinalTransactionIdForDeletion(int operatorId, int AdminId, int transId,
            string adminUserName)
        {
            var transactionId = db.TransactionIDsForDeletion.FirstOrDefault(x => x.IsActive == true
            && x.OperatorThatRequestsId == operatorId && x.TransactionId == transId);
            if (transactionId != null)
            {
                transactionId.AdminThatAllowedId = AdminId;
                transactionId.AdminThatAllowed = adminUserName;
                db.SaveChanges();
            }
        }

        public void InactivateTransactionIdForDeletion(int operatorId, int transId, bool wasDeletionAllowed)
        {
            var transactionId = db.TransactionIDsForDeletion.FirstOrDefault(x => x.IsActive == true
                && x.OperatorThatRequestsId == operatorId);
            if (transactionId != null)
            {
                transactionId.IsActive = false;
                transactionId.WasDeletionAllowed = wasDeletionAllowed;
                db.SaveChanges();
            }
            db.TransactionIDsForDeletion.RemoveRange(db.TransactionIDsForDeletion.Where(x => x.IsActive == true 
            && x.TransactionId == transId && x.OperatorThatRequestsId == operatorId));

        }
        public void DeleteAllSpareEntities(int userId)
        {
            var entitiesToRemove = db.TransactionIDsForDeletion.Where(x => x.OperatorThatRequestsId == userId &&
                x.AdminThatAllowed == null && x.AdminThatAllowedId == null).ToList();
            db.TransactionIDsForDeletion.RemoveRange(entitiesToRemove);
            db.SaveChanges();
        }
    }
}
