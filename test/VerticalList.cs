﻿using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace test
{
	public class VerticalList : Grid
	{
		protected readonly ICommand SelectedCommand;
		protected readonly StackLayout ItemsStackLayout;

		public event EventHandler SelectedItemChanged;

		public static StackOrientation ListOrientation { get; set; }

		public VerticalList ()
		{
			SelectedCommand = new Command<object> (item => {
				var selectable = item as ISelectable;
				if (selectable == null)
					return;

				SetSelected (selectable);
				SelectedItem = selectable.IsSelected ? selectable : null;
			});

			ItemsStackLayout = new StackLayout {
				Orientation = ListOrientation, 
				Padding = this.Padding,  
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			Children.Add (ItemsStackLayout);
		}

		public static readonly BindableProperty CommandProperty = 
			BindableProperty.Create<VerticalList, ICommand> (p => p.Command, null);

		public ICommand Command {
			get { return (ICommand)GetValue (CommandProperty); }
			set { SetValue (CommandProperty, value); }
		}

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create<VerticalList, IEnumerable> (p => p.ItemsSource, default(IEnumerable<object>), BindingMode.TwoWay, null, ItemsSourceChanged);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create<VerticalList, object> (p => p.SelectedItem, default(object), BindingMode.TwoWay, null, OnSelectedItemChanged);

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create<VerticalList, DataTemplate> (p => p.ItemTemplate, default(DataTemplate));

		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue (ItemsSourceProperty); }
			set { SetValue (ItemsSourceProperty, value); }
		}

		public object SelectedItem {
			get { return (object)GetValue (SelectedItemProperty); }
			set { SetValue (SelectedItemProperty, value); }
		}

		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue (ItemTemplateProperty); }
			set { SetValue (ItemTemplateProperty, value); }
		}

		private static void ItemsSourceChanged (BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
		{
			var itemsLayout = (VerticalList)bindable;
			itemsLayout.SetItems ();
		}

		protected virtual void SetItems ()
		{
			ItemsStackLayout.Children.Clear ();

			if (ItemsSource == null)
				return;

			foreach (var item in ItemsSource)
				ItemsStackLayout.Children.Add (GetItemView (item));

			SelectedItem = ItemsSource.OfType<ISelectable> ().FirstOrDefault (x => x.IsSelected);
		}

		protected virtual View GetItemView (object item)
		{
			var content = ItemTemplate.CreateContent ();
			var view = content as View;
			if (view == null)
				return null;

			view.BindingContext = item;

			var gesture = new TapGestureRecognizer {
				Command = SelectedCommand,
				CommandParameter = item
			};

			AddGesture (view, gesture);

			return view;
		}

		protected void AddGesture (View view, TapGestureRecognizer gesture)
		{
			view.GestureRecognizers.Add (gesture);

			var layout = view as Layout<View>;

			if (layout == null)
				return;

			foreach (var child in layout.Children)
				AddGesture (child, gesture);
		}

		protected virtual void SetSelected (ISelectable selectable)
		{
			selectable.IsSelected = true;
		}

		protected virtual void SetSelectedItem (ISelectable selectedItem)
		{
			var items = ItemsSource;

			foreach (var item in items.OfType<ISelectable>())
				item.IsSelected = selectedItem != null && item == selectedItem && selectedItem.IsSelected;

			var handler = SelectedItemChanged;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		private static void OnSelectedItemChanged (BindableObject bindable, object oldValue, object newValue)
		{
			var itemsView = (VerticalList)bindable;
			if (newValue == oldValue)
				return;

			var selectable = newValue as ISelectable;
			itemsView.SetSelectedItem (selectable ?? oldValue as ISelectable);
		}

	}


	public interface ISelectable
	{
		bool IsSelected { get; set; }

		ICommand SelectCommand { get; set; }
	}
}

