using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using static FinancialTracker.Controls.WrapChildrenWrapper;

namespace FinancialTracker.Controls;

public partial class TagsList : UserControl
{
    private readonly WrapChildrenWrapper children;

    public static readonly StyledProperty<List<string>> TagsProperty =
        AvaloniaProperty.Register<TagsList, List<string>>(nameof(Tags));

    public static readonly StyledProperty<ObservableCollection<string>> SelectedTagsProperty =
        AvaloniaProperty.Register<TagsList, ObservableCollection<string>>(nameof(SelectedTags));

    public List<string> Tags
    {
        get => GetValue(TagsProperty);
        set => SetValue(TagsProperty, value);
    }

    public ObservableCollection<string> SelectedTags {
        get => GetValue(SelectedTagsProperty);
        set => SetValue(SelectedTagsProperty, value);
    }

    public TagsList() {
        InitializeComponent();
        children = new(Wrap);
    }

    protected override void OnInitialized() {
        base.OnInitialized();

        children.SetItemsSource(Tags);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedTagsProperty) {
            if (change.OldValue is ObservableCollection<string> oldSelectedTags) {
                 oldSelectedTags.CollectionChanged -= NewSelectedTags_CollectionChanged;
            }

            UpdateList();

            if (change.NewValue is ObservableCollection<string> newSelectedTags) {
                newSelectedTags.CollectionChanged += NewSelectedTags_CollectionChanged;
            }
        }
    }

    private void NewSelectedTags_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        UpdateList();
    }

    private void UpdateList() {
        children.SelectedTags = SelectedTags;
        children.EnsureHasAtLeast(SelectedTags.Count + 1);

        int vis = SelectedTags.Count + 1;
        children.MakeExclusivelyVisible(vis);

        children.UpdateText();
    }

    private void DeleteTagAt(int i) {
        SelectedTags.RemoveAt(i);
    }
}

public class WrapChildrenWrapper : IEnumerable<AutoCompleteBoxWrapper> {
    private readonly WrapPanel wrap;

    private IEnumerable<string>? itemsSource;

    public AutoCompleteBoxWrapper this[int index] => (AutoCompleteBoxWrapper)wrap.Children[index];

    public IList<string>? SelectedTags { get; set; }

    public IEnumerable<AutoCompleteBoxWrapper> Children => wrap.Children.Cast<AutoCompleteBoxWrapper>();

    public int Count => wrap.Children.Count;

    public WrapChildrenWrapper(WrapPanel wrap) {
        this.wrap = wrap;
    }

    public void Dispose() {
        foreach (var item in Children) {
            item.LostFocus -= Ab_LostFocus;
        }
    }

    /// <summary>
    /// Set ItemsSource property of every child.
    /// </summary>
    /// <param name="itemsSource"></param>
    public void SetItemsSource(IEnumerable<string> itemsSource) {
        this.itemsSource = itemsSource;
        foreach (var c in Children) {
            c.ItemsSource = itemsSource;
        }
    }

    /// <summary>
    /// Make the first <paramref name="vis"/> children visible, while the rest - invisible.
    /// </summary>
    /// <param name="vis">Number of children.</param>
    public void MakeExclusivelyVisible(int vis) {
        for (int i = 0; i < vis; i++) {
            AutoCompleteBox c = this[i];
            c.IsVisible = true;
        }

        for (int i = vis; i < Count; i++) {
            AutoCompleteBox c = this[i];
            c.IsVisible = false;
        }
    }

    /// <summary>
    /// Ensures that there are at least <paramref name="cCount"/> children.
    /// </summary>
    /// <param name="cCount">Children count.</param>
    public void EnsureHasAtLeast(int cCount) {
        int diff = cCount - Count; // 3 - 2 = 1; 2 - 3 = -1

        for (; diff > 0; diff--) {
            AppendAutoCompleteBox();
        }
    }

    public void UpdateText() {
        if (SelectedTags is null) return;

        foreach (var c in Children) {
            int index = c.Index;

            if (SelectedTags.Count > index) {
                c.SelectedItem = SelectedTags[index];
            } else c.SelectedItem = null;
        }
    }

    /// <summary>
    /// Creates a valid, binded AutoCompleteBox and appends it to the end of the sequence.
    /// </summary>
    private void AppendAutoCompleteBox() {
        int index = Children.Count();
        AutoCompleteBoxWrapper ab = new(index) {
            ItemsSource = itemsSource,
        };

        if (SelectedTags is not null
            && SelectedTags.Count > index) {
            ab.SelectedItem = SelectedTags[index];
        }

        ab.LostFocus += Ab_LostFocus;

        AddChild(ab);
    }

    private void Ab_LostFocus(object? sender, RoutedEventArgs e) {
        if (sender is not AutoCompleteBoxWrapper box
            || SelectedTags is null) return;

        string? newTag = box.Text ?? "";
        int tagIndex = box.Index;

        if (tagIndex == SelectedTags.Count) {
            if (!string.IsNullOrWhiteSpace(newTag)) {
                SelectedTags.Add(newTag); 
            }
        }
        else {
            SelectedTags[tagIndex] = newTag;

            if (string.IsNullOrWhiteSpace(newTag)) {
                SelectedTags.RemoveAt(tagIndex);
            }
        }
    }

    private void AddChild(AutoCompleteBoxWrapper child) {
        wrap.Children.Add(child);
    }

    public IEnumerator<AutoCompleteBoxWrapper> GetEnumerator() => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public class AutoCompleteBoxWrapper : AutoCompleteBox {
        protected override Type StyleKeyOverride => typeof(AutoCompleteBox);

        public int Index { get; }

        public AutoCompleteBoxWrapper(int index) {
            Index = index;
        }
    }
}