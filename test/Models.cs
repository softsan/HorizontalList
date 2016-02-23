using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace test
{
	
	public class Categories
	{
		public int Id{ get; set; }

		public string Name { get; set; }

		public List<CategoryItems> CategoryItem{ get; set; }
	}

	public class CategoryItems
	{
		public int Id { get; set; }

		public string ItemName { get; set; }

		public int CatId { get; set; }

		public string ImageUrl { get; set; }
	}
}

