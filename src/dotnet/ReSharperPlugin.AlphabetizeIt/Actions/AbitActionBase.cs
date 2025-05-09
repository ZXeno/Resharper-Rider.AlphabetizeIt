using System;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel.Storage;
using JetBrains.DocumentModel.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.AlphabetizeIt.Actions;

public abstract class AbitActionBase : BulbActionBase
{
    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        return delegate(ITextControl textControl)
        {
            solution.GetComponent<IDocumentStorageHelpers>().SaveAllDocuments();

            using ITransactionCookie transactionCookie =
                solution.GetComponent<DocumentTransactionManager>()
                    .CreateTransactionCookie(DefaultAction.Commit, "action name");

            IPsiServices services = solution.GetPsiServices();
            services.Transactions.Execute(
                Text,
                () => services.Locks.ExecuteWithWriteLock(() => { ExecuteAction(solution, textControl); }));
        };
    }

    protected abstract void ExecuteAction(ISolution solution, ITextControl textControl);
}