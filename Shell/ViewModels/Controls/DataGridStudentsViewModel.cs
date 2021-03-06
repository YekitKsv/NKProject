﻿using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Shell.Models.DataModel;
using Shell.Views.Controls;

namespace Shell.ViewModels.Controls
{
    public class DataGridStudentsViewModel : ViewModelBase
    {
        
        private readonly Model _db;
        private IDependencyResolver _dependencyResolver;
        private IUIVisualizerService _uiVisualizerService;

        public DataGridStudentsViewModel()
        {
            _db = HomeViewModel.Model;
            var viewLocator = ServiceLocator.Default.ResolveType<IViewLocator>();
            viewLocator.Register(typeof(DataGridStudentsViewModel), typeof(DataGridStudentsView));
        }

        #region Property

        #region CBI property

        /// <summary>
        ///     Gets or sets the CBI value.
        /// </summary>
        public string CBI
        {
            get { return GetValue<string>(CBIProperty); }
            set { SetValue(CBIProperty, value); }
        }

        /// <summary>
        ///     CBI property data.
        /// </summary>
        public static readonly PropertyData CBIProperty = RegisterProperty("CBI", typeof(string));

        #endregion

        #region DbGroups property

        /// <summary>
        ///     Gets or sets the DbGroups value.
        /// </summary>
        public ObservableCollection<Groups> DbGroups
        {
            get { return GetValue<ObservableCollection<Groups>>(DbGroupsProperty); }
            set { SetValue(DbGroupsProperty, value); }
        }

        /// <summary>
        ///     DbGroups property data.
        /// </summary>
        public static readonly PropertyData DbGroupsProperty = RegisterProperty("DbGroups",
            typeof(ObservableCollection<Groups>));

        #endregion

        #region DbStudents property

        /// <summary>
        ///     Gets or sets the DbStudents value.
        /// </summary>
        public ObservableCollection<Students> DbStudents
        {
            get { return GetValue<ObservableCollection<Students>>(DbStudentsProperty); }
            set { SetValue(DbStudentsProperty, value); }
        }

        /// <summary>
        ///     DbStudents property data.
        /// </summary>
        public static readonly PropertyData DbStudentsProperty = RegisterProperty("DbStudents",
            typeof(ObservableCollection<Students>));

        #endregion

        #region TextFind property

        /// <summary>
        ///     Gets or sets the TextFind value.
        /// </summary>
        public string TextFind
        {
            get { return GetValue<string>(TextFindProperty); }
            set { SetValue(TextFindProperty, value); }
        }

        /// <summary>
        ///     TextFind property data.
        /// </summary>
        public static readonly PropertyData TextFindProperty = RegisterProperty("TextFind", typeof(string), null,
            (sender, e) => ((DataGridStudentsViewModel) sender).OnTextFindChanged());

        /// <summary>
        ///     Called when the TextFind property has changed.
        /// </summary>
        private void OnTextFindChanged()
        {
            if (CBI == "Фамилии")
                DbStudents = new ObservableCollection<Students>(_db.Students.Where(s => s.LastName.StartsWith(TextFind)));
            else if (CBI == "Группе")
                DbStudents =
                    new ObservableCollection<Students>(_db.Students.Where(s => s.CodeGroups.StartsWith(TextFind)));
            else if (CBI == "Возрасту")
                if (TextFind == "")
                {
                    DbStudents = new ObservableCollection<Students>(_db.Students);
                }
                else
                {
                    int? a = Convert.ToInt32(TextFind);
                    DbStudents = new ObservableCollection<Students>(_db.Students.Where(s => s.Age >= a));
                }
        }

        #endregion

        #region SelectedStudents property

        /// <summary>
        ///     Gets or sets the SelectedDBobj value.
        /// </summary>
        public Students SelectedStudents
        {
            get { return GetValue<Students>(SelectedStudentsProperty); }
            set { SetValue(SelectedStudentsProperty, value); }
        }

        /// <summary>
        ///     SelectedDBobj property data.
        /// </summary>
        public static readonly PropertyData SelectedStudentsProperty = RegisterProperty("Students", typeof(Students));

        #endregion

        #endregion

        #region Commands

        #region DbLoaded command

        private TaskCommand _dbLoadedCommand;

        /// <summary>
        ///     Gets the DbLoaded command.
        /// </summary>
        public TaskCommand DbLoadedCommand
        {
            get { return _dbLoadedCommand ?? (_dbLoadedCommand = new TaskCommand(DbLoaded, CanDbLoaded)); }
        }

        /// <summary>
        ///     Method to invoke when the DbLoaded command is executed.
        /// </summary>
        public Task DbLoaded()
        {
            return Task.Factory.StartNew(() =>
            {
                var dependencyResolver = this.GetDependencyResolver();
                var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();

                if (AuthViewModel.LoginCheck)
                {
                    pleaseWaitService.Show("Загружаем таблицы с сервера...");
                    if (DataGridGroupsViewModel.GroupsCollection == null)
                    {
                        _db.Groups.Load();
                        DbGroups = new ObservableCollection<Groups>(_db.Groups);
                        DataGridGroupsViewModel.GroupsCollection = DbGroups;
                    }
                    else
                    {
                        DbGroups = DataGridGroupsViewModel.GroupsCollection;
                    }
                    _db.Students.Load();
                    DbStudents = new ObservableCollection<Students>(_db.Students);
                    pleaseWaitService.Hide();
                }
            });
        }

