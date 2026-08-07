using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using FinancialTracker.Commands;
using FinancialTracker.Models;
using FinancialTracker.StateMachines;
using FinancialTracket.DataAccessLayer;
using FinancialTracket.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialTracker.ViewModels {
    public partial class FinancesViewModel : MainNavigationPaneViewModel {
        readonly FinancesViewModelStateMachine stateMachine;

        public List<string>? Tags => stateMachine.Tags;

        public ICommand UndoCommand => CommandHistory.UndoCommand;
        public ICommand RedoCommand => CommandHistory.RedoCommand;

        public List<FinanceRecordDto> Finances => stateMachine.Finances;

        public IEnumerable<FinanceRecordDto>? SelectedFinances => SelectedFinancesBind?.Cast<FinanceRecordDto>();

        public IList? SelectedFinancesBind { get; set; }

        public ObservableCollection<MenuItem> AddTagsMenuItems { 
            get; 
        } = [];

        public ObservableCollection<MenuItem> RemoveTagsMenuItems {
            get;
        } = [];

        private CommandHistory CommandHistory => stateMachine.CommandHistory;

        public FinancesViewModel(IDbContextFactory<AppDbContext> dbContextFactory) {
            stateMachine = new(this, dbContextFactory);

            stateMachine.PropertyChanged += StateMachine_PropertyChanged;
            stateMachine.Start();
        }

        private void StateMachine_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(e);
        }

        public override bool CheckCanSafelyClose(out string message) {
            message = string.Empty;
            if (Finances is null) return true;

            if (Finances.Any(f => f.IsModified) || Finances.Any(x => x.IsAdded)) {
                message = "There are unsaved changes. Are you sure you want to close?";
                return false;
            } else return true;
        }

        public override async Task Initialize() {
            stateMachine.DispatchEventNotify(FinancesViewModelStateMachine.EventId.POPULATEREQUEST);
        }

        [RelayCommand]
        private void SendSaveRequest() {
            stateMachine.DispatchEventNotify(FinancesViewModelStateMachine.EventId.SAVEREQUEST);
        }

        [RelayCommand]
        private void Rollback() {
            stateMachine.DispatchEventNotify(FinancesViewModelStateMachine.EventId.POPULATEREQUEST);
        }

        [RelayCommand]
        private void AddTagToSelectedRecords(string tag) {
            CommandHistory.Execute(new AddTagFromSelectedRecordsCommand(tag, this));

        }

        [RelayCommand]
        private void RemoveTagFromSelectedRecords(string tag) {
            CommandHistory.Execute(new RemoveTagFromSelectedRecordsCommand(tag, this));
        }

        [RelayCommand]
        private void MarkRecordDeleted() {
            CommandHistory.Execute(new MarkRecordDeletedCommand(this));
        }

        [RelayCommand]
        private void AddDefaultRecord() {
            CommandHistory.Execute(new AddDefaultFinanceRecord(Finances));
        }
    }
}
