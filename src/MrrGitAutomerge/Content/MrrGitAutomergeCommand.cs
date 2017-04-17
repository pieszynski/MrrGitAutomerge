//------------------------------------------------------------------------------
// <copyright file="MrrGitAutomergeCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MrrGitAutomerge.Core.Contracts;
using EnvDTE80;
using EnvDTE;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MrrGitAutomerge.Core;

namespace MrrGitAutomerge.Content
{
    internal sealed class MrrGitAutomergeCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("808149e7-bb74-4b8e-ba81-19f69cf69abf");
        private readonly Package package;

        private static ILogger StatLogger;
        private static MrrGitAutomergeUtil Mrr;
        private static string MergeToBranch = "master";

        private MrrGitAutomergeCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }
        public static MrrGitAutomergeCommand Instance
        {
            get;
            private set;
        }
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }
        public static void Initialize(Package package)
        {
            StatLogger = new ExtensionLogger();
            Mrr = new MrrGitAutomergeUtil(StatLogger);
            Instance = new MrrGitAutomergeCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE2 dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            object selectedItem = ((Array)dte.ToolWindows.SolutionExplorer.SelectedItems)
                .OfType<UIHierarchyItem>()
                .FirstOrDefault()
                ?.Object;

            string filePath = (selectedItem as Solution)?.FileName ?? (selectedItem as Project).FileName;
            if (string.IsNullOrEmpty(filePath))
                return;

            string fileDir = Path.GetDirectoryName(filePath);
            
            MrrGitAutomergeDialog dlg = new MrrGitAutomergeDialog(
                Mrr,
                fileDir,
                MergeToBranch
                );

            dlg.ShowDialog();

            MergeToBranch = dlg.SelectedMergeToBranch;
        }
    }
}