        private bool CanDbLoaded()
        {
            if (DbStudents == null)
                return true;
            return false;
        }

        #endregion

        #region DbSync command

        private TaskCommand _dbSyncCommand;

        /// <summary>
        ///     Gets the DbSync command.
        /// </summary>
        public TaskCommand DbSyncCommand
        {
            get { return _dbSyncCommand ?? (_dbSyncCommand = new TaskCommand(DbSync, CanDbSync)); }
        }

        /// <summary>
        ///     Method to invoke when the DbSync command is executed.
        /// </summary>
        private Task DbSync()
        {
            return Task.Factory.StartNew(() =>
            {
                var dependencyResolver = this.GetDependencyResolver();
                var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
                pleaseWaitService.Show("Синхронизация данных...");
                if (DataGridGroupsViewModel.GroupsCollection == null)
                {
                    _db.Groups.Load();
                    DbGroups = new ObservableCollection<Groups>(_db.Groups);
                    DataGridGroupsViewModel.GroupsCollection = DbGroups;
                }
                else
                {
                    DbGroups = DataGridGroupsViewModel.GroupsCollection;
                }                
                _db.Students.Load();
                DbStudents = new ObservableCollection<Students>(_db.Students);
                pleaseWaitService.Hide();
            });
        }

        private bool CanDbSync()
        {
            if (DbStudents != null && DbGroups != null)
                return true;
            return false;
        }

        #endregion

        #region DbSave command

        private Command _dbSaveCommand;

        /// <summary>
        ///     Gets the DBUpdate command.
        /// </summary>
        public Command DbSaveCommand
        {
            get { return _dbSaveCommand ?? (_dbSaveCommand = new Command(DbSave, CanDbSave)); }
        }

        /// <summary>
        ///     Method to invoke when the DBUpdate command is executed.
        /// </summary>
        private void DbSave()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
            pleaseWaitService.Show("Сохранение данных...");
            _db.SaveChanges();
            pleaseWaitService.Hide();
        }

        private bool CanDbSave()
        {
            if (DbStudents != null && DbGroups != null)
                return true;
            return false;
        }

        #endregion

        #region DbAdd command

        private Command _dbAddCommand;

        /// <summary>
        ///     Gets the DbAdd command.
        /// </summary>
        public Command DbAddCommand
        {
            get { return _dbAddCommand ?? (_dbAddCommand = new Command(DbAdd, CanDbAddt)); }
        }

        /// <summary>
        ///     Method to invoke when the DbAdd command is executed.
        /// </summary>
        private void DbAdd()
        {
            var viewModel = new WindowDataGridStudentsViewModel();
            _dependencyResolver = this.GetDependencyResolver();
            var pleaseWaitService = _dependencyResolver.Resolve<IPleaseWaitService>();
            _uiVisualizerService = _dependencyResolver.Resolve<IUIVisualizerService>();
            _uiVisualizerService.ShowDialog(viewModel, (sender, e) =>
            {
                if (e.Result ?? false)
                {
                    pleaseWaitService.Show("Сохранение данных...");
                    _db.Students.Add(viewModel.StudentsCollection);
                    DbStudents.Add(viewModel.StudentsCollection);
                    _db.SaveChanges();
                    pleaseWaitService.Hide();
                }
            });
        }

        private bool CanDbAddt()
        {
            if (DbStudents != null && DbGroups != null)
                return true;
            return false;
        }

        #endregion

        #region DbEdit command

        private Command _dbEditCommand;

        /// <summary>
        ///     Gets the DbAdd command.
        /// </summary>
        public Command DbEditCommand
        {
            get { return _dbEditCommand ?? (_dbEditCommand = new Command(DbEdit, CanDbEdit)); }
        }

        /// <summary>
        ///     Method to invoke when the DbAdd command is executed.
        /// </summary>
        private void DbEdit()
        {
            var viewModel = new WindowDataGridStudentsViewModel(SelectedStudents);

            _dependencyResolver = this.GetDependencyResolver();
            _uiVisualizerService = _dependencyResolver.Resolve<IUIVisualizerService>();
            _uiVisualizerService.Show(viewModel);
            _db.SaveChanges();
        }

        private bool CanDbEdit()
        {
            if (SelectedStudents != null && DbStudents != null && DbGroups != null)
                return true;
            return false;
        }

        #endregion

        #region DBDelete command

        private Command _dbDeleteCommand;

        /// <summary>
        ///     Gets the DBDelete command.
        /// </summary>
        public Command DbDeleteCommand
        {
            get { return _dbDeleteCommand ?? (_dbDeleteCommand = new Command(DbDelete, CanDbDelete)); }
        }

        /// <summary>
        ///     Method to invoke when the DBDelete command is executed.
        /// </summary>
        private void DbDelete()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
            pleaseWaitService.Show("Удаление данных...");
            _db.Students.Remove(SelectedStudents);
            DbStudents.Remove(SelectedStudents);
            _db.SaveChanges();
            pleaseWaitService.Hide();
        }

        private bool CanDbDelete()
        {
            if (SelectedStudents != null && DbStudents != null && DbGroups != null)
                return true;
            return false;
        }

        #endregion

        #endregion
    }
}