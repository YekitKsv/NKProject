﻿using System.Threading.Tasks;
using Catel.MVVM;

namespace Shell.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        internal static string ReportTitle;

        public ReportViewModel()
        {
            ReportTitle = Title;
        }

        public override string Title
        {
            get { return "Отчёты"; }
        }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}