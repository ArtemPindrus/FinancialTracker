using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinancialTracker.Models;
using FinancialTracker.ViewModels;

namespace FinancialTracker.Views;

public partial class FinanceEditView : UserControl
{
    public FinanceEditView()
    {
        InitializeComponent();

        if (Design.IsDesignMode) {
            FinanceRecordDto record = new() { Tags = ["Tag1", "Tag2"] };
            FinanceUpdateViewModel vm = new(record, ["Tag1", "Tag2", "Tag3", "Tag4"]);

            Design.SetDataContext(this, vm);
        }
    }
}