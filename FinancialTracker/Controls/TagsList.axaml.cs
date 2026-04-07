using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace FinancialTracker.Controls;

public partial class TagsList : UserControl
{
    private readonly WrapChildrenWrapper children;
    private readonly ICommand deleteTagCommand;

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

    public TagsList()
    {
        InitializeComponent();
        children = new(Wrap);

        deleteTagCommand = new RelayCommand<int>(DeleteTagAt);
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
        children.SetDataContext(SelectedTags);
        children.EnsureHasAtLeast(SelectedTags.Count + 1);

        // make redundant children invisible
        int vis = SelectedTags.Count;
        children.EnsureAreVisible(vis);

        for (int i = 0; i < children.Count; i++) {
            var c = children[i];
            if (c.IsVisible) {
                if (c.InnerRightContent is Button innerButton) {
                    if (!innerButton.IsVisible) innerButton.IsVisible = true;
                } else {
                    Button deleteButton = new() {
                        Content = "X",
                        Background = Brushes.Transparent,
                        Command = deleteTagCommand,
                        CommandParameter = i,
                        IsTabStop = false,
                    };

                    c.InnerRightContent = deleteButton;
                }
            }
        }

        // create hanging one
        foreach (var i in children.Where(x => x.IsVisible)) {
            i.KeyBindings.Clear();
        }

        AutoCompleteBox hanging = children[vis];
        hanging.IsVisible = true;
        hanging.Text = null;

        if (hanging.InnerRightContent is Button b) b.IsVisible = false;

        RelayCommand enterCommand = new(() => {
            if (string.IsNullOrWhiteSpace(hanging.Text)) return;

            if (hanging.InnerRightContent is Button b) b.IsVisible = true;

            SelectedTags.Add(hanging.Text);
        });

        KeyBinding enterKey = new() {
            Gesture = new KeyGesture(Key.Enter),
            Command = enterCommand
        };

        hanging.KeyBindings.Add(enterKey);
    }

    private void DeleteTagAt(int i) {
        SelectedTags.RemoveAt(i);
    }
}

public class WrapChildrenWrapper : IEnumerable<AutoCompleteBox> {
    private const int BindingDelay = 300;

    private readonly WrapPanel wrap;
    private IEnumerable<string>? itemsSource;
    private object? dataContext;

    public AutoCompleteBox this[int index] => (AutoCompleteBox)wrap.Children[index];

    public IEnumerable<AutoCompleteBox> Children => wrap.Children.Cast<AutoCompleteBox>();

    public int Count => wrap.Children.Count;

    public WrapChildrenWrapper(WrapPanel wrap) {
        this.wrap = wrap;
    }

    public void SetItemsSource(IEnumerable<string> itemsSource) {
        this.itemsSource = itemsSource;
        foreach (AutoCompleteBox c in Children) {
            c.ItemsSource = itemsSource;
        }
    }

    public void SetDataContext(object? dataContext) {
        this.dataContext = dataContext;
        foreach (AutoCompleteBox c in Children) {
            c.DataContext = dataContext;
        }
    }

    public void EnsureAreVisible(int vis) {
        for (int i = 0; i < vis; i++) {
            AutoCompleteBox c = this[i];
            c.IsVisible = true;
        }

        for (int i = vis + 1; i < Count; i++) {
            AutoCompleteBox c = this[i];
            c.IsVisible = false;
        }
    }

    public void EnsureHasAtLeast(int cCount) {
        int diff = cCount - Count; // 3 - 2 = 1; 2 - 3 = -1

        int i = Count;
        for (; diff > 0; diff--) {
            AutoCompleteBox ab = new() {
                DataContext = dataContext,
                ItemsSource = itemsSource,
            };

            var binding = new Binding($"[{i}]") { Delay = BindingDelay };
            ab.Bind(AutoCompleteBox.TextProperty, binding);

            AddChild(ab);
            i++;
        }
    }

    private void AddChild(AutoCompleteBox child) {
        wrap.Children.Add(child);
    }

    public IEnumerator<AutoCompleteBox> GetEnumerator() => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}