using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FinancialTracker.Controls {
    public class NotifyingDataGrid : DataGrid {
        protected override Type StyleKeyOverride => typeof(DataGrid);

        public readonly static StyledProperty<IList> SelectedItemsBindingProperty =
            AvaloniaProperty.Register<NotifyingDataGrid, IList>(
                nameof(SelectedItemsBinding),
                defaultBindingMode: Avalonia.Data.BindingMode.OneWayToSource
            );

        public IList SelectedItemsBinding {
            get => SelectedItems;
        }

        public NotifyingDataGrid() {
            SetValue(SelectedItemsBindingProperty, SelectedItems);
        }
    }
}
