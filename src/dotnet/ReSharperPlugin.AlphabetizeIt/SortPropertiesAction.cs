using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.Storage;
using JetBrains.DocumentModel.Transactions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;


namespace ReSharperPlugin.AlphabetizeIt;

public sealed class SortPropertiesAction : BulbActionBase
{
    private readonly IClassDeclaration _classDeclaration;

    public SortPropertiesAction(ICSharpContextActionDataProvider provider)
    {
        _classDeclaration = provider.GetSelectedElement<IClassDeclaration>();
    }

    public override string Text => "Sort properties alphabetically";

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
                () => services.Locks.ExecuteWithWriteLock(() => { ExecuteTransactionInner(solution, textControl); }));
        };
    }

    private void ExecuteTransactionInner(ISolution solution, ITextControl textControl)
    {
        IList<IPropertyDeclaration> properties =
            _classDeclaration.MemberDeclarations
                .OfType<IPropertyDeclaration>()
                .OrderBy(p => p.DeclaredName)
                .ToList();

        if (properties.Count <= 1)
        {
            return;
        }

        // Get the class body where properties are located
        IClassBody classBody = _classDeclaration.Body;
        if (classBody == null)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();
        CSharpElementFactory factory = CSharpElementFactory.GetInstance(_classDeclaration);

        // Remove all properties
        foreach (IPropertyDeclaration prop in properties)
        {
            _classDeclaration.RemoveClassMemberDeclaration(prop);
        }

        // Add the properties back in the sorted order after the last constructor and before methods.
        bool hasConstructors = _classDeclaration.ConstructorDeclarations.Count > 0;
        bool hasMethods = _classDeclaration.MethodDeclarations.Count > 0;
        ITreeNode anchor = hasConstructors
            ? _classDeclaration.ConstructorDeclarations.Last()
            : classBody;

        anchor = !hasConstructors && hasMethods
            ? _classDeclaration.MethodDeclarations[0]
            : anchor;

        foreach (IPropertyDeclaration prop in properties)
        {
            IPropertyDeclaration newprop =
                (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(prop.GetText());

            if (hasConstructors)
            {
                IPropertyDeclaration addedProp = ModificationUtil.AddChildAfter(anchor, newprop);
                anchor = addedProp;
                continue;
            }

            if (hasMethods)
            {
                ModificationUtil.AddChildBefore(anchor, newprop);
                continue;
            }

            _classDeclaration.AddClassMemberDeclaration(newprop);
        }

        classBody.FormatNode();
    }
}