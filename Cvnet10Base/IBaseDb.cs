using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvnet10Base {
    public interface IBaseCodeName {
		public string Code { get; set; }
		public string? Name { get; set; }
		public string? Ryaku { get; set; }
		public string? Kana { get; set; }
	}
}
